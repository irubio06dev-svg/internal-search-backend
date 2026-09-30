using internal_search.Business.Interfaces;
using internal_search.Domain.Constants;
using internal_search_backend.Business.Services.Buscador.individual;
using internal_search_backend.Business.Services.Buscador.masivos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.RegularExpressions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace internal_search_backend.Controllers.Buscador.masivo
{


    [ApiController]
    [Route("api/buscador")]
    [Authorize]
    public class BuscadorMasivoController : ControllerBase
    {
        private readonly IBuscadorMasivoService _masivoService;
        private readonly IBuscadorMasivoExcelService _excelService;

        public BuscadorMasivoController(IBuscadorMasivoService masivoService, IBuscadorMasivoExcelService excelService)
        {
            _masivoService = masivoService;
            _excelService = excelService;

        }

        //[HttpPost("masivo")]
        //[Consumes("multipart/form-data")]
        //[RequestSizeLimit(5_000_000)]
        //public async Task<IActionResult> BuscarMasivo(
        //    IFormFile archivo, [FromForm] string? periodo, CancellationToken ct)
        //{
        //    if (archivo == null || archivo.Length == 0)
        //        return BadRequest("Sube un archivo .txt o .csv.");

        //    var ext = Path.GetExtension(archivo.FileName).ToLowerInvariant();
        //    if (ext is not (".txt" or ".csv"))
        //        return BadRequest("Solo se permiten archivos .txt o .csv.");

        //    if (!string.IsNullOrEmpty(periodo) && !Regex.IsMatch(periodo, @"^\d{6}$"))
        //        return BadRequest("El periodo debe tener formato YYYYMM.");

        //    try
        //    {
        //        using var stream = archivo.OpenReadStream();
        //        return Ok(await _masivoService.BuscarMasivoAsync(stream, ct));
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}


        [HttpPost("masivo/exportar")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(5_000_000)]
        public async Task<IActionResult> ExportarMasivo(
            IFormFile archivo,
            [FromForm] string[] secciones,
            CancellationToken ct)
        {
            if (archivo == null || archivo.Length == 0)
                return BadRequest("Sube un archivo .txt o .csv.");

            var ext = Path.GetExtension(archivo.FileName).ToLowerInvariant();

            if (ext is not (".txt" or ".csv"))
                return BadRequest("Solo se permiten archivos .txt o .csv.");

            if (secciones == null || secciones.Length == 0)
                return BadRequest("Selecciona al menos una sección.");

            // Normaliza y valida contra la lista permitida
            var seleccionadas = secciones
                .Select(s => s.Trim().ToLowerInvariant())
                .Distinct()
                .ToHashSet();

            var invalidas = seleccionadas
                .Where(s => !SeccionesMasivo.Todas.Contains(s))
                .ToList();

            if (invalidas.Count > 0)
                return BadRequest($"Secciones no válidas: {string.Join(", ", invalidas)}");

            try
            {
                using var stream = archivo.OpenReadStream();

                var resultado = await _masivoService.BuscarMasivoAsync(
                    stream,
                    seleccionadas,   // nuevo
                    ct);

                var excel = _excelService.GenerarExcel(
                    resultado,
                    seleccionadas);  // nuevo

                return File(
                    excel,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"Resultado_Masivo_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}