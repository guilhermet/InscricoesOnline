using InscricoesOnline.Models;
using InscricoesOnline.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace InscricoesOnline.Controllers
{
    public class HomeController : Controller
    {
        private IOContext db = new IOContext();

        public ActionResult Index()
        {
            var eventos = db.Eventos.Where(e => e.Ativo).ToList();

            if (eventos.Count == 1)
                return RedirectToAction("Evento", new { id = eventos.FirstOrDefault().Id });

            return View(eventos);
        }

        [Route("Eventos/{id}")]
        public ActionResult Evento(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var evento = db.Eventos.Find(id);

            if (evento == null)
            {
                return HttpNotFound();
            }

            return View(evento);
        }

        public ActionResult Login(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var evento = db.Eventos.Find(id);

            if (evento == null)
            {
                return HttpNotFound();
            }

            var equipe = new Equipe()
            {
                EventoId = evento.Id,
            };

            return View(equipe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Equipe model)
        {
            var equipe = db.Equipes.Where(u => u.Login == model.Login && u.Senha == model.Senha && u.EventoId == model.EventoId).FirstOrDefault();

            if (equipe != null)
            {
                SiteSessionPersister.IdEquipe = equipe.Id;
                //if (model.RememberMe)
                //{
                //    Response.Cookies.Get("iousername").Value = AdminSessionPersister.Username;
                //    Response.Cookies.Get("iousername").Expires = DateTime.Now.AddYears(2);
                //}

                return RedirectToAction("Inscricoes", new { id = equipe.Id });
            }
            else
            {
                ViewBag.Erro = "Usuário ou senha inválida";
                return View(model);
            }
        }

        public ActionResult Equipe(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            var evento = db.Eventos.Find(id);

            if (evento == null)
            {
                return HttpNotFound();
            }

            var equipe = new Equipe()
            {
                EventoId = evento.Id
            };

            return View(equipe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Equipe(Equipe equipe)
        {
            equipe.DataRegistro = DateTime.Now;
            if (ModelState.IsValid)
            {
                equipe.Nome = equipe.Nome.ToUpper();
                db.Equipes.Add(equipe);
                db.SaveChanges();

                return RedirectToAction("Inscricoes", new { id = equipe.Id });
            }

            return View(equipe);
        }

        [Route("Inscricoes/{id}")]
        public ActionResult Inscricoes(long? id)
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

            return View(equipe);
        }
    }
}