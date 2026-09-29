using internal_search.Domain.DTOs.buscador.individual;
using internal_search.Domain.DTOs.buscador.persona;
using internal_search.Domain.Entities;
using InternalSearchBackend.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace internal_search_backend.Business.Services.Buscador.individual
{
    public interface IBuscadorService
    {
        Task<BuscadorHistorialResponse> BuscarAsync(BuscadorHistorialEntrada entrada, CancellationToken ct);

        Task<BuscadorTelefonoResponse> BuscarPorTelefonoAsync(string telefono, CancellationToken ct);
    }
}
