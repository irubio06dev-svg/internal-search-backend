using internal_search.Domain.DTOs.Buscador;
using System;
using System.Collections.Generic;
using System.Text;

namespace internal_search.Domain.Interfaces.Buscador
{
    public interface IBuscadorRepository
    {
        Task<BuscadorResponse> BuscarAsync(BuscadorEntrada request);
    }
}
