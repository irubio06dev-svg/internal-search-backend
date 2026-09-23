using internal_search.Domain.DTOs.Buscador;
using internal_search_backend.Business.Services.Buscador;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace internal_search_backend.Controllers.Buscador
{
    [ApiController]
    [Route("system/buscador")]
    public class BuscadorController : ControllerBase
    {
        private readonly IBuscadorService _buscadorService;

        public BuscadorController(IBuscadorService buscadorService)
        {
            _buscadorService = buscadorService;
        }


        [Authorize]
        [HttpPost("buscar")]
        public async Task<IActionResult> Buscar(
            [FromBody] BuscadorEntrada request)
        {
            try
            {
                var resultado = await _buscadorService
                    .BuscarAsync(request);

                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    mensaje = ex.Message
                });
            }
        }
    }
}
