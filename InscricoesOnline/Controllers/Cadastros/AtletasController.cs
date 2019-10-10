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
using System.Text.RegularExpressions;
using InscricoesOnline.Models;
using InscricoesOnline.Security;

namespace InscricoesOnline.Controllers.Admin.Cadastro
{
    [AdminAuthorizeAttribute]
    public class AtletasController : Controller
    {
        private IOContext db = new IOContext();

        [Route("Admin/Atletas/Lista")]
        public ActionResult Lista(long? idEquipe)
        {
            var filiados = db.Atletas.Where(f => (idEquipe != null && idEquipe != 0 ? f.EquipeId == idEquipe : true) && f.Equipe.EventoId == AdminSessionPersister.Evento.Id).OrderByDescending(f => f.Ativo).ThenBy(f => f.Nome);

            ViewBag.TotalFiliados = filiados.Count();
            ViewBag.Academia = idEquipe;

            return View(filiados.ToList());
        }

        [Route("Admin/Atletas/Visualizar/{id}")]
        public ActionResult Visualizar(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Atleta filiado = db.Atletas.Find(id);

            if (filiado == null)
            {
                return HttpNotFound();
            }

            return View(filiado);
        }

        [Route("Admin/Atletas/Novo")]
        public ActionResult Novo(Atleta atleta)
        {
            ViewBag.Faixas = db.Faixas.OrderBy(f => f.Ordem).ToList();
            atleta.Ativo = true;
            return View(atleta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Atletas/NovoSalvar")]
        public ActionResult NovoSalvar(Atleta atleta)
        {
            atleta.DataRegistro = DateTime.Now;
            if (ModelState.IsValid)
            {
                atleta.Nome = atleta.Nome.ToUpper();
                db.Atletas.Add(atleta);
                db.SaveChanges();

                return RedirectToAction("Visualizar", new { id = atleta.Id });
            }

            ViewBag.Faixas = db.Faixas.OrderBy(f => f.Ordem).ToList();
            return View("Novo", atleta);
        }

        [Route("Admin/Atletas/Editar/{id}")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Atleta atleta = db.Atletas.Find(id);
            if (atleta == null)
            {
                return HttpNotFound();
            }

            ViewBag.Faixas = db.Faixas.OrderBy(f => f.Ordem).ToList();
            return View(atleta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Atletas/EditarSalvar")]
        public ActionResult EditSalvar(Atleta atleta)
        {
            if (ModelState.IsValid)
            {
                atleta.Nome = atleta.Nome.ToUpper();

                db.Entry(atleta).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Visualizar", new { id = atleta.Id });
            }

            ViewBag.Faixas = db.Faixas.OrderBy(f => f.Ordem).ToList();
            return View("Edit", atleta);
        }

        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Atleta atleta = db.Atletas.Find(id);
            if (atleta == null)
            {
                return HttpNotFound();
            }

            db.Atletas.Remove(atleta);
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
