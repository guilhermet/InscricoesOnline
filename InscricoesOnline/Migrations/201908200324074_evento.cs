namespace InscricoesOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class evento : DbMigration
    {
        public override void Up()
        {
            AddColumn("LIXO.TBL_EQUIPES", "Telefone", c => c.String(maxLength: 30));
            AddColumn("LIXO.TBL_EQUIPES", "Email", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("LIXO.TBL_EQUIPES", "Email");
            DropColumn("LIXO.TBL_EQUIPES", "Telefone");
        }
    }
}
