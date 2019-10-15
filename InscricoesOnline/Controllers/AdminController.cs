using InscricoesOnline.Models;
using InscricoesOnline.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InscricoesOnline.Controllers
{
    [AdminAuthorize]
    public class AdminController : Controller
    {
        private IOContext db = new IOContext();

        [Route("Admin")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("Admin/BuscaPesos/{idCategoriaIdade}/{sexo}")]
        public ActionResult BuscaPesos(long idCategoriaIdade, string sexo)
        {
            var categoriaPesos = db.CategoriaLutaPeso.Where(c => c.CategoriaIdadeId == idCategoriaIdade && c.Sexo == sexo).OrderBy(c => c.PesoInicial).ToList();

            return Json(categoriaPesos);
        }
    }
}