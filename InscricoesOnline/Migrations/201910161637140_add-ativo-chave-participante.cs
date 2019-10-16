namespace InscricoesOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addativochaveparticipante : DbMigration
    {
        public override void Up()
        {
            AddColumn("LIXO.TBL_CHAVE", "Ativo", c => c.Decimal(nullable: true, defaultValue: 1, precision: 1, scale: 0));
        }
        
        public override void Down()
        {
            DropColumn("LIXO.TBL_CHAVE", "Ativo");
        }
    }
}
