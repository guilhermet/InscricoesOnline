using InscricoesOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace InscricoesOnline.Security
{
    public class AdminPrincipal : IPrincipal
    {
        private Evento Evento;
        private IOContext db = new IOContext();

        public AdminPrincipal(Evento evento)
        {
            this.Evento = evento;
            this.Identity = new GenericIdentity(evento.Login);
        }

        public IIdentity Identity { get; set; }

        public bool IsInRole(string role)
        {
            return true;
        }
    }
}