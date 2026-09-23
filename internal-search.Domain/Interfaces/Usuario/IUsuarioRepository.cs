using internal_search.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace internal_search.Domain.Interfaces.Usuario
{
    public interface IUsuarioRepository
    {
        Task<Usuarios?> ObtenerPorUsuarioAsync(string usuario);
        Task<Usuarios?> ObtenerPorIdAsync(int codUsuario);

    }
}
