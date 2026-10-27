using System.Collections.Concurrent;

namespace Desp2Lab3.Services
{
    /// <summary>
    /// Almacén en memoria (+ disco temporal opcional) para no tocar la carpeta del proyecto
    /// y evitar que Visual Studio / hot reload reinicien el proceso al subir archivos.
    /// </summary>
    public class DocumentoStore
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, DocumentoItem>> _usuarios = new();

        public IReadOnlyList<DocumentoItem> Listar(string usuario)
        {
            if (!_usuarios.TryGetValue(Normalizar(usuario), out var docs))
            {
                return Array.Empty<DocumentoItem>();
            }

            return docs.Values.OrderByDescending(d => d.Fecha).ToList();
        }

        public DocumentoItem Guardar(string usuario, string nombreArchivo, byte[] contenido)
        {
            var key = Normalizar(usuario);
            var docs = _usuarios.GetOrAdd(key, _ => new ConcurrentDictionary<string, DocumentoItem>(StringComparer.OrdinalIgnoreCase));

            var nombre = Path.GetFileName(nombreArchivo);
            var item = new DocumentoItem
            {
                Nombre = nombre,
                Contenido = contenido,
                Fecha = DateTime.Now
            };

            docs[nombre] = item;

            // Intento de persistencia fuera del proyecto (si falla, no importa: queda en memoria)
            try
            {
                var carpeta = Path.Combine(Path.GetTempPath(), "Desp2Lab3_Uploads", key);
                Directory.CreateDirectory(carpeta);
                System.IO.File.WriteAllBytes(Path.Combine(carpeta, nombre), contenido);
            }
            catch
            {
                // ignorar errores de disco
            }

            return item;
        }

        public DocumentoItem? Obtener(string usuario, string nombre)
        {
            if (!_usuarios.TryGetValue(Normalizar(usuario), out var docs))
            {
                return null;
            }

            docs.TryGetValue(Path.GetFileName(nombre), out var item);
            return item;
        }

        private static string Normalizar(string usuario)
        {
            var limpio = string.Concat((usuario ?? "usuario").Where(c => char.IsLetterOrDigit(c) || c == '_' || c == '-'));
            return string.IsNullOrEmpty(limpio) ? "usuario" : limpio;
        }
    }

    public class DocumentoItem
    {
        public string Nombre { get; set; } = string.Empty;
        public byte[] Contenido { get; set; } = Array.Empty<byte>();
        public DateTime Fecha { get; set; }

        public string TamanoLegible
        {
            get
            {
                var bytes = Contenido.LongLength;
                if (bytes < 1024) return $"{bytes} B";
                if (bytes < 1024 * 1024) return $"{bytes / 1024.0:0.0} KB";
                return $"{bytes / (1024.0 * 1024.0):0.00} MB";
            }
        }
    }
}
