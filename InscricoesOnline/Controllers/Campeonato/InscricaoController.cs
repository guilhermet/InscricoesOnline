using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using InscricoesOnline.Models;
using InscricoesOnline.Security;
using InscricoesOnline.ViewModel;

namespace InscricoesOnline.Controllers.Admin.Campeonato
{
    [AdminAuthorize]
    public class InscricaoController : Controller
    {
        private IOContext db = new IOContext();

        [Route("Admin/Inscricao/Lista")]
        public ActionResult Lista()
        {
            return View(db.Inscricoes.Where(i => i.Evento.Id == AdminSessionPersister.Evento.Id).OrderBy(i => i.Academia.Nome).ThenBy(i => i.Filiado.Nome).ToList());
        }

        [Route("Admin/Inscricao/Visualizar/{id}")]
        public ActionResult Visualizar(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var inscricao = db.Inscricoes.Where(i => i.Id == id).FirstOrDefault();
            if (inscricao == null)
            {
                return HttpNotFound();
            }

            var inscricoes = db.InscricoesModalidades.Include(i => i.CategoriaLutaPeso).Where(i => i.InscricaoId == id);
            var inscricaoModalidade = new InscricaoViewModel
            {
                Equipe = inscricao.Academia,
                Atleta = inscricao.Filiado,
                InscricoesModalidade = inscricoes.ToList()
            };

            return View(inscricaoModalidade);
        }

        [Route("Admin/Inscricao/Novo")]
        public ActionResult Novo()
        {
            var evento = db.Eventos.Where(e => e.Id == AdminSessionPersister.Evento.Id).FirstOrDefault();
            if (evento == null)
            {
                return HttpNotFound();
            }

            ViewBag.Academias = db.Equipes.OrderBy(a => a.Nome).ToList();

            var inscricaoViewModel = new InscricaoViewModel()
            {
                Equipe = new Equipe(),
            };

            return View(inscricaoViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Inscricao/NovoSalvar")]
        public ActionResult NovoSalvar(InscricaoViewModel inscricaoViewModel, FormCollection formCollection)
        {
            if (String.IsNullOrEmpty(formCollection["txtIdInscricaoModalidade"]))
            {
                var incricoesModalidadesExcluidas = db.InscricoesModalidades.Where(i => i.Inscricao.AcademiaId == inscricaoViewModel.Equipe.Id).ToList();

                incricoesModalidadesExcluidas.ForEach(e => db.Entry(e).State = EntityState.Deleted);

                var inscricoesExcluidas = db.Inscricoes.Where(i => i.AcademiaId == inscricaoViewModel.Equipe.Id).ToList();

                inscricoesExcluidas.ForEach(e => db.Entry(e).State = EntityState.Deleted);
                db.SaveChanges();
            }

            if (!String.IsNullOrEmpty(formCollection["txtIdFiliado"]))
            {
                long id2 = 0;
                var IdsFiliados = formCollection["txtIdFiliado"].Split(',').Select(id => Convert.ToInt64(id)).ToList();
                var IdsInscricaoModalidade = formCollection["txtIdInscricaoModalidade"].Split(',').Select(id => Convert.ToInt64(id)).ToList();
                var IdsModalidades = formCollection["txtIdModalidade"].Split(',').Select(id => Convert.ToInt64(id)).ToList();
                var IdsCategoriasFaixa = formCollection["txtIdCategoriaFaixa"].Split(',').Select(id => (Int64.TryParse(id, out id2) ? Convert.ToInt64(id) : 0)).ToList();
                var IdsCategoriasIdade = formCollection["txtIdCategoriaIdade"].Split(',').Select(id => (Int64.TryParse(id, out id2) ? Convert.ToInt64(id) : 0)).ToList();
                var IdsCategoriasPeso = formCollection["txtIdCategoriaPeso"].Split(',').Select(id => (Int64.TryParse(id, out id2) ? Convert.ToInt64(id) : 0)).ToList();

                var incricoesModalidadesExcluidas = db.InscricoesModalidades.Where(i => i.Inscricao.AcademiaId == inscricaoViewModel.Equipe.Id &&
                                                                                        !IdsInscricaoModalidade.Contains(i.Id)).ToList();

                incricoesModalidadesExcluidas.ForEach(e => db.Entry(e).State = EntityState.Deleted);

                for (var i = 0; i < IdsFiliados.Count; i++)
                {
                    if (IdsInscricaoModalidade[i] == 0)
                    {
                        var idFiliado = IdsFiliados[i];
                        var inscricao = db.Inscricoes.Where(ins => ins.AcademiaId == inscricaoViewModel.Equipe.Id &&
                                                                   ins.FiliadoId == idFiliado).FirstOrDefault();

                        if (inscricao == null)
                        {
                            inscricao = new Inscricao();
                            inscricao.DataInscricao = DateTime.Now;
                            inscricao.AcademiaId = inscricaoViewModel.Equipe.Id;
                            inscricao.EventoId = AdminSessionPersister.Evento.Id;
                            inscricao.FiliadoId = IdsFiliados[i];
                            db.Inscricoes.Add(inscricao);
                            db.SaveChanges();
                        }

                        if (inscricao.InscricaoModalidade == null)
                        {
                            inscricao.InscricaoModalidade = new List<InscricaoModalidade>();
                        }

                        var incricaoModalidade = new InscricaoModalidade();
                        incricaoModalidade.ModalidadeId = IdsModalidades[i];
                        incricaoModalidade.EventoId = AdminSessionPersister.Evento.Id;
                        incricaoModalidade.CategoriaFaixaId = IdsCategoriasFaixa[i] != 0 ? (long?)IdsCategoriasFaixa[i] : null;
                        incricaoModalidade.CategoriaIdadeId = IdsCategoriasIdade[i] != 0 ? (long?)IdsCategoriasIdade[i] : null;
                        incricaoModalidade.CategoriaLutaPesoId = IdsCategoriasPeso[i] != 0 ? (long?)IdsCategoriasPeso[i] : null;

                        inscricao.InscricaoModalidade.Add(incricaoModalidade);
                    }
                }
                db.SaveChanges();
                db.Inscricoes.Where(i => i.AcademiaId == inscricaoViewModel.Equipe.Id &&
                                         i.InscricaoModalidade.Count == 0).ToList().ForEach(i => db.Entry(i).State = EntityState.Deleted);

                db.SaveChanges();
            }
            return RedirectToAction("Lista");
        }

        [Route("Admin/Inscricao/Edit/{id}")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var inscricoesModalidades = db.InscricoesModalidades.Find(id);
            if (inscricoesModalidades == null)
            {
                return HttpNotFound();
            }

            List<CategoriaFaixa> categoriasFaixa = new List<CategoriaFaixa>();
            if (inscricoesModalidades.Modalidade.CategoriaFaixa)
                categoriasFaixa = db.CategoriaFaixas.Where(cf => cf.Ativo &&
                                                                 cf.ModalidadeId == inscricoesModalidades.ModalidadeId).OrderBy(cf => cf.FaixaInicial.Ordem).ToList();

            List<CategoriaIdade> categoriasIdade = new List<CategoriaIdade>();
            List<CategoriaLutaPeso> categoriasPeso = new List<CategoriaLutaPeso>();
            if (inscricoesModalidades.Modalidade.CategoriaIdade)
            {
                categoriasIdade = db.CategoriaIdades.Where(ci => ci.Ativo &&
                                                                 ci.ModalidadeId == inscricoesModalidades.ModalidadeId)
                                                                 .OrderBy(ci => ci.IdadeInicial).ToList();

                if (inscricoesModalidades.Modalidade.CategoriaPeso)
                {
                    if (categoriasIdade.Count > 0)
                    {
                        categoriasPeso = db.CategoriaLutaPeso.Where(cp => cp.Ativo &&
                                                                          cp.CategoriaIdade.Id == inscricoesModalidades.CategoriaIdadeId &&
                                                                          cp.Sexo == inscricoesModalidades.Inscricao.Filiado.Sexo).OrderBy(cp => cp.PesoInicial).ToList();
                    }
                }
            }

            ViewBag.CategoriaFaixaId = new SelectList(categoriasFaixa, "Id", "Titulo", inscricoesModalidades.CategoriaFaixaId);
            ViewBag.CategoriaIdadeId = new SelectList(categoriasIdade, "Id", "Titulo", inscricoesModalidades.CategoriaIdadeId);
            ViewBag.CategoriaLutaPesoId = new SelectList(categoriasPeso, "Id", "Titulo", inscricoesModalidades.CategoriaLutaPesoId);

            return View(inscricoesModalidades);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Inscricao/EditSalvar")]
        public ActionResult EditSalvar(InscricaoModalidade inscricaoModalidade)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inscricaoModalidade).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Visualizar", new { id = inscricaoModalidade.InscricaoId });
            }

            return View("Edit", inscricaoModalidade);
        }

        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InscricaoModalidade inscricaoModalidade = db.InscricoesModalidades.Find(id);

            if (inscricaoModalidade == null)
            {
                return HttpNotFound();
            }

            var idEvento = inscricaoModalidade.Inscricao.EventoId;
            var idAcademia = inscricaoModalidade.Inscricao.AcademiaId;
            var idFiliado = inscricaoModalidade.Inscricao.FiliadoId;

            db.ChaveParticipantes.Where(c => c.InscricaoModalidadeId == inscricaoModalidade.Id).ToList()
                     .ForEach(c => db.Entry(c).State = EntityState.Deleted);

            db.InscricoesModalidades.Remove(inscricaoModalidade);
            db.SaveChanges();

            var inscricao = db.Inscricoes.Where(i => i.AcademiaId == idAcademia &&
                                                      i.EventoId == idEvento &&
                                                      i.FiliadoId == idFiliado).FirstOrDefault();

            if (inscricao != null)
            {
                if (inscricao.InscricaoModalidade.Count() == 0)
                {
                    db.Entry(inscricao).State = EntityState.Deleted;
                    db.SaveChanges();

                    return RedirectToAction("Lista", new { id = idEvento });
                }
                return RedirectToAction("Visualizar", new { id = inscricao.Id });
            }

            return RedirectToAction("Lista", new { id = idEvento });
        }

        [Route("Admin/Inscricao/GerarChaves")]
        public ActionResult GerarChaves()
        {
            var evento = db.Eventos.Where(e => e.Id == AdminSessionPersister.Evento.Id).FirstOrDefault();
            if (evento == null)
            {
                return HttpNotFound();
            }

            var modalidades = db.Modalidades.ToList();

            foreach (var modalidade in modalidades)
            {
                if (modalidade.CategoriaFaixa)
                {
                    foreach (var categoriaFaixa in db.CategoriaFaixas.Where(cf => cf.ModalidadeId == modalidade.Id).OrderBy(cf => cf.FaixaInicial.Ordem))
                    {
                        if (modalidade.CategoriaIdade)
                        {
                            foreach (var categoriaIdade in db.CategoriaIdades.Where(ci => ci.ModalidadeId == modalidade.Id).OrderBy(ci => ci.IdadeInicial))
                            {
                                if (modalidade.CategoriaPeso)
                                {
                                    foreach (var categoriaPeso in db.CategoriaLutaPeso.Where(cp => cp.CategoriaIdadeId == categoriaIdade.Id).OrderBy(cf => cf.PesoInicial))
                                    {
                                        var chave = db.Chaves.Where(c => c.EventoId == evento.Id &&
                                                                         c.ModalidadeId == modalidade.Id &&
                                                                         c.Sexo == categoriaPeso.Sexo &&
                                                                         c.CategoriaFaixaId == categoriaFaixa.Id &&
                                                                         c.CategoriaIdadeId == categoriaIdade.Id &&
                                                                         c.CategoriaLutaPesoId == categoriaPeso.Id).FirstOrDefault();
                                        if (chave == null)
                                        {
                                            chave = new Chave
                                            {
                                                Evento = evento,
                                                Modalidade = modalidade,
                                                Sexo = categoriaPeso.Sexo,
                                                CategoriaFaixa = categoriaFaixa,
                                                CategoriaIdade = categoriaIdade,
                                                CategoriaLutaPeso = categoriaPeso
                                            };
                                            db.Chaves.Add(chave);
                                        }
                                        chave.Inscritos = db.InscricoesModalidades.Where(i => i.Inscricao.EventoId == evento.Id &&
                                                                                              i.ModalidadeId == modalidade.Id &&
                                                                                              i.CategoriaFaixaId == categoriaFaixa.Id &&
                                                                                              i.CategoriaIdadeId == categoriaIdade.Id &&
                                                                                              i.CategoriaLutaPesoId == categoriaPeso.Id).ToList();
                                    }
                                }
                                else
                                {
                                    foreach (var sexo in new[] { "M", "F" })
                                    {
                                        var chave = db.Chaves.Where(c => c.EventoId == evento.Id &&
                                                                         c.ModalidadeId == modalidade.Id &&
                                                                         c.Sexo == sexo &&
                                                                         c.CategoriaFaixaId == categoriaFaixa.Id &&
                                                                         c.CategoriaIdadeId == categoriaIdade.Id &&
                                                                         c.CategoriaLutaPesoId == null).FirstOrDefault();
                                        if (chave == null)
                                        {
                                            chave = new Chave
                                            {
                                                Evento = evento,
                                                Modalidade = modalidade,
                                                Sexo = sexo,
                                                CategoriaFaixa = categoriaFaixa,
                                                CategoriaIdade = categoriaIdade,
                                            };
                                            db.Chaves.Add(chave);
                                        }
                                        chave.Inscritos = db.InscricoesModalidades.Where(i => i.Inscricao.EventoId == evento.Id &&
                                                                                              i.ModalidadeId == modalidade.Id &&
                                                                                              i.CategoriaFaixaId == categoriaFaixa.Id &&
                                                                                              i.CategoriaIdadeId == categoriaIdade.Id &&
                                                                                              i.Inscricao.Filiado.Sexo == sexo).ToList();
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var sexo in new[] { "M", "F" })
                            {
                                var chave = db.Chaves.Where(c => c.EventoId == evento.Id &&
                                                                 c.ModalidadeId == modalidade.Id &&
                                                                 c.Sexo == sexo &&
                                                                 c.CategoriaFaixaId == categoriaFaixa.Id &&
                                                                 c.CategoriaIdadeId == null &&
                                                                 c.CategoriaLutaPesoId == null).FirstOrDefault();
                                if (chave == null)
                                {
                                    chave = new Chave
                                    {
                                        Evento = evento,
                                        Sexo = sexo,
                                        Modalidade = modalidade,
                                        CategoriaFaixa = categoriaFaixa,
                                    };
                                    db.Chaves.Add(chave);
                                }
                                chave.Inscritos = db.InscricoesModalidades.Where(i => i.Inscricao.EventoId == evento.Id &&
                                                                                      i.ModalidadeId == modalidade.Id &&
                                                                                      i.CategoriaFaixaId == categoriaFaixa.Id &&
                                                                                      i.Inscricao.Filiado.Sexo == sexo).ToList();
                            }
                        }
                    }
                }
                else if (modalidade.CategoriaIdade)
                {
                    foreach (var categoriaIdade in db.CategoriaIdades.Where(ci => ci.ModalidadeId == modalidade.Id).OrderBy(ci => ci.IdadeInicial))
                    {
                        if (modalidade.CategoriaPeso)
                        {
                            foreach (var categoriaPeso in db.CategoriaLutaPeso.Where(cp => cp.CategoriaIdadeId == categoriaIdade.Id).OrderBy(cf => cf.PesoInicial))
                            {
                                var chave = db.Chaves.Where(c => c.EventoId == evento.Id &&
                                                                 c.ModalidadeId == modalidade.Id &&
                                                                 c.Sexo == categoriaPeso.Sexo &&
                                                                 c.CategoriaFaixaId == null &&
                                                                 c.CategoriaIdadeId == categoriaIdade.Id &&
                                                                 c.CategoriaLutaPesoId == categoriaPeso.Id).FirstOrDefault();
                                if (chave == null)
                                {
                                    chave = new Chave
                                    {
                                        Evento = evento,
                                        Modalidade = modalidade,
                                        Sexo = categoriaPeso.Sexo,
                                        CategoriaIdade = categoriaIdade,
                                        CategoriaLutaPeso = categoriaPeso,
                                    };
                                    db.Chaves.Add(chave);
                                }
                                chave.Inscritos = db.InscricoesModalidades.Where(i => i.Inscricao.EventoId == evento.Id &&
                                                                                      i.ModalidadeId == modalidade.Id &&
                                                                                      i.CategoriaIdadeId == categoriaIdade.Id &&
                                                                                      i.CategoriaLutaPesoId == categoriaPeso.Id).ToList();
                            }
                        }
                        else
                        {
                            foreach (var sexo in new[] { "M", "F" })
                            {
                                var chave = db.Chaves.Where(c => c.EventoId == evento.Id &&
                                                                 c.ModalidadeId == modalidade.Id &&
                                                                 c.Sexo == sexo &&
                                                                 c.CategoriaFaixaId == null &&
                                                                 c.CategoriaIdadeId == categoriaIdade.Id &&
                                                                 c.CategoriaLutaPesoId == null).FirstOrDefault();
                                if (chave == null)
                                {
                                    chave = new Chave
                                    {
                                        Evento = evento,
                                        Modalidade = modalidade,
                                        Sexo = sexo,
                                        CategoriaIdade = categoriaIdade,
                                    };
                                    db.Chaves.Add(chave);
                                }
                                chave.Inscritos = db.InscricoesModalidades.Where(i => i.Inscricao.EventoId == evento.Id &&
                                                                                      i.ModalidadeId == modalidade.Id &&
                                                                                      i.CategoriaIdadeId == categoriaIdade.Id &&
                                                                                      i.Inscricao.Filiado.Sexo == sexo).ToList();
                            }
                        }
                    }
                }
                else
                {
                    foreach (var sexo in new[] { "M", "F" })
                    {
                        var chave = db.Chaves.Where(c => c.EventoId == evento.Id &&
                                                         c.ModalidadeId == modalidade.Id &&
                                                         c.Sexo == sexo &&
                                                         c.CategoriaFaixaId == null &&
                                                         c.CategoriaIdadeId == null &&
                                                         c.CategoriaLutaPesoId == null).FirstOrDefault();
                        if (chave == null)
                        {
                            chave = new Chave
                            {
                                Evento = evento,
                                Modalidade = modalidade,
                                Sexo = sexo
                            };
                            db.Chaves.Add(chave);
                        }
                        chave.Inscritos = db.InscricoesModalidades.Where(i => i.Inscricao.EventoId == evento.Id &&
                                                                              i.ModalidadeId == modalidade.Id &&
                                                                              i.Inscricao.Filiado.Sexo == sexo).ToList();
                    }
                }
            }

            db.SaveChanges();
            var chaves = db.Chaves.Include(c => c.Inscritos).Include(c => c.ChaveParticipantes)
                                  .Where(c => c.EventoId == AdminSessionPersister.Evento.Id && (c.Inscritos.Count() > 0 || c.ChaveParticipantes.Count() > 0))
                                  .OrderBy(c => c.Modalidade.Titulo)
                                  .ThenBy(c => c.CategoriaFaixa.FaixaInicial.Ordem)
                                  .ThenBy(c => c.CategoriaIdade.IdadeInicial)
                                  .ThenBy(c => c.Sexo)
                                  .ThenBy(c => c.CategoriaLutaPeso.PesoInicial).ToList();

            return View(chaves);
        }

        [Route("Admin/Inscricao/SortearChaves")]
        public ActionResult SortearEvento()
        {
            var chaves = db.Chaves.Include(c => c.Inscritos).Include(c => c.ChaveParticipantes).Where(c => c.EventoId == AdminSessionPersister.Evento.Id).ToList();

            foreach (var chave in chaves)
            {
                chave.Inscritos = chave.Inscritos.OrderBy(i => new Random().Next()).ToList();
                chave.ChaveParticipantes.ToList().ForEach(c => db.Entry(c).State = EntityState.Deleted);

                int j = 1;
                foreach (var participante in chave.Inscritos)
                {
                    chave.ChaveParticipantes.Add(new ChaveParticipante
                    {
                        Chave = chave,
                        Colocacao = 0,
                        Ativo = true,
                        InscricaoModalidade = participante,
                        EventoId = AdminSessionPersister.Evento.Id,
                        Ordem = j++
                    });
                }
            }

            db.SaveChanges();

            var chavesAtualizadas = db.Chaves.Include(c => c.Inscritos).Include(c => c.ChaveParticipantes)
                                                            .Where(c => c.EventoId == AdminSessionPersister.Evento.Id && c.Inscritos.Count() > 0)
                                                            .OrderBy(c => c.Modalidade.Titulo)
                                                            .ThenBy(c => c.CategoriaFaixa.FaixaInicial.Ordem)
                                                            .ThenBy(c => c.CategoriaIdade.IdadeInicial)
                                                            .ThenBy(c => c.Sexo)
                                                            .ThenBy(c => c.CategoriaLutaPeso.PesoInicial).ToList();

            return View("GerarChaves", chavesAtualizadas);
        }

        //[Route("Admin/Inscricao/ImprimirChaves/{id}")]
        //public ActionResult ImprimirEvento(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    var chaves = db.Chaves.Where(c => c.EventoId == id);

        //    foreach (var chave in chaves)
        //    {
        //        var stream = ImprimirChave(chave);
        //    }
        //    //File(stream, "application/pdf", "CustomerList.pdf");

        //    return View();
        //}

        [Route("Admin/Inscricao/SortearChave/{id}")]
        public ActionResult Sortear(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var chave = db.Chaves.Include(c => c.Inscritos).Include(c => c.ChaveParticipantes).Where(c => c.Id == id).FirstOrDefault();
            if (chave == null)
            {
                return HttpNotFound();
            }

            chave.Inscritos = chave.Inscritos.OrderBy(i => new Random().Next()).ToList();
            chave.ChaveParticipantes.ToList().ForEach(c => db.Entry(c).State = EntityState.Deleted);

            int j = 1;
            foreach (var participante in chave.Inscritos)
            {
                chave.ChaveParticipantes.Add(new ChaveParticipante
                {
                    Chave = chave,
                    Colocacao = 0,
                    Ativo = true,
                    InscricaoModalidade = participante,
                    EventoId = AdminSessionPersister.Evento.Id,
                    Ordem = j++
                });
            }

            db.SaveChanges();

            var result = new
            {
                Id = chave.Id,
                TituloModalidade = chave.Modalidade.Titulo,
                TituloCategoriaFaixa = (chave.CategoriaFaixa != null ? chave.CategoriaFaixa.Titulo : ""),
                TituloCategoriaIdade = (chave.CategoriaIdade != null ? chave.CategoriaIdade.Titulo : ""),
                TituloCategoriaPeso = (chave.CategoriaLutaPeso != null ? chave.CategoriaLutaPeso.Titulo : ""),
                Sexo = chave.Sexo,
                QtdeParticipantes = chave.ChaveParticipantes.Count(),
                QtdeInscritos = chave.Inscritos.Count()
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Route("Admin/Inscricao/ImprimirChave/{id}")]
        public ActionResult Imprimir(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var chave = db.Chaves.Find(id);
            if (chave == null)
            {
                return HttpNotFound();
            }

            var stream = ImprimirChave(chave);

            var nomeArquivo = chave.Modalidade.Titulo + "_-_" + chave.Sexo;

            if (chave.CategoriaIdade != null)
                nomeArquivo += "_-_" + chave.CategoriaIdade.Titulo;

            if (chave.CategoriaFaixa != null)
                nomeArquivo += "_-_" + chave.CategoriaFaixa.Titulo;

            if (chave.CategoriaLutaPeso != null)
                nomeArquivo += "_-_" + chave.CategoriaLutaPeso.Titulo;

            nomeArquivo += ".pdf";

            return File(stream, "application/pdf", nomeArquivo);
        }

        private Stream ImprimirChave(Chave chave)
        {
            var chaveOrdenada = chave.ChaveParticipantes.OrderBy(cp => cp.Ordem).ToList();

            var participantes1 = (chaveOrdenada.Count >= 1 ? chaveOrdenada[0].InscricaoModalidade.Inscricao.Filiado.Nome : "");
            var academias1 = (chaveOrdenada.Count >= 1 ? chaveOrdenada[0].InscricaoModalidade.Inscricao.Academia.Nome : "");
            var participantes2 = (chaveOrdenada.Count >= 2 ? chaveOrdenada[1].InscricaoModalidade.Inscricao.Filiado.Nome : "");
            var academias2 = (chaveOrdenada.Count >= 2 ? chaveOrdenada[1].InscricaoModalidade.Inscricao.Academia.Nome : "");
            var participantes3 = (chaveOrdenada.Count >= 3 ? chaveOrdenada[2].InscricaoModalidade.Inscricao.Filiado.Nome : "");
            var academias3 = (chaveOrdenada.Count >= 3 ? chaveOrdenada[2].InscricaoModalidade.Inscricao.Academia.Nome : "");
            var participantes4 = (chaveOrdenada.Count >= 4 ? chaveOrdenada[3].InscricaoModalidade.Inscricao.Filiado.Nome : "");
            var academias4 = (chaveOrdenada.Count >= 4 ? chaveOrdenada[3].InscricaoModalidade.Inscricao.Academia.Nome : "");
            var participantes5 = (chaveOrdenada.Count >= 5 ? chaveOrdenada[4].InscricaoModalidade.Inscricao.Filiado.Nome : "");
            var academias5 = (chaveOrdenada.Count >= 5 ? chaveOrdenada[4].InscricaoModalidade.Inscricao.Academia.Nome : "");
            var participantes6 = (chaveOrdenada.Count >= 6 ? chaveOrdenada[5].InscricaoModalidade.Inscricao.Filiado.Nome : "");
            var academias6 = (chaveOrdenada.Count >= 6 ? chaveOrdenada[5].InscricaoModalidade.Inscricao.Academia.Nome : "");
            var participantes7 = (chaveOrdenada.Count >= 7 ? chaveOrdenada[6].InscricaoModalidade.Inscricao.Filiado.Nome : "");
            var academias7 = (chaveOrdenada.Count >= 7 ? chaveOrdenada[6].InscricaoModalidade.Inscricao.Academia.Nome : "");
            var participantes8 = (chaveOrdenada.Count >= 8 ? chaveOrdenada[7].InscricaoModalidade.Inscricao.Filiado.Nome : "");
            var academias8 = (chaveOrdenada.Count >= 8 ? chaveOrdenada[7].InscricaoModalidade.Inscricao.Academia.Nome : "");
            var participantes9 = (chaveOrdenada.Count >= 9 ? chaveOrdenada[8].InscricaoModalidade.Inscricao.Filiado.Nome : "");
            var academias9 = (chaveOrdenada.Count >= 9 ? chaveOrdenada[8].InscricaoModalidade.Inscricao.Academia.Nome : "");
            var participantes10 = (chaveOrdenada.Count >= 10 ? chaveOrdenada[9].InscricaoModalidade.Inscricao.Filiado.Nome : "");
            var academias10 = (chaveOrdenada.Count >= 10 ? chaveOrdenada[9].InscricaoModalidade.Inscricao.Academia.Nome : "");
            var participantes11 = (chaveOrdenada.Count >= 11 ? chaveOrdenada[10].InscricaoModalidade.Inscricao.Filiado.Nome : "");
            var academias11 = (chaveOrdenada.Count >= 11 ? chaveOrdenada[10].InscricaoModalidade.Inscricao.Academia.Nome : "");
            var participantes12 = (chaveOrdenada.Count >= 12 ? chaveOrdenada[11].InscricaoModalidade.Inscricao.Filiado.Nome : "");
            var academias12 = (chaveOrdenada.Count >= 12 ? chaveOrdenada[11].InscricaoModalidade.Inscricao.Academia.Nome : "");
            var participantes13 = (chaveOrdenada.Count >= 13 ? chaveOrdenada[12].InscricaoModalidade.Inscricao.Filiado.Nome : "");
            var academias13 = (chaveOrdenada.Count >= 13 ? chaveOrdenada[12].InscricaoModalidade.Inscricao.Academia.Nome : "");
            var participantes14 = (chaveOrdenada.Count >= 14 ? chaveOrdenada[13].InscricaoModalidade.Inscricao.Filiado.Nome : "");
            var academias14 = (chaveOrdenada.Count >= 14 ? chaveOrdenada[13].InscricaoModalidade.Inscricao.Academia.Nome : "");
            var participantes15 = (chaveOrdenada.Count >= 15 ? chaveOrdenada[14].InscricaoModalidade.Inscricao.Filiado.Nome : "");
            var academias15 = (chaveOrdenada.Count >= 15 ? chaveOrdenada[14].InscricaoModalidade.Inscricao.Academia.Nome : "");
            var participantes16 = (chaveOrdenada.Count >= 16 ? chaveOrdenada[15].InscricaoModalidade.Inscricao.Filiado.Nome : "");
            var academias16 = (chaveOrdenada.Count >= 16 ? chaveOrdenada[15].InscricaoModalidade.Inscricao.Academia.Nome : "");
            var participantes17 = (chaveOrdenada.Count >= 17 ? chaveOrdenada[16].InscricaoModalidade.Inscricao.Filiado.Nome : "");
            var academias17 = (chaveOrdenada.Count >= 17 ? chaveOrdenada[16].InscricaoModalidade.Inscricao.Academia.Nome : "");
            var participantes18 = (chaveOrdenada.Count >= 18 ? chaveOrdenada[17].InscricaoModalidade.Inscricao.Filiado.Nome : "");
            var academias18 = (chaveOrdenada.Count >= 18 ? chaveOrdenada[17].InscricaoModalidade.Inscricao.Academia.Nome : "");
            var participantes19 = (chaveOrdenada.Count >= 19 ? chaveOrdenada[18].InscricaoModalidade.Inscricao.Filiado.Nome : "");
            var academias19 = (chaveOrdenada.Count >= 19 ? chaveOrdenada[18].InscricaoModalidade.Inscricao.Academia.Nome : "");
            var participantes20 = (chaveOrdenada.Count >= 20 ? chaveOrdenada[19].InscricaoModalidade.Inscricao.Filiado.Nome : "");
            var academias20 = (chaveOrdenada.Count >= 20 ? chaveOrdenada[19].InscricaoModalidade.Inscricao.Academia.Nome : "");
            var participantes21 = (chaveOrdenada.Count >= 21 ? chaveOrdenada[20].InscricaoModalidade.Inscricao.Filiado.Nome : "");
            var academias21 = (chaveOrdenada.Count >= 21 ? chaveOrdenada[20].InscricaoModalidade.Inscricao.Academia.Nome : "");
            var participantes22 = (chaveOrdenada.Count >= 22 ? chaveOrdenada[21].InscricaoModalidade.Inscricao.Filiado.Nome : "");
            var academias22 = (chaveOrdenada.Count >= 22 ? chaveOrdenada[21].InscricaoModalidade.Inscricao.Academia.Nome : "");
            var participantes23 = (chaveOrdenada.Count >= 23 ? chaveOrdenada[22].InscricaoModalidade.Inscricao.Filiado.Nome : "");
            var academias23 = (chaveOrdenada.Count >= 23 ? chaveOrdenada[22].InscricaoModalidade.Inscricao.Academia.Nome : "");
            var participantes24 = (chaveOrdenada.Count >= 24 ? chaveOrdenada[23].InscricaoModalidade.Inscricao.Filiado.Nome : "");
            var academias24 = (chaveOrdenada.Count >= 24 ? chaveOrdenada[23].InscricaoModalidade.Inscricao.Academia.Nome : "");
            var participantes25 = (chaveOrdenada.Count >= 25 ? chaveOrdenada[24].InscricaoModalidade.Inscricao.Filiado.Nome : "");
            var academias25 = (chaveOrdenada.Count >= 25 ? chaveOrdenada[24].InscricaoModalidade.Inscricao.Academia.Nome : "");
            var participantes26 = (chaveOrdenada.Count >= 26 ? chaveOrdenada[25].InscricaoModalidade.Inscricao.Filiado.Nome : "");
            var academias26 = (chaveOrdenada.Count >= 26 ? chaveOrdenada[25].InscricaoModalidade.Inscricao.Academia.Nome : "");
            var participantes27 = (chaveOrdenada.Count >= 27 ? chaveOrdenada[26].InscricaoModalidade.Inscricao.Filiado.Nome : "");
            var academias27 = (chaveOrdenada.Count >= 27 ? chaveOrdenada[26].InscricaoModalidade.Inscricao.Academia.Nome : "");
            var participantes28 = (chaveOrdenada.Count >= 28 ? chaveOrdenada[27].InscricaoModalidade.Inscricao.Filiado.Nome : "");
            var academias28 = (chaveOrdenada.Count >= 28 ? chaveOrdenada[27].InscricaoModalidade.Inscricao.Academia.Nome : "");
            var participantes29 = (chaveOrdenada.Count >= 29 ? chaveOrdenada[28].InscricaoModalidade.Inscricao.Filiado.Nome : "");
            var academias29 = (chaveOrdenada.Count >= 29 ? chaveOrdenada[28].InscricaoModalidade.Inscricao.Academia.Nome : "");
            var participantes30 = (chaveOrdenada.Count >= 30 ? chaveOrdenada[29].InscricaoModalidade.Inscricao.Filiado.Nome : "");
            var academias30 = (chaveOrdenada.Count >= 30 ? chaveOrdenada[29].InscricaoModalidade.Inscricao.Academia.Nome : "");
            var participantes31 = (chaveOrdenada.Count >= 31 ? chaveOrdenada[30].InscricaoModalidade.Inscricao.Filiado.Nome : "");
            var academias31 = (chaveOrdenada.Count >= 31 ? chaveOrdenada[30].InscricaoModalidade.Inscricao.Academia.Nome : "");
            var participantes32 = (chaveOrdenada.Count >= 32 ? chaveOrdenada[31].InscricaoModalidade.Inscricao.Filiado.Nome : "");
            var academias32 = (chaveOrdenada.Count >= 32 ? chaveOrdenada[31].InscricaoModalidade.Inscricao.Academia.Nome : "");

            var categoriaTitulo = "";
            var masculino = "MASCULINO";
            var feminino = "FEMININO";
            var chaves = db.Chaves.Where(p => p.Id == chave.Id)
                               .Select(p => new
                               {
                                   IdEvento = p.Evento.Id,
                                   TituloEvento = p.Evento.Titulo.ToUpper(),
                                   LocalEvento = p.Evento.Local.ToUpper(),
                                   IdModalidade = p.Modalidade.Id,
                                   TituloModalidade = p.Modalidade.Titulo.ToUpper(),
                                   Sexo = (p.Sexo == "M" ? masculino : feminino),
                                   IdCategoriaFaixa = (p.CategoriaFaixa == null ? 0 : p.CategoriaFaixa.Id),
                                   TitutloCategoriaFaixa = (p.CategoriaFaixa == null ? categoriaTitulo : p.CategoriaFaixa.Titulo.ToUpper()),
                                   IdCategoriaIdade = (p.CategoriaIdade == null ? 0 : p.CategoriaIdade.Id),
                                   TituloCategoriaIdade = (p.CategoriaIdade == null ? categoriaTitulo : p.CategoriaIdade.Titulo.ToUpper()),
                                   IdCategoriaPeso = (p.CategoriaLutaPeso == null ? 0 : p.CategoriaLutaPeso.Id),
                                   TituloCategoriaPeso = (p.CategoriaLutaPeso == null ? categoriaTitulo : p.CategoriaLutaPeso.Titulo.ToUpper()),
                                   NomeParticipante1 = participantes1.ToUpper(),
                                   AcademiaParticipante1 = academias1.ToUpper(),
                                   NomeParticipante2 = participantes2.ToUpper(),
                                   AcademiaParticipante2 = academias2.ToUpper(),
                                   NomeParticipante3 = participantes3.ToUpper(),
                                   AcademiaParticipante3 = academias3.ToUpper(),
                                   NomeParticipante4 = participantes4.ToUpper(),
                                   AcademiaParticipante4 = academias4.ToUpper(),
                                   NomeParticipante5 = participantes5.ToUpper(),
                                   AcademiaParticipante5 = academias5.ToUpper(),
                                   NomeParticipante6 = participantes6.ToUpper(),
                                   AcademiaParticipante6 = academias6.ToUpper(),
                                   NomeParticipante7 = participantes7.ToUpper(),
                                   AcademiaParticipante7 = academias7.ToUpper(),
                                   NomeParticipante8 = participantes8.ToUpper(),
                                   AcademiaParticipante8 = academias8.ToUpper(),
                                   NomeParticipante9 = participantes9.ToUpper(),
                                   AcademiaParticipante9 = academias9.ToUpper(),
                                   NomeParticipante10 = participantes10.ToUpper(),
                                   AcademiaParticipante10 = academias10.ToUpper(),
                                   NomeParticipante11 = participantes11.ToUpper(),
                                   AcademiaParticipante11 = academias11.ToUpper(),
                                   NomeParticipante12 = participantes12.ToUpper(),
                                   AcademiaParticipante12 = academias12.ToUpper(),
                                   NomeParticipante13 = participantes13.ToUpper(),
                                   AcademiaParticipante13 = academias13.ToUpper(),
                                   NomeParticipante14 = participantes14.ToUpper(),
                                   AcademiaParticipante14 = academias14.ToUpper(),
                                   NomeParticipante15 = participantes15.ToUpper(),
                                   AcademiaParticipante15 = academias15.ToUpper(),
                                   NomeParticipante16 = participantes16.ToUpper(),
                                   AcademiaParticipante16 = academias16.ToUpper(),
                                   NomeParticipante17 = participantes17.ToUpper(),
                                   AcademiaParticipante17 = academias17.ToUpper(),
                                   NomeParticipante18 = participantes18.ToUpper(),
                                   AcademiaParticipante18 = academias18.ToUpper(),
                                   NomeParticipante19 = participantes19.ToUpper(),
                                   AcademiaParticipante19 = academias19.ToUpper(),
                                   NomeParticipante20 = participantes20.ToUpper(),
                                   AcademiaParticipante20 = academias20.ToUpper(),
                                   NomeParticipante21 = participantes21.ToUpper(),
                                   AcademiaParticipante21 = academias21.ToUpper(),
                                   NomeParticipante22 = participantes22.ToUpper(),
                                   AcademiaParticipante22 = academias22.ToUpper(),
                                   NomeParticipante23 = participantes23.ToUpper(),
                                   AcademiaParticipante23 = academias23.ToUpper(),
                                   NomeParticipante24 = participantes24.ToUpper(),
                                   AcademiaParticipante24 = academias24.ToUpper(),
                                   NomeParticipante25 = participantes25.ToUpper(),
                                   AcademiaParticipante25 = academias25.ToUpper(),
                                   NomeParticipante26 = participantes26.ToUpper(),
                                   AcademiaParticipante26 = academias26.ToUpper(),
                                   NomeParticipante27 = participantes27.ToUpper(),
                                   AcademiaParticipante27 = academias27.ToUpper(),
                                   NomeParticipante28 = participantes28.ToUpper(),
                                   AcademiaParticipante28 = academias28.ToUpper(),
                                   NomeParticipante29 = participantes29.ToUpper(),
                                   AcademiaParticipante29 = academias29.ToUpper(),
                                   NomeParticipante30 = participantes30.ToUpper(),
                                   AcademiaParticipante30 = academias30.ToUpper(),
                                   NomeParticipante31 = participantes31.ToUpper(),
                                   AcademiaParticipante31 = academias31.ToUpper(),
                                   NomeParticipante32 = participantes32.ToUpper(),
                                   AcademiaParticipante32 = academias32.ToUpper(),
                               })
                               .ToList();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("/Reports"), "chaves0" + chaveOrdenada.Count.ToString("00") + ".rpt"));

            ((TextObject)rd.ReportDefinition.ReportObjects["txtNomeEmpresa"]).Text = db.Equipes.FirstOrDefault()?.Nome.ToUpper();
            rd.SetDataSource(chaves);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

            rd.Close();
            rd.Dispose();

            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        [Route("Admin/Inscricao/GerarChaves/Inscritos/{id}")]
        public ActionResult Inscritos(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var chave = db.Chaves.Find(id);
            if (chave == null)
            {
                return HttpNotFound();
            }

            return View(chave);
        }

        [Route("Admin/Inscricao/GerarChaves/Inscritos/{id}/Reordenar")]
        public ActionResult Reordenar(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var chave = db.Chaves.Find(id);
            if (chave == null || chave.ChaveParticipantes.Count() == 0)
            {
                return HttpNotFound();
            }

            return View(chave.ChaveParticipantes.OrderBy(c => c.Ordem).ToList());
        }

        [Route("Admin/Inscricao/GerarChaves/Inscritos/{id}/ReordenarSalvar")]
        public ActionResult ReordenarSalvar(List<ChaveParticipante> chaveParticipantes)
        {
            foreach (var chaveParticipante in chaveParticipantes)
            {
                db.Entry(chaveParticipante).State = EntityState.Modified;
            }
            db.SaveChanges();

            var chave = db.Chaves.Find(chaveParticipantes.First().ChaveId);

            return RedirectToAction("GerarChaves", new { id = chave?.EventoId });
        }

        [Route("Admin/Inscricao/Controle")]
        public ActionResult Controle()
        {
            var chave = db.Chaves.Where(c => c.EventoId == AdminSessionPersister.Evento.Id);
            if (chave == null)
            {
                return HttpNotFound();
            }

            return View(chave.Where(c => c.ChaveParticipantes.Count > 0)
                             .OrderBy(c => c.Modalidade.Titulo)
                             .ThenBy(c => c.CategoriaIdade.IdadeInicial)
                             .ThenBy(c => c.CategoriaFaixa.FaixaInicial.Ordem)
                             .ThenBy(c => c.CategoriaLutaPeso.Sexo)
                             .ThenBy(c => c.CategoriaLutaPeso.PesoInicial).ToList());
        }

        [Route("Admin/Inscricao/Desclassificar/{id}")]
        public ActionResult Desclassificar(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var chaveParticipantes = db.ChaveParticipantes.Where(c => c.Chave.Id == id);
            if (chaveParticipantes == null)
            {
                return HttpNotFound();
            }

            return View(chaveParticipantes.OrderBy(c => c.InscricaoModalidade.Modalidade.Titulo)
                                          .ThenBy(c => c.InscricaoModalidade.CategoriaFaixa.FaixaInicial.Ordem)
                                          .ThenBy(c => c.InscricaoModalidade.CategoriaIdade.IdadeInicial)
                                          .ThenBy(c => c.InscricaoModalidade.Inscricao.Filiado.Sexo)
                                          .ThenBy(c => c.InscricaoModalidade.CategoriaLutaPeso.PesoInicial).ToList());
        }

        [ValidateAntiForgeryToken]
        [Route("Admin/Inscricao/DesclassificarSalvar")]
        public ActionResult DesclassificarSalvar(List<ChaveParticipante> chaveParticipantes)
        {
            foreach (var chaveParticipante in chaveParticipantes)
            {
                chaveParticipante.Colocacao = 99;
                db.Entry(chaveParticipante).State = EntityState.Modified;
            }
            db.SaveChanges();

            return RedirectToAction("Eventos");
        }

        [Route("Admin/Inscricao/GerarChaves/Inscritos/{id}/Classificar")]
        public ActionResult Classificar(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var chave = db.Chaves.Find(id);
            if (chave == null || chave.ChaveParticipantes.Count() == 0)
            {
                return HttpNotFound();
            }

            var chavesParticipantes = chave.ChaveParticipantes.Where(c => c.Ativo).OrderBy(c => c.Colocacao).ToList();

            if (chavesParticipantes.Where(c => c.Colocacao == 0).Any())
            {
                var i = 1;
                chavesParticipantes.ForEach(c => c.Colocacao = i++);
            }

            return View(chavesParticipantes);
        }

        [Route("Admin/Inscricao/GerarChaves/Inscritos/{id}/ClassificarSalvar")]
        public ActionResult ClassificarSalvar(List<ChaveParticipante> chaveParticipantes)
        {
            foreach (var chaveParticipante in chaveParticipantes)
            {
                db.Entry(chaveParticipante).State = EntityState.Modified;
            }
            db.SaveChanges();

            var chave = db.Chaves.Find(chaveParticipantes.First().ChaveId);

            return RedirectToAction("Controle", new { id = chave?.EventoId });
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
