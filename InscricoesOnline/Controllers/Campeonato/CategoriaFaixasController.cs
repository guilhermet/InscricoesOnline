using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InscricoesOnline.Models;
using InscricoesOnline.Security;

namespace InscricoesOnline.Controllers.Admin.Campeonato
{
    [AdminAuthorize(Roles = "Campeonato")]
    public class CategoriaFaixaController : Controller
    {
        private IOContext db = new IOContext();

        [Route("Admin/CategoriaFaixas/Lista")]
        public ActionResult Lista()
        {
            var categoriaFaixas = db.CategoriaFaixas.Include(c => c.FaixaFinal).Include(c => c.FaixaInicial).Include(c => c.Modalidade);
            return View(categoriaFaixas.OrderBy(c => new { c.Modalidade.Titulo, c.FaixaInicial.Ordem }).ToList());
        }

        [Route("Admin/CategoriaFaixas/Novo")]
        public ActionResult Novo(CategoriaFaixa categoriaFaixa)
        {
            ViewBag.Faixas = db.Faixas.OrderBy(f => f.Ordem).ToList();
            ViewBag.ModalidadeId = new SelectList(db.Modalidades, "Id", "Titulo");
            return View(categoriaFaixa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/CategoriaFaixas/NovoSalvar")]
        public ActionResult NovoSalvar(CategoriaFaixa categoriaFaixa)
        {
            if (ModelState.IsValid)
            {
                db.CategoriaFaixas.Add(categoriaFaixa);
                db.SaveChanges();
                return RedirectToAction("Lista");
            }

            ViewBag.Faixas = db.Faixas.OrderBy(f => f.Ordem).ToList();
            ViewBag.ModalidadeId = new SelectList(db.Modalidades, "Id", "Titulo", categoriaFaixa.ModalidadeId);
            return View("Novo", categoriaFaixa);
        }

        [Route("Admin/CategoriaFaixas/Edit")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoriaFaixa categoriaFaixa = db.CategoriaFaixas.Find(id);
            if (categoriaFaixa == null)
            {
                return HttpNotFound();
            }

            ViewBag.Faixas = db.Faixas.OrderBy(f => f.Ordem).ToList();
            ViewBag.ModalidadeId = new SelectList(db.Modalidades, "Id", "Titulo", categoriaFaixa.ModalidadeId);
            return View(categoriaFaixa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/CategoriaFaixas/EditSalvar")]
        public ActionResult EditSalvar(CategoriaFaixa categoriaFaixa)
        {
            if (ModelState.IsValid)
            {
                db.Entry(categoriaFaixa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Lista");
            }
            
            ViewBag.Faixas = db.Faixas.OrderBy(f => f.Ordem).ToList();
            ViewBag.ModalidadeId = new SelectList(db.Modalidades, "Id", "Titulo", categoriaFaixa.ModalidadeId);
            return View("Edit", categoriaFaixa);
        }

        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoriaFaixa categoriaFaixa = db.CategoriaFaixas.Find(id);
            db.CategoriaFaixas.Remove(categoriaFaixa);
            db.SaveChanges();
            return RedirectToAction("Lista");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
