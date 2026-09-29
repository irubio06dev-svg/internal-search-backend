using internal_search.Domain.DTOs.buscador.persona;

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace internal_search.Business.Interfaces
{
    public interface IBuscadorMasivoService
    {
        Task<BuscadorMasivoResponse> BuscarMasivoAsync(
            Stream archivo, HashSet<string> secciones, CancellationToken ct);
    }
}
