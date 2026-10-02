using internal_search.Domain.DTOs.Usuario;

namespace internal_search_backend.Business.Services.Usuario
{
    public interface IUsuarioAdminService
    {
        // ArgumentException: datos inválidos (400). InvalidOperationException: duplicado (409).
        Task<CrearUsuarioResponseDto> CrearAsync(CrearUsuarioDto dto, string creadoPor, string? ip);
    }
}
