using internal_search.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace internal_search.Domain.Interfaces.Buscador
{
    public interface IBuscadorRepository
    {
        // RRCC: filtran por documento + periodo
        // RRCC: filtran por documento + periodo
        Task<List<Deuda>> BuscarDeudasAsync(string documento, string periodo, CancellationToken ct);
        Task<List<LineaCredito>> BuscarLineasCreditoAsync(string documento, string periodo, CancellationToken ct);
        Task<List<Calificacion>> BuscarCalificacionesAsync(string documento, string periodo, CancellationToken ct);

        // Operador: filtran solo por documento (no manejan periodo)
        Task<List<Sueldo>> BuscarSueldosAsync(string documento, CancellationToken ct);
        Task<List<Movil>> BuscarMovilesAsync(string documento, CancellationToken ct);
    }
}
