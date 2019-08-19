using InscricoesOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InscricoesOnline.Security
{
    public static class SiteSessionPersister
    {
        static string usernameSessionVar = "username";

        public static long? IdEquipe
        {
            get
            {
                if (HttpContext.Current == null)
                    return null;

                var sessionVar = HttpContext.Current.Session[usernameSessionVar];
                if (sessionVar != null)
                    return sessionVar as long?;

                return null;
            }
            set
            {
                HttpContext.Current.Session[usernameSessionVar] = value;
            }
        }

        public static Equipe Equipe
        {
            get
            {
                if (HttpContext.Current == null)
                    return null;

                var db = new IOContext();
                var idEquipe = Convert.ToInt64(HttpContext.Current.Session[usernameSessionVar]);
                var equipe = db.Equipes.Where(e => e.Id == idEquipe).FirstOrDefault();

                if (equipe != null)
                    return equipe;

                return null;
            }
        }
    }
}