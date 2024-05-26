namespace TCC_Projeto.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CampoFitaId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PDFEtiquetas", "NFITA", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PDFEtiquetas", "NFITA");
        }
    }
}
