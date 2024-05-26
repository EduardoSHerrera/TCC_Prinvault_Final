namespace TCC_Projeto.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class criacaoEtiquetaPDF : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PDFEtiquetas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NPDF = c.String(maxLength: 50),
                        Descricao = c.String(maxLength: 150),
                        Largura = c.String(maxLength: 50),
                        Quantidade = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PDFEtiquetas");
        }
    }
}
