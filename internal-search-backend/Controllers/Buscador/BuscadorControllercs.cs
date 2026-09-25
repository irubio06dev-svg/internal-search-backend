using internal_search.Domain.DTOs.buscador.individual;
using internal_search_backend.Business.Services.Buscador;
using InternalSearchBackend.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace internal_search_backend.Controllers.Buscador
{
    [ApiController]
    [Route("api/buscador")]
    [Authorize]
    public class BuscadorController : ControllerBase
    {
        private readonly IBuscadorService _buscadorService;

        public BuscadorController(IBuscadorService buscadorService)
        {
            _buscadorService = buscadorService;
        }

        /// <summary>
        /// Busca por Documento + TipoDocumento (DNI/RUC) + Periodo (YYYYMM).
        /// La validación de formato la hace BuscadorHistorialEntrada vía IValidatableObject;
        /// con [ApiController], ASP.NET Core devuelve 400 automáticamente si el ModelState
        /// es inválido, así que si este método se ejecuta, la entrada ya es válida.
        /// </summary>
        [HttpPost("buscar")]
        [ProducesResponseType(typeof(BuscadorHistorialResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Buscar([FromBody] BuscadorHistorialEntrada entrada, CancellationToken ct)
        {
            var resultado = await _buscadorService.BuscarAsync(entrada, ct);
            return Ok(resultado);
        }
    }
}
