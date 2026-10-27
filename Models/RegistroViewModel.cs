using System.ComponentModel.DataAnnotations;

namespace Desp2Lab3.Models
{
    public class RegistroViewModel
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [Display(Name = "Nombre de usuario")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 50 caracteres.")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "La edad es obligatoria.")]
        [Range(16, 120, ErrorMessage = "Solo se aceptan usuarios mayores de 15 años.")]
        [Display(Name = "Edad")]
        public int? Edad { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "La contraseña debe tener al menos 4 caracteres.")]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe confirmar la contraseña.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Contrasena), ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar contraseña")]
        public string ConfirmarContrasena { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; } = string.Empty;
    }
}
