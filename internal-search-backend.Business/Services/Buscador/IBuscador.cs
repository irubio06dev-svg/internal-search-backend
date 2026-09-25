using internal_search.Domain.DTOs.buscador.individual;
using InternalSearchBackend.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace internal_search_backend.Business.Services.Buscador
{
    public interface IBuscadorService
    {
        Task<BuscadorHistorialResponse> BuscarAsync(BuscadorHistorialEntrada entrada, CancellationToken ct);
    }
}
