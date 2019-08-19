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

namespace InscricoesOnline.Controllers.Admin.Cadastro
{
    public class AcademiasController : Controller
    {
        private IOContext db = new IOContext();

        [Route("Admin/Academias/Lista")]
        public ActionResult Lista()
        {
            return View(db.Equipes.OrderBy(a => a.Nome).ToList());
        }

        [Route("Admin/Academias/Visualizar/{id}")]
        public ActionResult Visualizar(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipe equipe = db.Equipes.Find(id);
            if (equipe == null)
            {
                return HttpNotFound();
            }
            return View(equipe);
        }

        [Route("Admin/Academias/Novo")]
        public ActionResult Novo()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Academias/NovoSalvar")]
        public ActionResult NovoSalvar(Equipe equipe)
        {
            equipe.DataRegistro = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Equipes.Add(equipe);
                db.SaveChanges();
                return RedirectToAction("Visualizar", new { id = equipe.Id });
            }

            return View("Novo", equipe);
        }

        [Route("Admin/Academias/Edit/{id}")]
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
        [Route("Admin/Academias/EditSalvar")]
        public ActionResult EditSalvar(Equipe equipe, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                db.Entry(equipe).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Visualizar", new { id = equipe.Id });
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
