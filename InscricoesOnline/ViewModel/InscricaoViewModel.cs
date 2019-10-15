using InscricoesOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InscricoesOnline.ViewModel
{
    public class InscricaoViewModel
    {
        public Equipe Equipe { get; set; }
        public Atleta Atleta { get; set; }
        public IEnumerable<InscricaoModalidade> InscricoesModalidade { get; set; }
    }
}