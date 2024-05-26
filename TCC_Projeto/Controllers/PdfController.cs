using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using System.Data.Entity;
using System.Linq;
using TCC_Projeto.Models;
using TCC_Projeto.Models.Etiqueta;


namespace TCC_Projeto.Controllers
{
    public class PdfController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly Conexao _context;

        public PdfController(IWebHostEnvironment hostingEnvironment, Conexao context)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
        }

        public async Task<IActionResult> CriarPdf(int id)
        {
            // Obtém a lista de PDFEtiquetas com base no ID
            var pdf_etiquetas = await _context.PDFEtiquetas
                                             .Where(e => e.NPDF == id.ToString())
                                             .ToListAsync();

            // Se a lista estiver vazia ou nula, retorne NotFound
            if (pdf_etiquetas == null || pdf_etiquetas.Count == 0)
            {
                return NotFound("Nenhuma etiqueta encontrada.");
            }

            // Obter todos os valores de NFITA da lista
            var listaNFITA = pdf_etiquetas
                             .Select(e => e.NFITA) // Obter valores de NFITA
                             .Distinct() // Garantir valores únicos
                             .ToList();

            // Verificar se a lista está vazia
            if (listaNFITA.Count == 0)
            {
                return NotFound("Nenhum valor de NFITA encontrado.");
            }

            // Buscar todas as etiquetas associadas aos valores de NFITA
            var etiquetas = await _context.Etiquetas
                                         .Where(e => listaNFITA.Contains(e.Id.ToString())) // Busca onde o ID corresponde
                                         .ToListAsync();

            // Converter as imagens para Base64 e criar o modelo para a view
            var etiquetasComImagemBase64 = etiquetas.Select(e => new
            {
                Base64Image = e.Imagem != null ? Convert.ToBase64String(e.Imagem) : string.Empty,
                Descricao = e.Descricao // Outros dados para usar no HTML
            }).ToList();

            // Retorna a view para criar o PDF, passando a lista de imagens
            return new ViewAsPdf("CriarPdf", etiquetasComImagemBase64)
            {
            CustomSwitches = "--page-width 600mm --page-height 213mm", // Define um tamanho de papel de 610 mm por 840 mm
                PageOrientation = Orientation.Portrait, // Orientação retrato
                PageMargins = new Margins(0, 0, 0, 0) // Definição das margens
            }; // Passa a lista para a view
        }
    }
}
