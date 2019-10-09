using InscricoesOnline.Models;
using InscricoesOnline.Security;
using InscricoesOnline.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

            var equipes = db.Equipes.Where(e => e.Email == equipe.Email).ToList();

            if (equipes.Count > 0)
                ViewBag.Error = "E-mail já está cadastrado no sistema";
            else
            {
                if (ModelState.IsValid)
                {
                    equipe.Nome = equipe.Nome.ToUpper();
                    db.Equipes.Add(equipe);
                    db.SaveChanges();

                    SiteSessionPersister.IdEquipe = equipe.Id;

                    return RedirectToAction("Inscricoes");
                }
            }

            return View(equipe);
        }

        [Route("Inscricoes")]
        [SiteAuthorize]
        public ActionResult Inscricoes()
        {
            var equipe = db.Equipes.Find(SiteSessionPersister.IdEquipe);

            if (equipe == null)
            {
                return HttpNotFound();
            }

            ViewBag.Faixas = db.Faixas.OrderBy(f => f.Ordem).ToList();

            var viewModelInscricoes = new ViewModelInscricoes
            {
                Equipe = equipe,
                Inscricoes = db.Inscricoes.Where(i => i.AcademiaId == SiteSessionPersister.IdEquipe).ToList()
            };

            return View(viewModelInscricoes);
        }

        public ActionResult BuscaCategoriaIdadeLuta(long modalidadeId, string dataNascimento)
        {
            var idade = DateTime.Now.Year - Convert.ToDateTime(dataNascimento).Year;

            var categoriaIdades = db.CategoriaIdades.Where(c => c.IdadeInicial <= idade && c.IdadeFinal >= idade && c.ModalidadeId == modalidadeId).OrderBy(c => c.IdadeInicial).ToList();

            return Json(categoriaIdades);
        }

        public ActionResult BuscaCategoriaIdadePoomsae(string dataNascimento)
        {
            var idade = DateTime.Now.Year - Convert.ToDateTime(dataNascimento).Year;
            var modalidadeId = 2;

            var categoriaIdades = db.CategoriaIdades.Where(c => c.IdadeInicial <= idade && c.IdadeFinal >= idade && c.ModalidadeId == modalidadeId).OrderBy(c => c.IdadeInicial).ToList();

            return Json(categoriaIdades);
        }

        public ActionResult BuscaCategoriaFaixaLuta(long ordem, string nascimento)
        {
            DateTime dt;

            var idade = 0;
            if (DateTime.TryParse(nascimento, out dt))
            {
                idade = DateTime.Today.Year - dt.Year;
            }

            var categoriaFaixas = db.CategoriaFaixas.Where(c => c.FaixaInicial.Ordem <= ordem && c.FaixaFinal.Ordem >= ordem && (c.ModalidadeId == 1 || c.ModalidadeId == 3)).OrderBy(c => c.FaixaInicial.Ordem).ToList();
            if (idade > 14 && ordem < 10)
            {
                categoriaFaixas = categoriaFaixas.Where(f => f.ModalidadeId == 1).ToList();
            }

            return Json(categoriaFaixas);
        }

        public ActionResult BuscaCategoriaFaixaPoomsae(long ordem)
        {
            var categoriaFaixas = db.CategoriaFaixas.Where(c => c.FaixaInicial.Ordem <= ordem && c.FaixaFinal.Ordem >= ordem && c.ModalidadeId == 2).OrderBy(c => c.FaixaInicial.Ordem).ToList();

            return Json(categoriaFaixas);
        }

        public ActionResult BuscaCategoriaPesos(long categoriaIdadeId, string sexo)
        {
            var categoriaPesos = db.CategoriaLutaPeso.Where(c => c.CategoriaIdadeId == categoriaIdadeId && c.Sexo == sexo).OrderBy(c => c.PesoInicial).ToList();

            return Json(categoriaPesos);
        }

        public ActionResult InserirAtleta(int eventoId, int equipeId, string nome, string nascimento, string sexo, int faixaId, bool luta, bool poomsae, bool paratkd,
                                          int catFaixaLuta, int catIdadeLuta, int catPesoLuta,
                                          int catFaixaPoomsae, int catIdadePoomsae)
        {
            var atleta = new Atleta()
            {
                Ativo = true,
                Nome = nome,
                DataNascimento = Convert.ToDateTime(nascimento),
                DataRegistro = DateTime.Now,
                EquipeId = equipeId,
                FaixaId = faixaId,
                Sexo = sexo,
                Parataekwondo = paratkd
            };

            var inscricao = new Inscricao()
            {
                AcademiaId = equipeId,
                Filiado = atleta,
                DataInscricao = DateTime.Now,
                EventoId = eventoId
            };

            db.Atletas.Add(atleta);
            db.Inscricoes.Add(inscricao);

            if (luta)
            {
                var modalidadeIdLuta = db.CategoriaIdades.Find(catIdadeLuta).ModalidadeId;

                var inscricaoModalidade = new InscricaoModalidade()
                {
                    CategoriaFaixaId = catFaixaLuta,
                    CategoriaIdadeId = catIdadeLuta,
                    CategoriaLutaPesoId = catPesoLuta,
                    EventoId = eventoId,
                    Inscricao = inscricao,
                    ModalidadeId = modalidadeIdLuta
                };
                db.InscricoesModalidades.Add(inscricaoModalidade);
            }

            if (poomsae)
            {
                var modalidadeIdPoomsae = db.CategoriaIdades.Find(catIdadePoomsae).ModalidadeId;

                var inscricaoModalidade = new InscricaoModalidade()
                {
                    CategoriaFaixaId = catFaixaPoomsae,
                    CategoriaIdadeId = catIdadePoomsae,
                    EventoId = eventoId,
                    Inscricao = inscricao,
                    ModalidadeId = modalidadeIdPoomsae
                };
                db.InscricoesModalidades.Add(inscricaoModalidade);
            }

            db.SaveChanges();

            var result = new
            {
                inscricao.Id
            };

            return Json(result);
        }

        public ActionResult DeleteInscricao(long id)
        {
            var inscricoes = db.Inscricoes.Find(id);

            var inscricoesModalidades = inscricoes.InscricaoModalidade.Where(im => im.InscricaoId == inscricoes.Id).ToList();

            for (var i = 0; i < inscricoesModalidades.Count(); i++)
            {
                db.InscricoesModalidades.Remove(inscricoesModalidades[i]);
            }

            db.Inscricoes.Remove(inscricoes);

            db.SaveChanges();

            var result = new
            {
                id
            };

            return Json(result);
        }
    }
}