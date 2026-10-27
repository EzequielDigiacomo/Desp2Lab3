using Desp2Lab3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Desp2Lab3.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public LoginViewModel Input { get; set; } = new();

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var usuarioCookie = Request.Cookies["UsuarioRegistrado"];
            var passwordCookie = Request.Cookies["PasswordUsuario"];
            var usuarioSesion = HttpContext.Session.GetString("Usuario");
            var passwordSesion = HttpContext.Session.GetString("Password");

            var usuarioValido = (!string.IsNullOrEmpty(usuarioCookie) && Input.NombreUsuario == usuarioCookie)
                || (!string.IsNullOrEmpty(usuarioSesion) && Input.NombreUsuario == usuarioSesion);

            var passwordValida = (!string.IsNullOrEmpty(passwordCookie) && Input.Contrasena == passwordCookie)
                || (!string.IsNullOrEmpty(passwordSesion) && Input.Contrasena == passwordSesion);

            if (!usuarioValido || !passwordValida)
            {
                ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos. Debe registrarse primero.");
                return Page();
            }

            HttpContext.Session.SetString("Usuario", Input.NombreUsuario);
            HttpContext.Session.SetString("Password", Input.Contrasena);
            HttpContext.Session.SetString("Autenticado", "true");

            return RedirectToPage("/Documentos");
        }
    }
}
