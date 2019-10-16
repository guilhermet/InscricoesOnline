using CrystalDecisions.CrystalReports.Engine;
using InscricoesOnline.Models;
using InscricoesOnline.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FPTKD.Controllers.Admin
{
    [AdminAuthorize]
    public class RelatoriosController : Controller
    {
        private IOContext db = new IOContext();

        [Route("Admin/Relatorios/Campeonato")]
        public ActionResult Campeonato()
        {
            return View();
        }

        [Route("Admin/Relatorios/Campeonato/Inscricoes")]
        public ActionResult Inscricoes()
        {
            ViewBag.ModalidadesId = db.Modalidades.OrderBy(m => m.Titulo);
            ViewBag.AcademiaId = db.Equipes.OrderBy(a => a.Nome);

            return View();
        }

        [Route("Admin/Relatorios/Campeonato/InscricoesDados")]
        public ActionResult InscricoesDados(string tipo, string arquivo, string modalidades, string academias, string eventos, string sexo)
        {
            var modalidadesFilter = (!String.IsNullOrEmpty(modalidades) ? modalidades.Split(',').Select(m => Convert.ToInt64(m)) : new List<Int64>());
            var academiasFilter = (!String.IsNullOrEmpty(academias) ? academias.Split(',').Select(m => Convert.ToInt64(m)) : new List<Int64>());
            var eventosFilter = (!String.IsNullOrEmpty(eventos) ? eventos.Split(',').Select(m => Convert.ToInt64(m)) : new List<Int64>());
            var sexoFilter = (!String.IsNullOrEmpty(sexo) ? sexo.Split(',').Select(m => Convert.ToString(m)) : new List<String>());
            var inscricoes = db.InscricoesModalidades.Where(i => (modalidadesFilter.Count() > 0 ? modalidadesFilter.Contains(i.ModalidadeId) : true) &&
                                                                 (academiasFilter.Count() > 0 ? academiasFilter.Contains(i.Inscricao.AcademiaId) : true) &&
                                                                 (eventosFilter.Count() > 0 ? eventosFilter.Contains(i.Inscricao.EventoId) : true) &&
                                                                 (sexoFilter.Count() > 0 ? sexoFilter.Contains(i.Inscricao.Filiado.Sexo) : true));

            var categoriaLutaPesoTitulo = "-";
            var masculino = "Masculino";
            var feminino = "Feminino";
            var inscricoesFiltrado = inscricoes.OrderBy(i => i.Inscricao.Evento.DataInicio).ThenBy(i => i.Inscricao.Filiado.Nome).Select(i => new
            {
                FiliadoId = i.Inscricao.FiliadoId,
                FiliadoNome = i.Inscricao.Filiado.Nome,
                DataNascimento = i.Inscricao.Filiado.DataNascimento.Value,
                Faixa = i.Inscricao.Filiado.Faixa.Descricao,
                AcademiaId = i.Inscricao.AcademiaId,
                AcademiaNome = i.Inscricao.Academia.Nome,
                CategoriaFaixaId = (i.CategoriaFaixa == null ? 0 : i.CategoriaFaixa.Id),
                CategoriaFaixaTitulo = (i.CategoriaFaixa == null ? categoriaLutaPesoTitulo : i.CategoriaFaixa.Titulo),
                CategoriaFaixaOrdem = (i.CategoriaFaixa == null ? 0 : i.CategoriaFaixa.FaixaInicial.Ordem),
                CategoriaIdadeId = (i.CategoriaIdade == null ? 0 : i.CategoriaIdade.Id),
                CategoriaIdadeTitulo = (i.CategoriaIdade == null ? categoriaLutaPesoTitulo : i.CategoriaIdade.Titulo),
                CategoriaIdadeOrdem = (i.CategoriaIdade == null ? 0 : i.CategoriaIdade.IdadeInicial),
                CategoriaPesoId = (i.CategoriaLutaPeso == null ? 0 : i.CategoriaLutaPeso.Id),
                CategoriaPesoTitulo = (i.CategoriaLutaPeso == null ? categoriaLutaPesoTitulo : i.CategoriaLutaPeso.Titulo),
                CategoriaPesoOrdem = (i.CategoriaLutaPeso == null ? 0 : i.CategoriaLutaPeso.PesoInicial),
                IdModadelidade = i.ModalidadeId,
                TituloModalidade = i.Modalidade.Titulo,
                EventoId = i.Inscricao.EventoId,
                EventoTitulo = i.Inscricao.Evento.Titulo,
                CategoriaPesoInicial = (i.CategoriaLutaPeso == null ? 0 : i.CategoriaLutaPeso.PesoInicial),
                CategoriaPesoFinal = (i.CategoriaLutaPeso == null ? 0 : i.CategoriaLutaPeso.PesoFinal),
                Sexo = (i.Inscricao.Filiado.Sexo == "M" ? masculino : feminino),
            }).ToList();

            if (!String.IsNullOrEmpty(tipo))
            {
                ReportDocument rd = new ReportDocument();

                var nomeArquivo = "inscritos001.rpt";
                switch (arquivo)
                {
                    case "Geral":
                        nomeArquivo = "inscritos001.rpt";
                        break;
                    case "Por Evento":
                        nomeArquivo = "inscritos002.rpt";
                        break;
                    case "Por Modalidade":
                        nomeArquivo = "inscritos003.rpt";
                        break;
                    case "Por Academia":
                        nomeArquivo = "inscritos004.rpt";
                        break;
                    case "Por Categoria":
                        nomeArquivo = "inscritos005.rpt";
                        break;
                    case "Resumo":
                        nomeArquivo = "inscritos006.rpt";
                        break;
                    case "Pesagem":
                        nomeArquivo = "inscritos007.rpt";
                        break;
                }

                rd.Load(Path.Combine(Server.MapPath("/Reports"), nomeArquivo));
                CabecalhoReports(rd);
                rd.SetDataSource(inscricoesFiltrado);

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();

                if (tipo == "PDF")
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/pdf", "listagem_de_inscritos_" + arquivo.Replace(" ", "_").ToLower() + ".pdf");
                }
                else if (tipo == "EXCEL")
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/vnd.ms-excel", "listagem_de_inscritos_" + arquivo.Replace(" ", "_").ToLower() + ".xls");
                }
                else if (tipo == "EXCEL")
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/vnd.ms-excel", "listagem_de_inscritos_" + arquivo.Replace(" ", "_").ToLower() + ".xls");
                }
            }

            return Json(inscricoesFiltrado);
        }

        [Route("Admin/Relatorios/Campeonato/Resultados")]
        public ActionResult Resultados()
        {
            ViewBag.ModalidadesId = db.Modalidades.OrderBy(m => m.Titulo);
            ViewBag.AcademiaId = db.Equipes.OrderBy(a => a.Nome);

            return View();
        }

        [Route("Admin/Relatorios/Campeonato/ResultadosDados")]
        public ActionResult ResultadosDados(string tipo, string arquivo, string modalidades, string academias, string eventos, string sexo)
        {
            var modalidadesFilter = (!String.IsNullOrEmpty(modalidades) ? modalidades.Split(',').Select(m => Convert.ToInt64(m)) : new List<Int64>());
            var academiasFilter = (!String.IsNullOrEmpty(academias) ? academias.Split(',').Select(m => Convert.ToInt64(m)) : new List<Int64>());
            var eventosFilter = (!String.IsNullOrEmpty(eventos) ? eventos.Split(',').Select(m => Convert.ToInt64(m)) : new List<Int64>());
            var sexoFilter = (!String.IsNullOrEmpty(sexo) ? sexo.Split(',').Select(m => Convert.ToString(m)) : new List<String>());
            var chaveInscricao = db.ChaveParticipantes.Where(i => (modalidadesFilter.Count() > 0 ? modalidadesFilter.Contains(i.InscricaoModalidade.ModalidadeId) : true) &&
                                                                 (academiasFilter.Count() > 0 ? academiasFilter.Contains(i.InscricaoModalidade.Inscricao.AcademiaId) : true) &&
                                                                 (eventosFilter.Count() > 0 ? eventosFilter.Contains(i.InscricaoModalidade.Inscricao.EventoId) : true) &&
                                                                 (sexoFilter.Count() > 0 ? sexoFilter.Contains(i.InscricaoModalidade.Inscricao.Filiado.Sexo) : true));

            var categoriaLutaPesoTitulo = "-";
            var OK = "OK";
            var Desqualificado = "DESQUALIFICADO";
            var masculino = "Masculino";
            var feminino = "Feminino";
            var inscricoesFiltrado = chaveInscricao.OrderBy(i => i.InscricaoModalidade.Inscricao.Evento.DataInicio).ThenBy(i => i.InscricaoModalidade.Inscricao.Filiado.Nome).Select(i => new
            {
                FiliadoId = i.InscricaoModalidade.Inscricao.FiliadoId,
                FiliadoNome = i.InscricaoModalidade.Inscricao.Filiado.Nome,
                DataNascimento = i.InscricaoModalidade.Inscricao.Filiado.DataNascimento.Value,
                Faixa = i.InscricaoModalidade.Inscricao.Filiado.Faixa.Descricao,
                AcademiaId = i.InscricaoModalidade.Inscricao.AcademiaId,
                AcademiaNome = i.InscricaoModalidade.Inscricao.Academia.Nome,
                CategoriaFaixaId = (i.InscricaoModalidade.CategoriaFaixa == null ? 0 : i.InscricaoModalidade.CategoriaFaixa.Id),
                CategoriaFaixaTitulo = (i.InscricaoModalidade.CategoriaFaixa == null ? categoriaLutaPesoTitulo : i.InscricaoModalidade.CategoriaFaixa.Titulo),
                CategoriaFaixaOrdem = (i.InscricaoModalidade.CategoriaFaixa == null ? 0 : i.InscricaoModalidade.CategoriaFaixa.FaixaInicial.Ordem),
                CategoriaIdadeId = (i.InscricaoModalidade.CategoriaIdade == null ? 0 : i.InscricaoModalidade.CategoriaIdade.Id),
                CategoriaIdadeTitulo = (i.InscricaoModalidade.CategoriaIdade == null ? categoriaLutaPesoTitulo : i.InscricaoModalidade.CategoriaIdade.Titulo),
                CategoriaIdadeOrdem = (i.InscricaoModalidade.CategoriaIdade == null ? 0 : i.InscricaoModalidade.CategoriaIdade.IdadeInicial),
                CategoriaPesoId = (i.InscricaoModalidade.CategoriaLutaPeso == null ? 0 : i.InscricaoModalidade.CategoriaLutaPeso.Id),
                CategoriaPesoTitulo = (i.InscricaoModalidade.CategoriaLutaPeso == null ? categoriaLutaPesoTitulo : i.InscricaoModalidade.CategoriaLutaPeso.Titulo),
                CategoriaPesoOrdem = (i.InscricaoModalidade.CategoriaLutaPeso == null ? 0 : i.InscricaoModalidade.CategoriaLutaPeso.PesoInicial),
                IdModadelidade = i.InscricaoModalidade.ModalidadeId,
                TituloModalidade = i.InscricaoModalidade.Modalidade.Titulo,
                EventoId = i.InscricaoModalidade.Inscricao.EventoId,
                EventoTitulo = i.InscricaoModalidade.Inscricao.Evento.Titulo,
                CategoriaPesoInicial = (i.InscricaoModalidade.CategoriaLutaPeso == null ? 0 : i.InscricaoModalidade.CategoriaLutaPeso.PesoInicial),
                CategoriaPesoFinal = (i.InscricaoModalidade.CategoriaLutaPeso == null ? 0 : i.InscricaoModalidade.CategoriaLutaPeso.PesoFinal),
                Sexo = (i.InscricaoModalidade.Inscricao.Filiado.Sexo == "M" ? masculino : feminino),
                Colocacao = i.Colocacao,
                //Status = (i.Ativo ? OK: Desqualificado),
                Pontuacao = (i.Colocacao == 1 ? 7 : 
                             i.Colocacao == 2 ? 5 :
                             i.Colocacao == 3 ? 3 : 0)
            }).ToList();

            if (!String.IsNullOrEmpty(tipo))
            {
                ReportDocument rd = new ReportDocument();

                var nomeArquivo = "resultados001.rpt";
                switch (arquivo)
                {
                    case "Geral":
                        nomeArquivo = "resultados001.rpt";
                        break;
                    case "Por Academia":
                        nomeArquivo = "resultados002.rpt";
                        break;
                    case "Por Modalidade":
                        nomeArquivo = "resultados003.rpt";
                        break;
                }

                rd.Load(Path.Combine(Server.MapPath("/Reports"), nomeArquivo));
                CabecalhoReports(rd);
                rd.SetDataSource(inscricoesFiltrado);

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();

                if (tipo == "PDF")
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/pdf", "resultados_" + arquivo.Replace(" ", "_").ToLower() + ".pdf");
                }
                else if (tipo == "EXCEL")
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/vnd.ms-excel", "resultados_" + arquivo.Replace(" ", "_").ToLower() + ".xls");
                }
                else if (tipo == "EXCEL")
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/vnd.ms-excel", "listagem_de_inscritos_" + arquivo.Replace(" ", "_").ToLower() + ".xls");
                }
            }

            return Json(inscricoesFiltrado);
        }

        public void CabecalhoReports(ReportDocument rd)
        {
            var evento = db.Eventos.FirstOrDefault();
            ((TextObject)rd.ReportDefinition.ReportObjects["txtNomeEmpresa"]).Text = evento.Titulo.ToUpper();
            ((TextObject)rd.ReportDefinition.ReportObjects["txtCNPJ"]).Text = evento.Descricao.ToUpper();
            ((TextObject)rd.ReportDefinition.ReportObjects["txtEndereco"]).Text = evento.Local.ToUpper();
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