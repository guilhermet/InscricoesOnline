using InscricoesOnline.Models;
using InscricoesOnline.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InscricoesOnline.Controllers
{
    [AdminAuthorize]
    public class AdminController : Controller
    {
        private IOContext db = new IOContext();

        [Route("Admin")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("Admin/BuscaFiliado/{idAcademia}/{id}")]
        public ActionResult BuscaFiliado(long idAcademia, long id)
        {
            var filiado = db.Atletas.Where(f => f.Id == id && f.EquipeId == idAcademia).FirstOrDefault();

            object result;

            if (filiado == null)
            {
                result = new
                {
                    Id = 0
                };
            }
            else
            {
                var modalidades = (from me in db.Modalidades select me).ToList();

                List<object> modalidadesFiliado = new List<object>();
                foreach (var modalidade in modalidades)
                {
                    List<CategoriaFaixa> categoriasFaixa = new List<CategoriaFaixa>();
                    if (modalidade.CategoriaFaixa)
                        categoriasFaixa = db.CategoriaFaixas.Where(cf => cf.Ativo &&
                                                                         cf.ModalidadeId == modalidade.Id).OrderBy(cf => cf.FaixaInicial.Ordem).ToList();

                    List<CategoriaIdade> categoriasIdade = new List<CategoriaIdade>();
                    List<CategoriaLutaPeso> categoriasPeso = new List<CategoriaLutaPeso>();
                    if (modalidade.CategoriaIdade)
                    {
                        categoriasIdade = db.CategoriaIdades.Where(ci => ci.Ativo &&
                                                                         ci.ModalidadeId == modalidade.Id)
                                                                         .OrderBy(ci => ci.IdadeInicial).ToList();

                        if (modalidade.CategoriaPeso)
                        {
                            if (categoriasIdade.Count > 0)
                            {
                                var categoriaIdade = categoriasIdade.First();
                                categoriasPeso = db.CategoriaLutaPeso.Where(cp => cp.Ativo &&
                                                                                  cp.CategoriaIdade.Id == categoriaIdade.Id &&
                                                                                  cp.Sexo == filiado.Sexo).OrderBy(cp => cp.PesoInicial).ToList();
                            }
                        }
                    }

                    if (!modalidade.CategoriaFaixa || (modalidade.CategoriaFaixa && categoriasFaixa.Count > 0) &&
                        !modalidade.CategoriaIdade || (modalidade.CategoriaIdade && categoriasIdade.Count > 0) &&
                        !modalidade.CategoriaPeso || (modalidade.CategoriaPeso && categoriasPeso.Count > 0))
                    {
                        modalidadesFiliado.Add(new
                        {
                            ModalidadeId = modalidade.Id,
                            ModalidadeTitulo = modalidade.Titulo,
                            UtilizarCategoriaFaixa = modalidade.CategoriaFaixa,
                            CategoriasFaixas = categoriasFaixa.ToList(),
                            UtilizarCategoriaIdade = modalidade.CategoriaIdade,
                            CategoriasIdades = categoriasIdade.ToList(),
                            UtilizarCategoriaPeso = modalidade.CategoriaPeso,
                            CategoriasPesos = categoriasPeso.ToList()
                        });
                    }
                }

                result = new
                {
                    Id = filiado.Id,
                    Nome = filiado.Nome,
                    Sexo = filiado.Sexo,
                    Faixa = filiado.Faixa.Descricao,
                    Modalidades = modalidadesFiliado
                };
            }
            return Json(result);
        }

        [Route("Admin/BuscaPesos/{idCategoriaIdade}/{sexo}")]
        public ActionResult BuscaPesos(long idCategoriaIdade, string sexo)
        {
            var categoriaPesos = db.CategoriaLutaPeso.Where(c => c.CategoriaIdadeId == idCategoriaIdade && c.Sexo == sexo).OrderBy(c => c.PesoInicial).ToList();

            return Json(categoriaPesos);
        }

        [Route("Admin/BuscaInscricoes/{idAcademia}")]
        public ActionResult BuscaInscricoes(long idAcademia)
        {
            var inscricoes = db.InscricoesModalidades.Where(i => i.Inscricao.AcademiaId == idAcademia).ToList();

            var list = new List<object>();

            foreach (var inscricaoModalidade in inscricoes)
            {
                list.Add(new
                {
                    IdInscricaoModalidade = inscricaoModalidade.Id,
                    IdFiliado = inscricaoModalidade.Inscricao.FiliadoId,
                    NomeFiliado = inscricaoModalidade.Inscricao.Filiado.Nome,
                    Faixa = inscricaoModalidade.Inscricao.Filiado.Faixa.Descricao,
                    Modalidade = inscricaoModalidade.Modalidade.Titulo,
                    ModalidadeId = inscricaoModalidade.ModalidadeId,
                    CategoriaFaixa = (inscricaoModalidade.CategoriaFaixa != null ? inscricaoModalidade.CategoriaFaixa.Titulo : ""),
                    CategoriaFaixaId = inscricaoModalidade.CategoriaFaixaId,
                    CategoriaIdade = (inscricaoModalidade.CategoriaIdade != null ? inscricaoModalidade.CategoriaIdade.Titulo : ""),
                    CategoriaIdadeId = inscricaoModalidade.CategoriaIdadeId,
                    CategoriaPeso = (inscricaoModalidade.CategoriaLutaPeso != null ? inscricaoModalidade.CategoriaLutaPeso.Titulo : ""),
                    CategoriaPesoId = inscricaoModalidade.CategoriaLutaPesoId
                });
            }

            object result = new
            {
                inscricoes = list
            };

            return Json(result);
        }
    }
}