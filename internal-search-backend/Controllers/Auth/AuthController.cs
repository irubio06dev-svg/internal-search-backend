using Azure.Core;
using internal_search.Domain.DTOs.Auth;
using internal_search_backend.Business.Services.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace internal_search_backend.Controllers.Auth
{
    [Route("system/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public AuthController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(
            IniciarSessionDto request)
        {
            try
            {
                var response = await _usuarioService
                    .IniciarSesionAsync(request);

                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
                });
            }
        }

        [Authorize]
        [HttpGet("check-status")]
        public async Task<IActionResult> CheckStatus()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(idClaim, out var codUsuario))
                return Unauthorized(new { message = "Token inválido" });

            var usuario = await _usuarioService
                .ObtenerUsuarioSesionAsync(codUsuario);

            if (usuario == null)
                return Unauthorized(new { message = "Usuario no encontrado" });

            var authorization = Request.Headers["Authorization"].ToString();

            if (!authorization.StartsWith(
                "Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized(new { message = "Token no encontrado" });
            }

            var token = authorization["Bearer ".Length..].Trim();

            if (string.IsNullOrEmpty(token))
                return Unauthorized(new { message = "Token no encontrado" });

            if (!long.TryParse(User.FindFirstValue("exp"), out var exp))
                return Unauthorized(new { message = "Vencimiento inválido" });

            var segundosRestantes =
                exp - DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            if (segundosRestantes <= 0)
                return Unauthorized(new { message = "Sesión expirada" });

            return Ok(new
            {
                estado = 1,
                token,
                tipoToken = "Bearer",
                expira = segundosRestantes,
                usuario
            });
        }
    }
}
