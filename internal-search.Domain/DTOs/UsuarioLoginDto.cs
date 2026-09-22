using System;
using System.Collections.Generic;
using System.Text;

namespace internal_search.Domain.DTOs
{
    public class UsuarioLoginDto
    {
        public int Id { get; set; }

        public string Usuario { get; set; } = string.Empty;

        public string NombreCompleto { get; set; } = string.Empty;

        public List<RolDto> Roles { get; set; } = new();

    }
}
