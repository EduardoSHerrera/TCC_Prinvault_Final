using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using TCC_Projeto.Models;
using System.Security.Cryptography;
using TCC_Projeto.Models.Usuario;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using TCC_Projeto.Models.Etiqueta;

namespace TCC_Projeto.Controllers
{
    public class HomeController : Controller
    {
        private readonly Conexao _context;

        public HomeController(Conexao context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CadastroUsuario()
        {
            return View();
        }

        public async Task<IActionResult> Signout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("~/");
        }

        public async Task<IActionResult> Cadastrar(CadastroViewModel request)
        {
            if (ModelState.IsValid)
            {
                using (var memoryStream = new MemoryStream())
                {
                    if (request.Password == request.Password2)
                    {
                        string senhaCriptografada = CriptografarSenha(request.Password);

                        var CadastroModel = new Usuarios
                        {
                            Username = request.Username,
                            Password = senhaCriptografada,
                            Nome = request.Nome,
                            Sobrenome = request.Sobrenome,
                            Email = request.Email,
                            DataCriacao = DateTime.Now,
                            Ativo = true
                        };

                        _context.Usuarios.Add(CadastroModel);
                        await _context.SaveChangesAsync();

                        return Redirect("~/");
                    }
                    else
                    {
                        return View();
                    }
                }
            }
            else
            {
                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Entrar(LoginViewModel request)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return View();
            }
            if (ModelState.IsValid)
            {
                var usuario = _context.Usuarios.FirstOrDefault(u => u.Username == request.Username);

                if (usuario != null && VerificarSenha(request.Password, usuario.Password))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usuario.Username),
                        new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties();

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return View();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Credenciais inválidas. Por favor, tente novamente.");
                    TempData["ErrorMessage"] = "Credenciais inválidas. Por favor, tente novamente.";
                }
            }

            return Redirect("~/");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private static string CriptografarSenha(string senha)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(senha));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private bool VerificarSenha(string senhaEntrada, string senhaBanco)
        {
            string senhaCriptografada = CriptografarSenha(senhaEntrada);
            return senhaCriptografada == senhaBanco;
        }
    }
}
