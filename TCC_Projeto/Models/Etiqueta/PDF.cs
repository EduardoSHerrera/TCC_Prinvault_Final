using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCC_Projeto.Models.Etiqueta
{
    public class PDF
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(50)]
        public string? NPedido { get; set; }

        public DateTime? Data { get; set; }

        [StringLength(255)]
        public string? CaminhoPDF { get; set; }
    }

    public class PDFEtiquetas
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(50)]
        public string? NPDF { get; set; }

        [StringLength(50)]
        public string? NFITA { get; set; }

        [StringLength(150)]
        public string? Descricao { get; set; }

        [StringLength(50)]
        public string? Largura { get; set; }

        [StringLength(50)]
        public string? Quantidade { get; set; }
    }
}