namespace TCC_Projeto.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PdfRetirarObrigatoriedade : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PDFs", "Data", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PDFs", "Data", c => c.DateTime(nullable: false));
        }
    }
}
