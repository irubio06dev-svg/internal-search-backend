using Microsoft.EntityFrameworkCore;
using internal_search.Domain.Entities;
using internal_search.Domain.Interfaces.Usuario;

namespace internal_search_backend.Infraestructure.Repositories.Usurio
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Usuarios?> ObtenerPorUsuarioAsync(string usuarioLogin)
        {
            return await _context.Usuarios
                .Include(u => u.UsuarioRoles.Where(ur => ur.Estado == 1))
                    .ThenInclude(ur => ur.Rol)
                .FirstOrDefaultAsync(u => u.UsuarioLogin == usuarioLogin);
        }

        public async Task<Usuarios?> ObtenerPorIdAsync(int codUsuario)
        {
            return await _context.Usuarios
                .Include(u => u.UsuarioRoles)
                .ThenInclude(ur => ur.Rol)
                .FirstOrDefaultAsync(u => u.CodUsuario == codUsuario);
        }
    }
}