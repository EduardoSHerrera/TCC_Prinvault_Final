using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using TCC_Projeto.Models;
using TCC_Projeto.Models.Etiqueta;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Data.Entity;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Image;
using Microsoft.AspNetCore.Authorization;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using System.Reflection.PortableExecutable;


namespace TCC_Projeto.Controllers
{
    public class EtiquetaController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly Conexao _context;

        public EtiquetaController(IWebHostEnvironment hostingEnvironment, Conexao context)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
        }
        [Authorize]
        public IActionResult Etiqueta()
        {
            return View();
        }

        //Método para lidar com o envio do formulário via AJAX
        [HttpPost]
        public async Task<IActionResult> EnviarEtiqueta(EtiquetaViewModel etiqueta)
        {
            if (ModelState.IsValid)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await etiqueta.Imagem.CopyToAsync(memoryStream);
                    var etiquetaModel = new Etiquetas
                    {
                        Nome = etiqueta.Nome,
                        Descricao = etiqueta.Descricao,
                        Imagem = memoryStream.ToArray()
                    };

                    _context.Etiquetas.Add(etiquetaModel);
                    await _context.SaveChangesAsync();
                }

                return Json(new { success = true, message = "Dados enviados com sucesso!" });
            }

            return Json(new { success = false, message = "Erro ao enviar os dados." });
        }

        [HttpPost]
        public async Task<IActionResult> AdicionarNovaFita(CriarEtiquetaPdf request)
        {
            int idFita;
            if (int.TryParse(request.IdFita, out idFita)) // Verifica se a conversão é bem-sucedida
            {
                var obj_fita = await _context.Etiquetas.FindAsync(idFita); // Usa o valor convertido
                if (obj_fita == null)
                {
                    return NotFound("Fita não encontrada.");
                }

                var pdfetiquetas = new PDFEtiquetas
                {
                    NPDF = request.Id.ToString(), // Se for int, convertê-lo para string
                    NFITA = request.IdFita,
                    Descricao = obj_fita.Nome,
                    Largura = request.Largura,
                    Quantidade = request.Quantidade,
                };

                _context.PDFEtiquetas.Add(pdfetiquetas);
                await _context.SaveChangesAsync();

                return RedirectToAction("CriarImpressaoDetalhes", "Etiqueta", new { id = request.Id });
            }
            else
            {
                return BadRequest("IdFita é inválido."); // Retorna erro se a conversão falhar
            }
        }

        [Authorize]
        public async Task<IActionResult> CriarImpressao(CriarImpressaoModel request)
        {
            var pdfModel = new PDF
            {
                NPedido = request.NPedido,
                Data = request.Data,
            };
            _context.PDF.Add(pdfModel);
            await _context.SaveChangesAsync();



            return RedirectToAction("CriarImpressaoDetalhes", new { id = pdfModel.Id });

        }

        [Authorize]
        public async Task<IActionResult> CriarImpressaoDetalhes(int id)
        {
            var pdfModel = await _context.PDF.FindAsync(id);

            if (pdfModel == null)
            {
                return NotFound(); // Retorna um erro se o ID não for encontrado
            }

            ViewBag.PDFId = id;
            ViewBag.NPedido = pdfModel.NPedido;
            ViewBag.Data = pdfModel.Data?.ToString("yyyy-MM-dd");

            return View(pdfModel); // Retorna a visão com o objeto encontrado
        }

        [HttpPost]
        public async Task<IActionResult> ExcluirEtiqueta(int id)
        {
            try
            {
                // Encontrar a etiqueta pelo ID
                var etiqueta = await _context.Etiquetas.FindAsync(id);

                // Verificar se a etiqueta foi encontrada
                if (etiqueta != null)
                {
                    // Remover a etiqueta do contexto
                    _context.Etiquetas.Remove(etiqueta);

                    // Salvar as alterações no banco de dados
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Etiqueta excluída com sucesso!" });
                }
                else
                {
                    return Json(new { success = false, message = "Etiqueta não encontrada." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public byte[]? ObterImagemDaEtiqueta(int id)
        {
            var etiqueta = _context.Etiquetas.FirstOrDefault(e => e.Id == id);

            if (etiqueta != null)
            {
                return etiqueta.Imagem;
            }

            return null;
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodasEtiquetas()
        {
            try
            {
                // Consultar todas as etiquetas do banco de dados de forma assíncrona e sem rastreamento
                var etiquetas = await _context.Etiquetas
                    .AsNoTracking()  // Desativa o rastreamento de mudanças para melhorar o desempenho
                    .Select(e => new
                    {
                        e.Id,
                        e.Nome,
                        e.Descricao,
                        e.Imagem
                    })
                    .ToListAsync();

                if (etiquetas == null || !etiquetas.Any())
                {
                    return Json(new { message = "Nenhuma etiqueta encontrada" });
                }

                // Mapear os dados das etiquetas para um novo objeto com a imagem convertida para base64
                var etiquetasComImagemBase64 = etiquetas.Select(e => new
                {
                    Id = e.Id,
                    Nome = e.Nome,
                    Descricao = e.Descricao,
                    Imagem = Convert.ToBase64String(e.Imagem) // Convertendo a imagem para uma string base64
                }).ToList();

                return Json(etiquetasComImagemBase64);
            }
            catch (Exception ex)
            {
                // Retornar uma mensagem de erro ou logar a exceção conforme necessário
                return StatusCode(500, new { message = "Erro ao obter etiquetas", details = ex.Message });
            }
        }



        [HttpGet]
        public IActionResult ObterTodosPDF()
        {
            // Consultar todas as etiquetas do banco de dados
            var pdfs = _context.PDF.ToList();

            // Mapear os dados das etiquetas para um novo objeto com a imagem convertida para base64
            var query_pdf = pdfs.Select(e => new
            {
                Id = e.Id,
                NPedido = e.NPedido,
                Data = e.Data,
            }).ToList();

            return Json(query_pdf);
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodasEtiquetasPDF(int id)
        {
            // Consultar todas as etiquetas relacionadas ao ID
            var pdf_etiquetas = await _context.PDFEtiquetas
                                             .Where(e => e.NPDF == id.ToString()) // Converte o ID para string
                                             .ToListAsync();

            // Se nenhum resultado for encontrado, retorne um erro ou uma resposta apropriada
            if (pdf_etiquetas == null || pdf_etiquetas.Count == 0)
            {
                return NotFound("Nenhuma etiqueta encontrada.");
            }

            // Mapear para um novo objeto, retornando como JSON
            var query_pdf_etiquetas = pdf_etiquetas.Select(e => new
            {
                id = e.Id,
                descricao = e.Descricao,
                quantidade = e.Quantidade,
                largura = e.Largura
            });

            return Json(query_pdf_etiquetas);
        }


        [HttpGet]
        public IActionResult ModalFitas(int id)
        {
            ViewBag.PDFId = id;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ExcluirPDF(int id)
        {
            
            // Encontrar a etiqueta pelo ID
            var objPdf = await _context.PDF.FindAsync(id);

            // Verificar se a etiqueta foi encontrada
            if (objPdf != null)
            {
                // Remover a etiqueta do contexto
                _context.PDF.Remove(objPdf);

                // Salvar as alterações no banco de dados
                await _context.SaveChangesAsync();

                    
            }
            return RedirectToAction("Entrar", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> SalvarFitasCsv(CsvRequest request)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest("Nenhum arquivo foi enviado.");
            }

            using var stream = new StreamReader(request.File.OpenReadStream());
            var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = true,
                IgnoreBlankLines = true
            };

            using var csvReader = new CsvReader(stream, csvConfiguration);

            // Ler e validar os cabeçalhos
            csvReader.Read();
            csvReader.ReadHeader();
            var headers = csvReader.Context.Reader.HeaderRecord;
            if (headers == null || headers.Length < 3 ||
                headers[0].ToLower() != "referencia" ||
                headers[1].ToLower() != "quantidade" ||
                headers[2].ToLower() != "largura")
            {
                return BadRequest("O arquivo CSV deve conter os cabeçalhos: Referência, Quantidade e Largura.");
            }
            var fitasParaAdicionar = new List<PDFEtiquetas>();
            while (csvReader.Read())
            {
                var referencia = csvReader.GetField<string>("referencia");
                if (string.IsNullOrWhiteSpace(referencia))
                {
                    return BadRequest("O arquivo CSV contém registros inválidos na coluna 'referencia'.");
                }

                if (!csvReader.TryGetField<int>("quantidade", out var quantidade) || quantidade <= 0)
                {
                    return BadRequest("O arquivo CSV contém registros inválidos na coluna 'quantidade'.");
                }

                if (!csvReader.TryGetField<int>("largura", out var largura) || largura <= 0)
                {
                    return BadRequest("O arquivo CSV contém registros inválidos na coluna 'largura'.");
                }

                var imagemCsv = _context.Etiquetas.FirstOrDefault(e => e.Nome == referencia);

                if(imagemCsv != null)
                {
                    var fita = new PDFEtiquetas
                    {
                        NPDF = request.Id.ToString(),
                        Descricao = referencia,
                        Quantidade = quantidade.ToString(),
                        Largura = largura.ToString(),
                        NFITA = imagemCsv.Id.ToString()
                    };

                    fitasParaAdicionar.Add(fita);
                }
            }

            _context.PDFEtiquetas.AddRange(fitasParaAdicionar);
            await _context.SaveChangesAsync();

            return RedirectToAction("CriarImpressaoDetalhes", "Etiqueta", new { id = fitasParaAdicionar.FirstOrDefault()?.NPDF });
        }

        [HttpPost]
        public async Task<IActionResult> ExcluirEtiquetaPdf(int id)
        {
            try
            {
                // Encontrar a etiqueta pelo ID
                var etiqueta = await _context.PDFEtiquetas.FindAsync(id);

                // Verificar se a etiqueta foi encontrada
                if (etiqueta != null)
                {
                    // Remover a etiqueta do contexto
                    _context.PDFEtiquetas.Remove(etiqueta);

                    // Salvar as alterações no banco de dados
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Etiqueta excluída com sucesso!" });
                }
                else
                {
                    return Json(new { success = false, message = "Etiqueta não encontrada." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
