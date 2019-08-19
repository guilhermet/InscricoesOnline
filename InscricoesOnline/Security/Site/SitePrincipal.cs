using InscricoesOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace InscricoesOnline.Security
{
    public class SitePrincipal : IPrincipal
    {
        private Equipe Equipe;
        private IOContext db = new IOContext();

        public SitePrincipal(Equipe equipe)
        {
            this.Equipe = equipe;
            this.Identity = new GenericIdentity(Equipe.Nome);
        }

        public IIdentity Identity { get; set; }

        public bool IsInRole(string role)
        {
            if (role == string.Empty)
                return true;

            return true;
        }
    }
}