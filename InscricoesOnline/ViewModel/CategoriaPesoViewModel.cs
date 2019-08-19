using InscricoesOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InscricoesOnline.ViewModel
{
    public class CategoriaPesoViewModel
    {
        public long CategoriaIdadeId { get; set; }
        public String CategoriaIdadeTitulo { get; set; }
        public IEnumerable<CategoriaLutaPeso> CategoriaLutaPeso { get; set; }
    }
}