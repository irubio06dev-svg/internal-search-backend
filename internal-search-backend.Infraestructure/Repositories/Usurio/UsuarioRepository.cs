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

        public async Task<Usuarios?> ObtenerActivoPorLoginOCorreoAsync(string identificador)
        {
            var porLogin = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.UsuarioLogin == identificador && u.Estado == 1);

            if (porLogin != null)
                return porLogin;

            var porCorreo = await _context.Usuarios
                .Where(u => u.Correo == identificador && u.Estado == 1)
                .Take(2)
                .ToListAsync();

            // Si el correo lo comparten varias cuentas no se puede saber a cuál enviar
            return porCorreo.Count == 1 ? porCorreo[0] : null;
        }

        public Task<bool> ExisteLoginAsync(string login) =>
            _context.Usuarios.AnyAsync(u => u.UsuarioLogin == login);

        public Task<bool> ExisteCorreoAsync(string correo) =>
            _context.Usuarios.AnyAsync(u => u.Correo == correo);

        public Task<List<int>> ObtenerRolesActivosAsync(IEnumerable<int> codRoles)
        {
            var ids = codRoles.Distinct().ToList();
            return _context.Roles
                .Where(r => ids.Contains(r.CodRol) && r.Estado == 1)
                .Select(r => r.CodRol)
                .ToListAsync();
        }

        public async Task<int> CrearAsync(Usuarios usuario, IEnumerable<int> codRoles)
        {
            foreach (var codRol in codRoles.Distinct())
            {
                usuario.UsuarioRoles.Add(new UsuarioRol
                {
                    CodRol = codRol,
                    Estado = 1,
                    UsuCreo = usuario.UsuCreo,
                    FechaCreo = usuario.FechaCreo
                });
            }

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario.CodUsuario;
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