using internal_search.Domain.Interfaces.Buscador.masivo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace internal_search_backend.Controllers.Buscador.masivo
{
    [Authorize]
    [Route("api/historial")]
    [ApiController]
    public class HistorialController : ControllerBase
    {
        private const string ExcelMime =
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        private readonly IHistorialService _historialService;

        public HistorialController(IHistorialService historialService)
        {
            _historialService = historialService;
        }

        private int CodUsuario =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        // GET api/historial -> historial del COD_USUARIO logueado (máx. 5)
        [HttpGet]
        public async Task<IActionResult> Historial() =>
            Ok(await _historialService.ListarAsync(CodUsuario));

        // GET api/historial/5/descargar -> archivo del historial (solo si es de ese COD_USUARIO)
        [HttpGet("usuario/{codUsuario:int}")]
        public async Task<IActionResult> HistorialPorUsuario(int codUsuario) =>
            Ok(await _historialService.ListarAsync(codUsuario));
    }
}