namespace InscricoesOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeaddativochaveparticipante : DbMigration
    {
        public override void Up()
        {
            AddColumn("LIXO.TBL_CHAVE_PARTICIPANTE", "Ativo", c => c.Decimal(nullable: true, precision: 1, scale: 0));
            DropColumn("LIXO.TBL_CHAVE", "Ativo");
        }
        
        public override void Down()
        {
            AddColumn("LIXO.TBL_CHAVE", "Ativo", c => c.Decimal(nullable: false, precision: 1, scale: 0));
            DropColumn("LIXO.TBL_CHAVE_PARTICIPANTE", "Ativo");
        }
    }
}
