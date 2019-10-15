using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.IO;
using System.Drawing;
using InscricoesOnline.Models;
using InscricoesOnline.Security;

namespace InscricoesOnline.Controllers.Admin.Cadastro
{
    [AdminAuthorizeAttribute]
    public class EquipesController : Controller
    {
        private IOContext db = new IOContext();

        [Route("Admin/Equipes/Lista")]
        public ActionResult Lista()
        {
            return View(db.Equipes.Where(a => a.EventoId == AdminSessionPersister.Evento.Id).OrderBy(a => a.Nome).ToList());
        }

        [Route("Admin/Equipes/Novo")]
        public ActionResult Novo()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Equipes/NovoSalvar")]
        public ActionResult NovoSalvar(Equipe equipe)
        {
            equipe.DataRegistro = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Equipes.Add(equipe);
                db.SaveChanges();
                return RedirectToAction("Lista", new { id = equipe.Id });
            }

            return View("Novo", equipe);
        }

        [Route("Admin/Equipes/Edit/{id}")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipe academia = db.Equipes.Find(id);
            if (academia == null)
            {
                return HttpNotFound();
            }

            return View(academia);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Equipes/EditSalvar")]
        public ActionResult EditSalvar(Equipe equipe)
        {
            if (ModelState.IsValid)
            {
                db.Entry(equipe).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Lista", new { id = equipe.Id });
            }

            return View("Edit", equipe);
        }

        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var equipe = db.Equipes.Find(id);
            if (equipe == null)
            {
                return HttpNotFound();
            }

            db.Equipes.Remove(equipe);
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
