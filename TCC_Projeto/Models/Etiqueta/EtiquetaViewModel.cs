using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCC_Projeto.Models.Etiqueta
{
    public class EtiquetaViewModel
    {
        [Required]
        public required string Nome { get; set; }

        [Required]
        public required string Descricao { get; set; }

        [Required]
        public required IFormFile Imagem { get; set; }
    }

    public class CriarImpressaoModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required int Id { get; set; }

        [Required(ErrorMessage = "O número do pedido é obrigatório")]
        public string? NPedido { get; set; }

        [Required(ErrorMessage = "A data é obrigatória")]
        public DateTime? Data { get; set; }

        [Required(ErrorMessage = "O arquivo PDF é obrigatório")]
        public IFormFile? PDF { get; set; }
    }

    public class CriarEtiquetaPdf
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required int Id { get; set; }

        [Required(ErrorMessage = "O número do pedido é obrigatório")]
        public string? IdFita { get; set; }

        public string? Descricao { get; set; }

        [Required(ErrorMessage = "O número do pedido é obrigatório")]
        public string? Largura { get; set; }

        [Required(ErrorMessage = "O número do pedido é obrigatório")]
        public string? Quantidade { get; set; }
    }

    public class CsvRecord
    {
        public string? Referencia { get; set; }
        public int Quantidade { get; set; }
        public int Largura { get; set; }
    }
}
