namespace InscricoesOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class equipe : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TBL_EQUIPES", "Login", c => c.String(maxLength: 50));
            AddColumn("dbo.TBL_EQUIPES", "Senha", c => c.String(maxLength: 30));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TBL_EQUIPES", "Senha");
            DropColumn("dbo.TBL_EQUIPES", "Login");
        }
    }
}
