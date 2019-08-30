using InscricoesOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InscricoesOnline.ViewModel
{
    public class ViewModelInscricoes
    {
        public Equipe Equipe { get; set; }
        public IEnumerable<Inscricao> Inscricoes { get; set; }
    }
}