using internal_search.Domain.DTOs.buscador.individual;
using internal_search.Domain.Interfaces.Buscador;
using internal_search_backend.Business.Services.Buscador;
using InternalSearchBackend.Domain.DTOs;

namespace InternalSearchBackend.Business.Services
{
    public class BuscadorService : IBuscadorService
    {
        private readonly IBuscadorRepository _repository;

        public BuscadorService(IBuscadorRepository repository)
        {
            _repository = repository;
        }

        public async Task<BuscadorHistorialResponse> BuscarAsync(
            BuscadorHistorialEntrada entrada,
            CancellationToken ct)
        {
            // La validación de longitud/tipo de documento ya la hizo
            // BuscadorHistorialEntrada mediante IValidatableObject.

            var documento = entrada.Documento.Trim();
            var periodo = entrada.Periodo.Trim();

            var respuesta = new BuscadorHistorialResponse
            {
                Documento = documento
            };

            // Ejecutamos las consultas de forma SECUENCIAL
            // porque utilizan el mismo AppDbContext.

            var deudas = await _repository
                .BuscarDeudasAsync(documento, periodo, ct);

            var lineasCredito = await _repository
                .BuscarLineasCreditoAsync(documento, periodo, ct);

            var calificaciones = await _repository
                .BuscarCalificacionesAsync(documento, periodo, ct);

            var sueldos = await _repository
                .BuscarSueldosAsync(documento, ct);

            var moviles = await _repository
                .BuscarMovilesAsync(documento, ct);

            respuesta.Deudas = deudas;
            respuesta.LineasCredito = lineasCredito;
            respuesta.Calificaciones = calificaciones;
            respuesta.Sueldos = sueldos;
            respuesta.Moviles = moviles;

            return respuesta;
        }
    }
}