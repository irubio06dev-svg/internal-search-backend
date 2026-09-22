using internal_search.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace internal_search.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObtenerPorUsuarioAsync(string usuario);
        Task<Usuario?> ObtenerPorIdAsync(int codUsuario);

    }
}
