using Microsoft.EntityFrameworkCore;
using internal_search.Domain.Interfaces;
using internal_search.Domain.Entities;

namespace internal_search.Infrastructure.Repositories
{
    public class UsuarioRepository : Domain.Interfaces.IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> ObtenerPorUsuarioAsync(string usuarioLogin)
        {
            return await _context.Usuarios
                .Include(u => u.UsuarioRoles.Where(ur => ur.Estado == 1))
                    .ThenInclude(ur => ur.Rol)
                .FirstOrDefaultAsync(u => u.UsuarioLogin == usuarioLogin);
        }

        public async Task<Usuario?> ObtenerPorIdAsync(int codUsuario)
        {
            return await _context.Usuarios
                .Include(u => u.UsuarioRoles)
                .ThenInclude(ur => ur.Rol)
                .FirstOrDefaultAsync(u => u.CodUsuario == codUsuario);
        }
    }
}