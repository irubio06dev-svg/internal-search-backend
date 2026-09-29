using System;
using System.Collections.Generic;
using System.Text;

namespace internal_search.Domain.Entities
{
    public class HistorialDescarga
    {
        public int CodHistorial { get; set; }
        public int CodUsuario { get; set; }
        public string NombreArchivo { get; set; } = default!;
        public string RutaArchivo { get; set; } = default!;
        public string Secciones { get; set; } = default!;
        public int TotalDnis { get; set; }
        public long TamanoBytes { get; set; }
        public DateTime FechaCreo { get; set; }
    }
}
