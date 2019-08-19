namespace InscricoesOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removido_valor : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.TBL_EVENTOS", "PDF");
            DropColumn("dbo.TBL_FAIXAS", "Valor");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TBL_FAIXAS", "Valor", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.TBL_EVENTOS", "PDF", c => c.String(maxLength: 100));
        }
    }
}
