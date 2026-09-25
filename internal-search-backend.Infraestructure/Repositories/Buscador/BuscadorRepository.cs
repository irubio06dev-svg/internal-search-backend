using internal_search.Domain.Entities;
using internal_search.Domain.Interfaces.Buscador;
using Microsoft.EntityFrameworkCore;

namespace InternalSearchBackend.Infraestructure.Repositories
{
    public class BuscadorRepository : IBuscadorRepository
    {
        private readonly AppDbContext _context;

        public BuscadorRepository(AppDbContext context)
        {
            _context = context;
        }

        // ---------- RRCC: documento + periodo ----------

        public async Task<List<Deuda>> BuscarDeudasAsync(
            string documento,
            string periodo,
            CancellationToken ct)
        {
            return await _context.Deudas
                .AsNoTracking()
                .Where(x => x.Documento == documento && x.Periodo == periodo)
                .OrderBy(x => x.Documento)
                .ToListAsync(ct);
        }

        public async Task<List<LineaCredito>> BuscarLineasCreditoAsync(
            string documento,
            string periodo,
            CancellationToken ct)
        {
            return await _context.LineaCreditos
                .AsNoTracking()
                .Where(x => x.Documento == documento && x.Periodo == periodo)
                .OrderBy(x => x.Documento)
                .ToListAsync(ct);
        }

        public async Task<List<Calificacion>> BuscarCalificacionesAsync(
            string documento,
            string periodo,
            CancellationToken ct)
        {
            return await _context.Calificaciones
                .AsNoTracking()
                .Where(x => x.Documento == documento && x.Periodo == periodo)
                .OrderBy(x => x.Documento)
                .ToListAsync(ct);
        }

        // ---------- Operador: solo documento ----------

        public async Task<List<Sueldo>> BuscarSueldosAsync(
            string documento,
            CancellationToken ct)
        {
            return await _context.Sueldos
                .AsNoTracking()
                .Where(x => x.Documento == documento)
                .OrderBy(x => x.Documento)
                .ToListAsync(ct);
        }

        public async Task<List<Movil>> BuscarMovilesAsync(
            string documento,
            CancellationToken ct)
        {
            return await _context.Movil
                .AsNoTracking()
                .Where(x => x.Documento == documento)
                .OrderBy(x => x.Documento)
                .ToListAsync(ct);
        }
    }
}
