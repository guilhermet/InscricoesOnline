using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InscricoesOnline.Models;
using System.Web.Routing;
using InscricoesOnline.Security;
using InscricoesOnline.ViewModel;

namespace InscricoesOnline.Controllers.Admin.Campeonato
{
    [AdminAuthorize(Roles = "Campeonato")]
    public class CategoriaIdadeController : Controller
    {
        private IOContext db = new IOContext();

        [Route("Admin/CategoriaIdade/Lista")]
        public ActionResult Lista()
        {
            var categoriaIdades = db.CategoriaIdades.Include(c => c.Modalidade);
            return View(categoriaIdades.Where(f => f.EventoId == AdminSessionPersister.Evento.Id).OrderBy(c => new { c.Modalidade.Titulo, c.IdadeInicial }).ToList());
        }

        [Route("Admin/CategoriaIdade/Novo")]
        public ActionResult Novo(CategoriaIdade categoriaIdade)
        {
            ViewBag.ModalidadeId = new SelectList(db.Modalidades, "Id", "Titulo");
            ViewBag.Faixas = db.Faixas.OrderBy(f => f.Ordem).ToList();
            return View(categoriaIdade);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/CategoriaIdade/NovoSalvar")]
        public ActionResult NovoSalvar(CategoriaIdade categoriaIdade)
        {
            if (ModelState.IsValid)
            {
                categoriaIdade.EventoId = AdminSessionPersister.Evento.Id;
                db.CategoriaIdades.Add(categoriaIdade);
                db.SaveChanges();
                return RedirectToAction("Lista");
            }

            ViewBag.ModalidadeId = new SelectList(db.Modalidades, "Id", "Titulo", categoriaIdade.ModalidadeId);
            ViewBag.Faixas = db.Faixas.OrderBy(f => f.Ordem).ToList();
            return View("Novo", categoriaIdade);
        }

        [Route("Admin/CategoriaIdade/Edit/{id}")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoriaIdade categoriaIdade = db.CategoriaIdades.Where(c => c.Id == id && c.EventoId == AdminSessionPersister.Evento.Id).FirstOrDefault();
            if (categoriaIdade == null)
            {
                return HttpNotFound();
            }
            ViewBag.ModalidadeId = new SelectList(db.Modalidades, "Id", "Titulo", categoriaIdade.ModalidadeId);
            ViewBag.Faixas = db.Faixas.OrderBy(f => f.Ordem).ToList();
            return View(categoriaIdade);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/CategoriaIdade/EditSalvar")]
        public ActionResult EditSalvar(CategoriaIdade categoriaIdade)
        {
            if (ModelState.IsValid)
            {
                db.Entry(categoriaIdade).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Lista");
            }
            ViewBag.ModalidadeId = new SelectList(db.Modalidades, "Id", "Titulo", categoriaIdade.ModalidadeId);
            ViewBag.Faixas = db.Faixas.OrderBy(f => f.Ordem).ToList();
            return View("Edit", categoriaIdade);
        }

        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoriaIdade categoriaIdade = db.CategoriaIdades.Where(c => c.Id == id && c.EventoId == AdminSessionPersister.Evento.Id).FirstOrDefault();
            db.CategoriaIdades.Remove(categoriaIdade);
            db.SaveChanges();
            return RedirectToAction("Lista");
        }

        [Route("Admin/CategoriaIdade/CategoriaPeso/Lista/{id}")]
        public ActionResult ListaCategoriaLutaPeso(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var categoriaIdade = db.CategoriaIdades.Find(id);
            var categoriaPesoViewModel = new CategoriaPesoViewModel
            {
                CategoriaIdadeId = categoriaIdade.Id,
                CategoriaIdadeTitulo = categoriaIdade.Titulo,
                CategoriaLutaPeso = db.CategoriaLutaPeso.Where(i => i.CategoriaIdadeId == id).OrderBy(c => new { c.Sexo, c.PesoInicial }).ToList()
            };
            return View(categoriaPesoViewModel);
        }

        [Route("Admin/CategoriaIdade/CategoriaPeso/Novo/{id}")]
        public ActionResult NovoCategoriaLutaPeso(long id)
        {
            var categoriaIdade = db.CategoriaIdades.Find(id);
            var categoriaLutaPeso = new CategoriaLutaPeso
            {
                CategoriaIdadeId = categoriaIdade.Id,
                CategoriaIdade = categoriaIdade
            };
            return View(categoriaLutaPeso);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/CategoriaIdade/CategoriaPeso/NovoSalvar")]
        public ActionResult NovoCategoriaLutaPesoSalvar(CategoriaLutaPeso CategoriaLutaPeso)
        {
            if (ModelState.IsValid)
            {
                CategoriaLutaPeso.EventoId = AdminSessionPersister.Evento.Id;
                db.CategoriaLutaPeso.Add(CategoriaLutaPeso);
                db.SaveChanges();
                return RedirectToAction("ListaCategoriaLutaPeso", new RouteValueDictionary(new { id = CategoriaLutaPeso.CategoriaIdadeId }));
            }

            return View("NovoCategoriaPeso", CategoriaLutaPeso);
        }

        [Route("Admin/CategoriaIdade/CategoriaPeso/Edit/{id}")]
        public ActionResult EditCategoriaLutaPeso(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var categoriaLutaPeso = db.CategoriaLutaPeso.Find(id);
            if (categoriaLutaPeso == null)
            {
                return HttpNotFound();
            }
            return View(categoriaLutaPeso);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/CategoriaIdade/CategoriaPeso/EditSalvar")]
        public ActionResult EditCategoriaLutaPesoSalvar(CategoriaLutaPeso categoriaLutaPeso)
        {
            if (ModelState.IsValid)
            {
                db.Entry(categoriaLutaPeso).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ListaCategoriaLutaPeso", new RouteValueDictionary(new { id = categoriaLutaPeso.CategoriaIdadeId }));
            }
            return View("EditCategoriaPeso", categoriaLutaPeso);
        }

        public ActionResult DeleteCategoriaLutaPeso(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var categoriaLutaPeso = db.CategoriaLutaPeso .Find(id);
            if (categoriaLutaPeso == null)
            {
                return HttpNotFound();
            }
            db.CategoriaLutaPeso.Remove(categoriaLutaPeso);
            db.SaveChanges();
            return RedirectToAction("ListaCategoriaLutaPeso", new RouteValueDictionary(new { id = categoriaLutaPeso.CategoriaIdadeId }));
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
