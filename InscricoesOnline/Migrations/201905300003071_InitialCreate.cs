namespace InscricoesOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TBL_ATLETAS",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Nome = c.String(nullable: false, maxLength: 150),
                        RG = c.String(maxLength: 20),
                        CPF = c.String(maxLength: 20),
                        DataNascimento = c.DateTime(nullable: false),
                        Sexo = c.String(nullable: false, maxLength: 20),
                        EstadoCivil = c.String(nullable: false, maxLength: 40),
                        CertidaoNascimento = c.String(maxLength: 40),
                        NomePai = c.String(maxLength: 150),
                        NomeMae = c.String(maxLength: 150),
                        RegistroCBTKD = c.String(maxLength: 40),
                        Observacoes = c.String(maxLength: 400),
                        Ativo = c.Boolean(nullable: false),
                        DataRegistro = c.DateTime(nullable: false),
                        Parataekwondo = c.Boolean(nullable: false),
                        URLFoto = c.String(maxLength: 10),
                        FaixaId = c.Long(nullable: false),
                        AcademiaId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TBL_EQUIPES", t => t.AcademiaId)
                .ForeignKey("dbo.TBL_FAIXAS", t => t.FaixaId)
                .Index(t => t.AcademiaId)
                .Index(t => t.FaixaId);
            
            CreateTable(
                "dbo.TBL_EQUIPES",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Nome = c.String(nullable: false, maxLength: 100),
                        CNPJ = c.String(maxLength: 20),
                        DataRegistro = c.DateTime(nullable: false),
                        EventoId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TBL_EVENTOS", t => t.EventoId)
                .Index(t => t.EventoId);
            
            CreateTable(
                "dbo.TBL_EVENTOS",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Titulo = c.String(nullable: false, maxLength: 60),
                        Descricao = c.String(),
                        AtivaInscricao = c.Boolean(nullable: false),
                        Login = c.String(maxLength: 30),
                        Senha = c.String(maxLength: 30),
                        DataInicio = c.DateTime(nullable: false),
                        DataFim = c.DateTime(nullable: false),
                        DataInicioInscricoes = c.DateTime(),
                        DataFimInscricoes = c.DateTime(),
                        DataFimAlteracoes = c.DateTime(),
                        Ativo = c.Boolean(nullable: false),
                        Imagem = c.String(maxLength: 10),
                        PDF = c.String(maxLength: 100),
                        Cidade = c.String(maxLength: 100),
                        Estado = c.String(maxLength: 10),
                        Local = c.String(maxLength: 120),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TBL_FAIXAS",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Descricao = c.String(nullable: false, maxLength: 80),
                        Valor = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Ordem = c.Int(nullable: false),
                        DataRegistro = c.DateTime(nullable: false),
                        EventoId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TBL_EVENTOS", t => t.EventoId)
                .Index(t => t.EventoId);
            
            CreateTable(
                "dbo.TBL_CATEGORIAS_FAIXA",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Titulo = c.String(nullable: false, maxLength: 50),
                        ModalidadeId = c.Long(nullable: false),
                        FaixaIdFaixaInicial = c.Long(nullable: false),
                        FaixaIdFaixaFinal = c.Long(nullable: false),
                        Ativo = c.Boolean(nullable: false),
                        EventoId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TBL_EVENTOS", t => t.EventoId)
                .ForeignKey("dbo.TBL_FAIXAS", t => t.FaixaIdFaixaFinal)
                .ForeignKey("dbo.TBL_FAIXAS", t => t.FaixaIdFaixaInicial)
                .ForeignKey("dbo.TBL_MODALIDADES", t => t.ModalidadeId)
                .Index(t => t.EventoId)
                .Index(t => t.FaixaIdFaixaFinal)
                .Index(t => t.FaixaIdFaixaInicial)
                .Index(t => t.ModalidadeId);
            
            CreateTable(
                "dbo.TBL_MODALIDADES",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Titulo = c.String(nullable: false, maxLength: 50),
                        CategoriaPeso = c.Boolean(nullable: false),
                        CategoriaFaixa = c.Boolean(nullable: false),
                        CategoriaIdade = c.Boolean(nullable: false),
                        Ranking = c.Boolean(nullable: false),
                        Parataekwondo = c.Boolean(nullable: false),
                        EventoId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TBL_EVENTOS", t => t.EventoId)
                .Index(t => t.EventoId);
            
            CreateTable(
                "dbo.TBL_CATEGORIAS_IDADE",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Titulo = c.String(nullable: false, maxLength: 50),
                        ModalidadeId = c.Long(nullable: false),
                        IdadeInicial = c.Int(nullable: false),
                        IdadeFinal = c.Int(nullable: false),
                        FaixaIdFaixaInicial = c.Long(nullable: false),
                        FaixaIdFaixaFinal = c.Long(nullable: false),
                        Ativo = c.Boolean(nullable: false),
                        EventoId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TBL_EVENTOS", t => t.EventoId)
                .ForeignKey("dbo.TBL_FAIXAS", t => t.FaixaIdFaixaFinal)
                .ForeignKey("dbo.TBL_FAIXAS", t => t.FaixaIdFaixaInicial)
                .ForeignKey("dbo.TBL_MODALIDADES", t => t.ModalidadeId)
                .Index(t => t.EventoId)
                .Index(t => t.FaixaIdFaixaFinal)
                .Index(t => t.FaixaIdFaixaInicial)
                .Index(t => t.ModalidadeId);
            
            CreateTable(
                "dbo.TBL_CATEGORIAS_PESO",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Titulo = c.String(nullable: false, maxLength: 50),
                        PesoInicial = c.Int(nullable: false),
                        PesoFinal = c.Int(nullable: false),
                        Sexo = c.String(nullable: false, maxLength: 10),
                        CategoriaIdadeId = c.Long(nullable: false),
                        Ativo = c.Boolean(nullable: false),
                        EventoId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TBL_CATEGORIAS_IDADE", t => t.CategoriaIdadeId)
                .ForeignKey("dbo.TBL_EVENTOS", t => t.EventoId)
                .Index(t => t.CategoriaIdadeId)
                .Index(t => t.EventoId);
            
            CreateTable(
                "dbo.TBL_CHAVE_PARTICIPANTE",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ChaveId = c.Long(nullable: false),
                        InscricaoModalidadeId = c.Long(nullable: false),
                        Ordem = c.Int(nullable: false),
                        Colocacao = c.Int(nullable: false),
                        EventoId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TBL_CHAVE", t => t.ChaveId)
                .ForeignKey("dbo.TBL_EVENTOS", t => t.EventoId)
                .ForeignKey("dbo.TBL_INSCRICOES_MODALIDADES", t => t.InscricaoModalidadeId)
                .Index(t => t.ChaveId)
                .Index(t => t.EventoId)
                .Index(t => t.InscricaoModalidadeId);
            
            CreateTable(
                "dbo.TBL_CHAVE",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EventoId = c.Long(nullable: false),
                        Sexo = c.String(maxLength: 10),
                        ModalidadeId = c.Long(nullable: false),
                        CategoriaFaixaId = c.Long(),
                        CategoriaIdadeId = c.Long(),
                        CategoriaLutaPesoId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TBL_CATEGORIAS_FAIXA", t => t.CategoriaFaixaId)
                .ForeignKey("dbo.TBL_CATEGORIAS_IDADE", t => t.CategoriaIdadeId)
                .ForeignKey("dbo.TBL_CATEGORIAS_PESO", t => t.CategoriaLutaPesoId)
                .ForeignKey("dbo.TBL_EVENTOS", t => t.EventoId)
                .ForeignKey("dbo.TBL_MODALIDADES", t => t.ModalidadeId)
                .Index(t => t.CategoriaFaixaId)
                .Index(t => t.CategoriaIdadeId)
                .Index(t => t.CategoriaLutaPesoId)
                .Index(t => t.EventoId)
                .Index(t => t.ModalidadeId);
            
            CreateTable(
                "dbo.TBL_INSCRICOES_MODALIDADES",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        InscricaoId = c.Long(nullable: false),
                        ModalidadeId = c.Long(nullable: false),
                        CategoriaFaixaId = c.Long(),
                        CategoriaIdadeId = c.Long(),
                        CategoriaLutaPesoId = c.Long(),
                        EventoId = c.Long(nullable: false),
                        Chave_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TBL_CATEGORIAS_FAIXA", t => t.CategoriaFaixaId)
                .ForeignKey("dbo.TBL_CATEGORIAS_IDADE", t => t.CategoriaIdadeId)
                .ForeignKey("dbo.TBL_CATEGORIAS_PESO", t => t.CategoriaLutaPesoId)
                .ForeignKey("dbo.TBL_EVENTOS", t => t.EventoId)
                .ForeignKey("dbo.TBL_INSCRICOES", t => t.InscricaoId)
                .ForeignKey("dbo.TBL_MODALIDADES", t => t.ModalidadeId)
                .ForeignKey("dbo.TBL_CHAVE", t => t.Chave_Id)
                .Index(t => t.CategoriaFaixaId)
                .Index(t => t.CategoriaIdadeId)
                .Index(t => t.CategoriaLutaPesoId)
                .Index(t => t.EventoId)
                .Index(t => t.InscricaoId)
                .Index(t => t.ModalidadeId)
                .Index(t => t.Chave_Id);
            
            CreateTable(
                "dbo.TBL_INSCRICOES",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EventoId = c.Long(nullable: false),
                        AcademiaId = c.Long(nullable: false),
                        FiliadoId = c.Long(nullable: false),
                        DataInscricao = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TBL_EQUIPES", t => t.AcademiaId)
                .ForeignKey("dbo.TBL_EVENTOS", t => t.EventoId)
                .ForeignKey("dbo.TBL_ATLETAS", t => t.FiliadoId)
                .Index(t => t.AcademiaId)
                .Index(t => t.EventoId)
                .Index(t => t.FiliadoId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TBL_CHAVE_PARTICIPANTE", "InscricaoModalidadeId", "dbo.TBL_INSCRICOES_MODALIDADES");
            DropForeignKey("dbo.TBL_CHAVE_PARTICIPANTE", "EventoId", "dbo.TBL_EVENTOS");
            DropForeignKey("dbo.TBL_CHAVE", "ModalidadeId", "dbo.TBL_MODALIDADES");
            DropForeignKey("dbo.TBL_INSCRICOES_MODALIDADES", "Chave_Id", "dbo.TBL_CHAVE");
            DropForeignKey("dbo.TBL_INSCRICOES_MODALIDADES", "ModalidadeId", "dbo.TBL_MODALIDADES");
            DropForeignKey("dbo.TBL_INSCRICOES_MODALIDADES", "InscricaoId", "dbo.TBL_INSCRICOES");
            DropForeignKey("dbo.TBL_INSCRICOES", "FiliadoId", "dbo.TBL_ATLETAS");
            DropForeignKey("dbo.TBL_INSCRICOES", "EventoId", "dbo.TBL_EVENTOS");
            DropForeignKey("dbo.TBL_INSCRICOES", "AcademiaId", "dbo.TBL_EQUIPES");
            DropForeignKey("dbo.TBL_INSCRICOES_MODALIDADES", "EventoId", "dbo.TBL_EVENTOS");
            DropForeignKey("dbo.TBL_INSCRICOES_MODALIDADES", "CategoriaLutaPesoId", "dbo.TBL_CATEGORIAS_PESO");
            DropForeignKey("dbo.TBL_INSCRICOES_MODALIDADES", "CategoriaIdadeId", "dbo.TBL_CATEGORIAS_IDADE");
            DropForeignKey("dbo.TBL_INSCRICOES_MODALIDADES", "CategoriaFaixaId", "dbo.TBL_CATEGORIAS_FAIXA");
            DropForeignKey("dbo.TBL_CHAVE", "EventoId", "dbo.TBL_EVENTOS");
            DropForeignKey("dbo.TBL_CHAVE_PARTICIPANTE", "ChaveId", "dbo.TBL_CHAVE");
            DropForeignKey("dbo.TBL_CHAVE", "CategoriaLutaPesoId", "dbo.TBL_CATEGORIAS_PESO");
            DropForeignKey("dbo.TBL_CHAVE", "CategoriaIdadeId", "dbo.TBL_CATEGORIAS_IDADE");
            DropForeignKey("dbo.TBL_CHAVE", "CategoriaFaixaId", "dbo.TBL_CATEGORIAS_FAIXA");
            DropForeignKey("dbo.TBL_CATEGORIAS_PESO", "EventoId", "dbo.TBL_EVENTOS");
            DropForeignKey("dbo.TBL_CATEGORIAS_PESO", "CategoriaIdadeId", "dbo.TBL_CATEGORIAS_IDADE");
            DropForeignKey("dbo.TBL_CATEGORIAS_IDADE", "ModalidadeId", "dbo.TBL_MODALIDADES");
            DropForeignKey("dbo.TBL_CATEGORIAS_IDADE", "FaixaIdFaixaInicial", "dbo.TBL_FAIXAS");
            DropForeignKey("dbo.TBL_CATEGORIAS_IDADE", "FaixaIdFaixaFinal", "dbo.TBL_FAIXAS");
            DropForeignKey("dbo.TBL_CATEGORIAS_IDADE", "EventoId", "dbo.TBL_EVENTOS");
            DropForeignKey("dbo.TBL_CATEGORIAS_FAIXA", "ModalidadeId", "dbo.TBL_MODALIDADES");
            DropForeignKey("dbo.TBL_MODALIDADES", "EventoId", "dbo.TBL_EVENTOS");
            DropForeignKey("dbo.TBL_CATEGORIAS_FAIXA", "FaixaIdFaixaInicial", "dbo.TBL_FAIXAS");
            DropForeignKey("dbo.TBL_CATEGORIAS_FAIXA", "FaixaIdFaixaFinal", "dbo.TBL_FAIXAS");
            DropForeignKey("dbo.TBL_CATEGORIAS_FAIXA", "EventoId", "dbo.TBL_EVENTOS");
            DropForeignKey("dbo.TBL_ATLETAS", "FaixaId", "dbo.TBL_FAIXAS");
            DropForeignKey("dbo.TBL_FAIXAS", "EventoId", "dbo.TBL_EVENTOS");
            DropForeignKey("dbo.TBL_ATLETAS", "AcademiaId", "dbo.TBL_EQUIPES");
            DropForeignKey("dbo.TBL_EQUIPES", "EventoId", "dbo.TBL_EVENTOS");
            DropIndex("dbo.TBL_CHAVE_PARTICIPANTE", new[] { "InscricaoModalidadeId" });
            DropIndex("dbo.TBL_CHAVE_PARTICIPANTE", new[] { "EventoId" });
            DropIndex("dbo.TBL_CHAVE", new[] { "ModalidadeId" });
            DropIndex("dbo.TBL_INSCRICOES_MODALIDADES", new[] { "Chave_Id" });
            DropIndex("dbo.TBL_INSCRICOES_MODALIDADES", new[] { "ModalidadeId" });
            DropIndex("dbo.TBL_INSCRICOES_MODALIDADES", new[] { "InscricaoId" });
            DropIndex("dbo.TBL_INSCRICOES", new[] { "FiliadoId" });
            DropIndex("dbo.TBL_INSCRICOES", new[] { "EventoId" });
            DropIndex("dbo.TBL_INSCRICOES", new[] { "AcademiaId" });
            DropIndex("dbo.TBL_INSCRICOES_MODALIDADES", new[] { "EventoId" });
            DropIndex("dbo.TBL_INSCRICOES_MODALIDADES", new[] { "CategoriaLutaPesoId" });
            DropIndex("dbo.TBL_INSCRICOES_MODALIDADES", new[] { "CategoriaIdadeId" });
            DropIndex("dbo.TBL_INSCRICOES_MODALIDADES", new[] { "CategoriaFaixaId" });
            DropIndex("dbo.TBL_CHAVE", new[] { "EventoId" });
            DropIndex("dbo.TBL_CHAVE_PARTICIPANTE", new[] { "ChaveId" });
            DropIndex("dbo.TBL_CHAVE", new[] { "CategoriaLutaPesoId" });
            DropIndex("dbo.TBL_CHAVE", new[] { "CategoriaIdadeId" });
            DropIndex("dbo.TBL_CHAVE", new[] { "CategoriaFaixaId" });
            DropIndex("dbo.TBL_CATEGORIAS_PESO", new[] { "EventoId" });
            DropIndex("dbo.TBL_CATEGORIAS_PESO", new[] { "CategoriaIdadeId" });
            DropIndex("dbo.TBL_CATEGORIAS_IDADE", new[] { "ModalidadeId" });
            DropIndex("dbo.TBL_CATEGORIAS_IDADE", new[] { "FaixaIdFaixaInicial" });
            DropIndex("dbo.TBL_CATEGORIAS_IDADE", new[] { "FaixaIdFaixaFinal" });
            DropIndex("dbo.TBL_CATEGORIAS_IDADE", new[] { "EventoId" });
            DropIndex("dbo.TBL_CATEGORIAS_FAIXA", new[] { "ModalidadeId" });
            DropIndex("dbo.TBL_MODALIDADES", new[] { "EventoId" });
            DropIndex("dbo.TBL_CATEGORIAS_FAIXA", new[] { "FaixaIdFaixaInicial" });
            DropIndex("dbo.TBL_CATEGORIAS_FAIXA", new[] { "FaixaIdFaixaFinal" });
            DropIndex("dbo.TBL_CATEGORIAS_FAIXA", new[] { "EventoId" });
            DropIndex("dbo.TBL_ATLETAS", new[] { "FaixaId" });
            DropIndex("dbo.TBL_FAIXAS", new[] { "EventoId" });
            DropIndex("dbo.TBL_ATLETAS", new[] { "AcademiaId" });
            DropIndex("dbo.TBL_EQUIPES", new[] { "EventoId" });
            DropTable("dbo.TBL_INSCRICOES");
            DropTable("dbo.TBL_INSCRICOES_MODALIDADES");
            DropTable("dbo.TBL_CHAVE");
            DropTable("dbo.TBL_CHAVE_PARTICIPANTE");
            DropTable("dbo.TBL_CATEGORIAS_PESO");
            DropTable("dbo.TBL_CATEGORIAS_IDADE");
            DropTable("dbo.TBL_MODALIDADES");
            DropTable("dbo.TBL_CATEGORIAS_FAIXA");
            DropTable("dbo.TBL_FAIXAS");
            DropTable("dbo.TBL_EVENTOS");
            DropTable("dbo.TBL_EQUIPES");
            DropTable("dbo.TBL_ATLETAS");
        }
    }
}
