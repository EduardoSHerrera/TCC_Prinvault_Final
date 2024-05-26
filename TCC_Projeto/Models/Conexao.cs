using System.Data.Entity;
using TCC_Projeto.Models.Etiqueta;
using TCC_Projeto.Models.Usuario;
namespace TCC_Projeto.Models

{
    public class Conexao : DbContext
    {
        private const string ConnectionString = @"Data Source=database-1.cpa20amgw67k.us-east-2.rds.amazonaws.com,1433;Initial Catalog=printvault;User Id=admin;Password=senha123;";
        public Conexao() : base(ConnectionString) 
        {
            Usuarios = Set<Usuarios>();
            Etiquetas = Set<Etiquetas>();
            PDF = Set<PDF>();
            PDFEtiquetas = Set<PDFEtiquetas>();
        }
        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Etiquetas> Etiquetas { get; set; }
        public DbSet<PDF> PDF { get; set; }
        public DbSet<PDFEtiquetas> PDFEtiquetas { get; set; }
    }
}
