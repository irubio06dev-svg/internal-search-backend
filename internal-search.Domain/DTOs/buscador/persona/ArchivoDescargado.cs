using System;
using System.Collections.Generic;
using System.Text;

namespace internal_search.Domain.DTOs.buscador.persona
{
    public class ArchivoDescargaDto
    {
        public byte[] Contenido { get; set; } = Array.Empty<byte>();
        public string NombreArchivo { get; set; } = string.Empty;
    }
}
