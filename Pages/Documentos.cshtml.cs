using Desp2Lab3.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Desp2Lab3.Pages
{
    [IgnoreAntiforgeryToken]
    public class DocumentosModel : PageModel
    {
        private readonly DocumentoStore _store;
        private readonly ILogger<DocumentosModel> _logger;

        private static readonly HashSet<string> ExtensionesPermitidas = new(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf", ".doc", ".docx", ".txt", ".png", ".jpg", ".jpeg"
        };

        public DocumentosModel(DocumentoStore store, ILogger<DocumentosModel> logger)
        {
            _store = store;
            _logger = logger;
        }

        public string NombreUsuario { get; set; } = string.Empty;
        public string? Mensaje { get; set; }
        public bool EsError { get; set; }
        public List<DocumentoItem> Archivos { get; set; } = new();

        [BindProperty]
        public IFormFile? Archivo { get; set; }

        public IActionResult OnGet()
        {
            if (!EstaAutenticado())
            {
                return RedirectToPage("/Login");
            }

            CargarDatos();
            return Page();
        }

        public async Task<IActionResult> OnPostSubirAsync()
        {
            _logger.LogInformation("=== INICIO SUBIDA ===");

            if (!EstaAutenticado())
            {
                _logger.LogWarning("Subida rechazada: no autenticado");
                return RedirectToPage("/Login");
            }

            NombreUsuario = HttpContext.Session.GetString("Usuario") ?? "";

            try
            {
                if (Archivo == null || Archivo.Length == 0)
                {
                    return MostrarError("Debe seleccionar un archivo válido.");
                }

                _logger.LogInformation("Archivo recibido: {Name} ({Length} bytes)", Archivo.FileName, Archivo.Length);

                if (Archivo.Length > 10 * 1024 * 1024)
                {
                    return MostrarError("El archivo supera el tamaño máximo de 10 MB.");
                }

                var extension = Path.GetExtension(Archivo.FileName);
                if (string.IsNullOrEmpty(extension) || !ExtensionesPermitidas.Contains(extension))
                {
                    return MostrarError("Tipo de archivo no permitido. Use PDF, DOC, DOCX, TXT, PNG o JPG.");
                }

                await using var ms = new MemoryStream();
                await Archivo.CopyToAsync(ms);
                var bytes = ms.ToArray();

                var guardado = _store.Guardar(NombreUsuario, Archivo.FileName, bytes);
                Mensaje = $"Documento \"{guardado.Nombre}\" cargado correctamente.";
                EsError = false;
                _logger.LogInformation("=== SUBIDA OK: {Nombre} ===", guardado.Nombre);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en subida");
                Mensaje = $"Error al subir el archivo: {ex.Message}";
                EsError = true;
            }

            CargarListaArchivos();
            return Page();
        }

        public IActionResult OnPostCerrarSesion()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }

        public IActionResult OnGetDescargar(string nombre)
        {
            if (!EstaAutenticado())
            {
                return RedirectToPage("/Login");
            }

            var usuario = HttpContext.Session.GetString("Usuario") ?? "";
            var item = _store.Obtener(usuario, nombre);
            if (item == null)
            {
                return NotFound();
            }

            return File(item.Contenido, "application/octet-stream", item.Nombre);
        }

        private IActionResult MostrarError(string mensaje)
        {
            Mensaje = mensaje;
            EsError = true;
            CargarListaArchivos();
            return Page();
        }

        private bool EstaAutenticado()
        {
            return HttpContext.Session.GetString("Autenticado") == "true"
                && !string.IsNullOrEmpty(HttpContext.Session.GetString("Usuario"));
        }

        private void CargarDatos()
        {
            NombreUsuario = HttpContext.Session.GetString("Usuario") ?? "";
            CargarListaArchivos();
        }

        private void CargarListaArchivos()
        {
            NombreUsuario = HttpContext.Session.GetString("Usuario") ?? NombreUsuario;
            Archivos = _store.Listar(NombreUsuario).ToList();
        }
    }
}
