using System;
using System.Collections.Generic;
using System.Text;

namespace internal_search.Domain.DTOs.Buscador
{
    public class BuscadorEntrada
    {
        public string TipoDocumento { get; set; } = string.Empty;
        public string? Documento { get; set; }
        public string? ApePat { get; set; }
        public string? ApeMat { get; set; }
        public string? Prenombres { get; set; }
        public string? Telefono { get; set; }

    }
}
