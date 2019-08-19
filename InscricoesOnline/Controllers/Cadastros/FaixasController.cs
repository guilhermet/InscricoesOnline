using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using InscricoesOnline.Models;
using InscricoesOnline.Security;

namespace InscricoesOnline.Controllers.Admin.Cadastro
{
    [AdminAuthorizeAttribute]
    public class FaixasController : Controller
    {
        private IOContext db = new IOContext();

        [Route("Admin/Faixas/Lista")]
        public ActionResult Lista()
        {
            return View(db.Faixas.Where(f => f.EventoId == AdminSessionPersister.Evento.Id).OrderBy(f => f.Ordem).ToList());
        }

        [Route("Admin/Faixas/Novo")]
        public ActionResult Novo()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Faixas/NovoSalvar")]
        public ActionResult NovoSalvar(Faixa faixa)
        {
            var faixas = db.Faixas.OrderBy(f => f.Ordem);
            int i = 1;
            while (faixas.Where(f => f.Ordem == i).FirstOrDefault() != null)
                i++;

            faixa.Ordem = i;
            faixa.DataRegistro = DateTime.Now;
            faixa.EventoId = AdminSessionPersister.Evento.Id;
            if (ModelState.IsValid)
            {
                db.Faixas.Add(faixa);
                db.SaveChanges();
                return RedirectToAction("Lista");
            }

            return View("Novo", faixa);
        }

        [Route("Admin/Faixas/Editar")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Faixa faixa = db.Faixas.Where(f => f.Id == id && f.EventoId == AdminSessionPersister.Evento.Id).FirstOrDefault();
            if (faixa == null)
            {
                return HttpNotFound();
            }
            return View(faixa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Faixas/EditarSalvar")]
        public ActionResult EditSalvar(Faixa faixa)
        {
            if (ModelState.IsValid)
            {
                db.Entry(faixa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Lista");
            }
            return View("Edit", faixa);
        }

        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Faixa faixa = db.Faixas.Where(f => f.Id == id && f.EventoId == AdminSessionPersister.Evento.Id).FirstOrDefault();
            if (faixa == null)
            {
                return HttpNotFound();
            }
            db.Faixas.Remove(faixa);
            db.SaveChanges();
            return RedirectToAction("Lista");
        }

        [Route("Admin/Faixas/Reordenar")]
        public ActionResult Reordenar()
        {
            return View(db.Faixas.OrderBy(f => f.Ordem).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Faixas/ReordenarSalvar")]
        public ActionResult ReordenarSalvar(List<Faixa> faixas)
        {
            foreach (var faixa in faixas)
            {
                db.Entry(faixa).State = EntityState.Modified;
            }
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
