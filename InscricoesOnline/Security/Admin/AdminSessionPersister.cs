using InscricoesOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InscricoesOnline.Security
{
    public static class AdminSessionPersister
    {
        static string usernameSessionVar = "username";

        public static string Username
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;

                var sessionVar = HttpContext.Current.Session[usernameSessionVar];
                if (sessionVar != null)
                    return sessionVar as string;

                return null;
            }
            set
            {
                HttpContext.Current.Session[usernameSessionVar] = value;
            }
        }

        public static Evento Evento
        {
            get
            {
                if (HttpContext.Current == null)
                    return null;

                var db = new IOContext();
                var username = Convert.ToString(HttpContext.Current.Session[usernameSessionVar]);
                var usuario = db.Eventos.Where(u => u.Login == username).FirstOrDefault();

                if (usuario != null)
                    return usuario;

                return null;
            }
        }
    }
}