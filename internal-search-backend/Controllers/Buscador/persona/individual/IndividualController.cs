using internal_search.Domain.DTOs.buscador.persona.individual;
using internal_search.Domain.DTOs.buscador.persona.masivos;
using internal_search.Domain.Entities;
using internal_search_backend.Business.Services.Buscador.personas.individual;
using internal_search_backend.Business.Services.Tokens;
using internal_search_backend.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace internal_search_backend.Controllers.Buscador.persona.individual
{
    [ApiController]
    [Route("api/buscador")]
    [Authorize]
    public class BuscadorController : ControllerBase
    {
        private const int CostoConsulta = 1;

        private readonly IIndividualService _buscadorService;
        private readonly ITokenService _tokens;

        public BuscadorController(IIndividualService buscadorService, ITokenService tokens)
        {
            _buscadorService = buscadorService;
            _tokens = tokens;
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
            var resultado = await _tokens.EjecutarAsync(
                this.Contexto(), CostoConsulta, "BUSQUEDA_PERSONA",
                $"{entrada.TipoDocumento} {entrada.Documento}",
                () => _buscadorService.BuscarAsync(entrada, ct));
            return Ok(resultado);
        }

        [HttpPost("buscar-telefono")]
        [ProducesResponseType(typeof(BuscadorTelefonoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BuscarPorTelefono(
        [FromBody] BuscadorPorTelfonoEntrada entrada, CancellationToken ct)
        {
            var resultado = await _tokens.EjecutarAsync(
                this.Contexto(), CostoConsulta, "BUSQUEDA_TELEFONO",
                entrada.Telefono,
                () => _buscadorService.BuscarPorTelefonoAsync(entrada.Telefono, ct));
            return Ok(resultado);
        }
    }
}
