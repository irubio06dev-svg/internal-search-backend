using internal_search.Domain.DTOs.Buscador;
using System;
using System.Collections.Generic;
using System.Text;

namespace internal_search_backend.Business.Services.Buscador
{
    public interface IBuscadorService
    {
        //Task<BuscadorResponse> BuscarAsync(string texto);

        Task<BuscadorResponse> BuscarAsync(BuscadorEntrada request);
    }
}
