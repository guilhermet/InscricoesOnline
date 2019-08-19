namespace InscricoesOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class atleta : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.TBL_ATLETAS", name: "AcademiaId", newName: "EquipeId");
            RenameIndex(table: "dbo.TBL_ATLETAS", name: "IX_AcademiaId", newName: "IX_EquipeId");
            DropColumn("dbo.TBL_ATLETAS", "RG");
            DropColumn("dbo.TBL_ATLETAS", "EstadoCivil");
            DropColumn("dbo.TBL_ATLETAS", "CertidaoNascimento");
            DropColumn("dbo.TBL_ATLETAS", "NomePai");
            DropColumn("dbo.TBL_ATLETAS", "NomeMae");
            DropColumn("dbo.TBL_ATLETAS", "RegistroCBTKD");
            DropColumn("dbo.TBL_ATLETAS", "Observacoes");
            DropColumn("dbo.TBL_ATLETAS", "URLFoto");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TBL_ATLETAS", "URLFoto", c => c.String(maxLength: 10));
            AddColumn("dbo.TBL_ATLETAS", "Observacoes", c => c.String(maxLength: 400));
            AddColumn("dbo.TBL_ATLETAS", "RegistroCBTKD", c => c.String(maxLength: 40));
            AddColumn("dbo.TBL_ATLETAS", "NomeMae", c => c.String(maxLength: 150));
            AddColumn("dbo.TBL_ATLETAS", "NomePai", c => c.String(maxLength: 150));
            AddColumn("dbo.TBL_ATLETAS", "CertidaoNascimento", c => c.String(maxLength: 40));
            AddColumn("dbo.TBL_ATLETAS", "EstadoCivil", c => c.String(nullable: false, maxLength: 40));
            AddColumn("dbo.TBL_ATLETAS", "RG", c => c.String(maxLength: 20));
            RenameIndex(table: "dbo.TBL_ATLETAS", name: "IX_EquipeId", newName: "IX_AcademiaId");
            RenameColumn(table: "dbo.TBL_ATLETAS", name: "EquipeId", newName: "AcademiaId");
        }
    }
}
