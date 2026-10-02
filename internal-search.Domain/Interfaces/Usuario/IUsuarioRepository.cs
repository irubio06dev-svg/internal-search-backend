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

        // Usuario activo por login o, si no hay, por correo (solo si el correo es inequívoco)
        Task<Usuarios?> ObtenerActivoPorLoginOCorreoAsync(string identificador);
        Task<bool> ExisteLoginAsync(string login);
        Task<bool> ExisteCorreoAsync(string correo);

        // Devuelve los códigos que existen y están activos
        Task<List<int>> ObtenerRolesActivosAsync(IEnumerable<int> codRoles);
        Task<int> CrearAsync(Usuarios usuario, IEnumerable<int> codRoles);
    }
}
