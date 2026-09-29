using internal_search.Domain.Entities;
using internal_search.Domain.Interfaces.Buscador.masivo;
using Microsoft.EntityFrameworkCore;

namespace internal_search_backend.Infraestructure.Repositories.Buscador.masiva
{
    public class HistorialRepository : IHistorialRepository
    {
        private readonly AppDbContext _db;
        public HistorialRepository(AppDbContext db) => _db = db;

        public async Task AgregarAsync(HistorialDescarga item)
        {
            _db.HistorialDescargas.Add(item);
            await _db.SaveChangesAsync();
        }

        // Todos los registros del COD_USUARIO, más reciente primero
        public Task<List<HistorialDescarga>> ObtenerPorUsuarioAsync(int codUsuario) =>
            _db.HistorialDescargas
               .AsNoTracking()
               .Where(h => h.CodUsuario == codUsuario)
               .OrderByDescending(h => h.FechaCreo)
               .ThenByDescending(h => h.CodHistorial)
               .ToListAsync();

        public async Task EliminarAsync(IEnumerable<HistorialDescarga> items)
        {
            _db.HistorialDescargas.RemoveRange(items);
            await _db.SaveChangesAsync();
        }
    }
}