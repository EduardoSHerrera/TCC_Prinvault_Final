using AngleSharp.Text;
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

            var etiquetas = await _context.Etiquetas
                             .Where(e => listaNFITA.Contains(e.Id.ToString())) // Busca onde o ID corresponde
                             .ToListAsync();

            var TodasFitasQuantidade = new List<object>();

            foreach(var obj_etiqueta in pdf_etiquetas)
            {
                if(obj_etiqueta != null)
                {
                    var quantidade_etiqueta = int.Parse(obj_etiqueta.Quantidade);
                    var etiqueta = etiquetas.FirstOrDefault(e => e.Id.ToString() == obj_etiqueta.NFITA);

                    if (etiqueta != null)
                    {
                        for (int i = 0; i < quantidade_etiqueta; i++)
                        {
                            TodasFitasQuantidade.Add(new
                            {
                                Base64Image = etiqueta.Imagem != null ? Convert.ToBase64String(etiqueta.Imagem) : string.Empty,
                                Descricao = etiqueta.Descricao // Outros dados para usar no HTML
                            });
                        }
                    }
                }
            }

            // Retorna a view para criar o PDF, passando a lista de imagens
            return new ViewAsPdf("CriarPdf", TodasFitasQuantidade)
            {
            CustomSwitches = "--page-width 600mm --page-height 213mm", // Define um tamanho de papel de 610 mm por 840 mm
                PageOrientation = Orientation.Portrait, // Orientação retrato
                PageMargins = new Margins(0, 0, 0, 0) // Definição das margens
            }; // Passa a lista para a view
        }
    }
}
