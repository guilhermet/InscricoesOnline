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
    [AdminAuthorizeAttribute]
    public class ModalidadesController : Controller
    {
        private IOContext db = new IOContext();

        [Route("Admin/Modalidades/Lista")]
        public ActionResult Lista()
        {
            return View(db.Modalidades.Where(f => f.EventoId == AdminSessionPersister.Evento.Id).OrderBy(m => m.Titulo).ToList());
        }

        [Route("Admin/Modalidades/Novo")]
        public ActionResult Novo(Modalidade modalidade)
        {
            return View(modalidade);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Modalidades/NovoSalvar")]
        public ActionResult NovoSalvar(Modalidade modalidade)
        {
            modalidade.EventoId = AdminSessionPersister.Evento.Id;
            if (ModelState.IsValid)
            {
                db.Modalidades.Add(modalidade);
                db.SaveChanges();
                return RedirectToAction("Lista");
            }
            return View("Novo", modalidade);
        }

        [Route("Admin/Modalidades/Edit/{id}")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Modalidade modalidade = db.Modalidades.Where(m => m.Id == id && m.EventoId == AdminSessionPersister.Evento.Id).FirstOrDefault();
            if (modalidade == null)
            {
                return HttpNotFound();
            }
            return View(modalidade);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Modalidades/EditSalvar")]
        public ActionResult EditSalvar(Modalidade modalidade)
        {
            if (ModelState.IsValid)
            {
                db.Entry(modalidade).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Lista");
            }
            return View("Edit", modalidade);
        }

        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Modalidade modalidade = db.Modalidades.Where(m => m.Id == id && m.EventoId == AdminSessionPersister.Evento.Id).FirstOrDefault();
            db.Modalidades.Remove(modalidade);
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
