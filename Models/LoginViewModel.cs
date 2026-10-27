using System.ComponentModel.DataAnnotations;

namespace Desp2Lab3.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [Display(Name = "Nombre de usuario")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; } = string.Empty;
    }
}
