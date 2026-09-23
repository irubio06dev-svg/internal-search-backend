using internal_search.Domain.DTOs.Buscador;
using internal_search.Domain.Interfaces.Buscador;
using System;
using System.Linq;

namespace internal_search_backend.Business.Services.Buscador
{
    public class BuscadorService : IBuscadorService
    {
        private readonly IBuscadorRepository _buscadorRepository;

        public BuscadorService(IBuscadorRepository buscadorRepository)
        {
            _buscadorRepository = buscadorRepository;
        }

        public async Task<BuscadorResponse> BuscarAsync(BuscadorEntrada request)
        {
            if (request == null)
                throw new ArgumentException("La solicitud de búsqueda es obligatoria.");

            // Normalizamos espacios en blanco
            request.Documento = request.Documento?.Trim();
            request.ApePat = request.ApePat?.Trim();
            request.ApeMat = request.ApeMat?.Trim();
            request.Prenombres = request.Prenombres?.Trim();
            request.Telefono = request.Telefono?.Trim();

            // ==========================================
            // AL MENOS UN CRITERIO DE BÚSQUEDA
            // ==========================================
            bool tieneAlgunCriterio =
                !string.IsNullOrWhiteSpace(request.Documento) ||
                !string.IsNullOrWhiteSpace(request.ApePat) ||
                !string.IsNullOrWhiteSpace(request.ApeMat) ||
                !string.IsNullOrWhiteSpace(request.Prenombres) ||
                !string.IsNullOrWhiteSpace(request.Telefono);

            if (!tieneAlgunCriterio)
            {
                throw new ArgumentException(
                    "Debe ingresar al menos un criterio de búsqueda."
                );
            }

            // ==========================================
            // VALIDAR TIPO DE DOCUMENTO (solo si viene Documento)
            // ==========================================
            if (!string.IsNullOrWhiteSpace(request.Documento))
            {
                var tipoDocumento = request.TipoDocumento?.Trim().ToUpper() ?? "";

                if (string.IsNullOrWhiteSpace(tipoDocumento))
                {
                    throw new ArgumentException(
                        "Debe seleccionar un tipo de documento cuando busca por documento."
                    );
                }

                switch (tipoDocumento)
                {
                    case "DNI":
                        if (request.Documento.Length != 8 || !request.Documento.All(char.IsDigit))
                            throw new ArgumentException("El DNI debe contener exactamente 8 dígitos.");
                        break;

                    case "RUC":
                        if (request.Documento.Length != 11 || !request.Documento.All(char.IsDigit))
                            throw new ArgumentException("El RUC debe contener exactamente 11 dígitos.");
                        break;

                    case "CE":
                        if (request.Documento.Length < 9 || request.Documento.Length > 12)
                            throw new ArgumentException("El Carnet de Extranjería debe tener entre 9 y 12 caracteres.");
                        break;

                    case "PASAPORTE":
                        if (request.Documento.Length < 6 || request.Documento.Length > 12)
                            throw new ArgumentException("El pasaporte debe tener entre 6 y 12 caracteres.");
                        break;

                    default:
                        throw new ArgumentException("El tipo de documento no es válido.");
                }

                request.TipoDocumento = tipoDocumento; // normalizado, para que el repositorio lo reciba consistente
            }

            // ==========================================
            // EJECUTAR BÚSQUEDA
            // ==========================================
            return await _buscadorRepository.BuscarAsync(request);
        }
    }
}