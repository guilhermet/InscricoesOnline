using InscricoesOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace InscricoesOnline.Security
{
    public class SiteAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var cookie = filterContext.HttpContext.Request.Cookies["site_user"];
            if (cookie != null)
            {
                var value = cookie.Value;
                if (SiteSessionPersister.IdEquipe == null && !String.IsNullOrEmpty(value))
                {
                    SiteSessionPersister.IdEquipe = Convert.ToInt64(value);
                }
            }

            if (SiteSessionPersister.IdEquipe == null)
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Home",
                    action = "Index",
                    returnUrl = filterContext.HttpContext.Request.Url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped)
                }));
            else
            {
                var db = new IOContext();
                var usuario = db.Equipes.Where(u => u.Id == SiteSessionPersister.IdEquipe).FirstOrDefault();
                if (usuario == null)
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "Home",
                        action = "Login",
                        returnUrl = filterContext.HttpContext.Request.Url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped)
                    }));
                }
                else
                {
                    SitePrincipal p = new SitePrincipal(usuario);

                    if (!p.IsInRole(Roles))
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Index" }));
                }
            }
        }
    }
}