using Desp2Lab3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Desp2Lab3.Pages
{
    public class RegistroModel : PageModel
    {
        [BindProperty]
        public RegistroViewModel Input { get; set; } = new();

        public string? MensajeExito { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Sesión (requisito del TP)
            HttpContext.Session.SetString("Usuario", Input.NombreUsuario);
            HttpContext.Session.SetString("Password", Input.Contrasena);
            HttpContext.Session.SetString("Email", Input.Email);
            HttpContext.Session.SetString("NombreCompleto", $"{Input.Nombre} {Input.Apellido}");
            HttpContext.Session.SetString("Edad", Input.Edad?.ToString() ?? "");
            HttpContext.Session.SetString("Autenticado", "true");

            // Cookie con la contraseña (requisito del TP)
            Response.Cookies.Append("PasswordUsuario", Input.Contrasena, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.Now.AddHours(2),
                IsEssential = true,
                SameSite = SameSiteMode.Lax,
                Path = "/"
            });

            Response.Cookies.Append("UsuarioRegistrado", Input.NombreUsuario, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.Now.AddHours(2),
                IsEssential = true,
                SameSite = SameSiteMode.Lax,
                Path = "/"
            });

            // Tras registrarse, entra directo a su espacio de documentos
            return RedirectToPage("/Documentos");
        }
    }
}
