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
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var cookie = filterContext.HttpContext.Request.Cookies["username"];
            if (cookie != null)
            {
                var value = cookie.Value;
                if (String.IsNullOrEmpty(AdminSessionPersister.Username) && !String.IsNullOrEmpty(value))
                {
                    AdminSessionPersister.Username = value;
                }
            }

            if (string.IsNullOrEmpty(AdminSessionPersister.Username))
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Account",
                    action = "Login",
                    returnUrl = filterContext.HttpContext.Request.Url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped)
                }));
            else
            {
                var db = new IOContext();
                var evento = db.Eventos.Where(u => u.Login == AdminSessionPersister.Username && u.Ativo).FirstOrDefault();

                if (evento == null)
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "Account",
                        action = "Login",
                        returnUrl = filterContext.HttpContext.Request.Url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped)
                    }));
                }
                else
                {
                    AdminPrincipal p = new AdminPrincipal(evento);

                    if (!p.IsInRole(Roles))
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Admin", action = "Index" }));
                }
                db.Dispose();
            }
        }
    }
}