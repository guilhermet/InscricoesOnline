using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Drawing;
using InscricoesOnline.Security;
using InscricoesOnline.Models;

namespace InscricoesOnline.Controllers.Admin.Cadastro
{
    [AdminAuthorize]
    public class EventosController : Controller
    {
        private IOContext db = new IOContext();

        
        [Route("Admin/Eventos/Edit")]
        public ActionResult Edit()
        {
            Evento evento = db.Eventos.Find(AdminSessionPersister.Evento.Id);
            if (evento == null)
            {
                return HttpNotFound();
            }
            
            return View(evento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [Route("Admin/Eventos/EditSalvar")]
        public ActionResult EditSalvar(Evento evento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(evento).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Admin");
            }

            return View("Edit", evento);
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
