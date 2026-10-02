using internal_search.Domain.DTOs.Usuario;
using internal_search_backend.Business.Services.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace internal_search_backend.Controllers.Usuarios
{
    [Route("system/usuarios")]
    [ApiController]
    [Authorize(Policy = "GestionUsuarios")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioAdminService _usuarioAdminService;

        public UsuariosController(IUsuarioAdminService usuarioAdminService)
        {
            _usuarioAdminService = usuarioAdminService;
        }

        // Alta de usuario. Sin "clave" se envía una invitación al correo para que defina la suya.
        [HttpPost]
        public async Task<IActionResult> Crear(CrearUsuarioDto request)
        {
            var creadoPor = User.FindFirstValue("UsuarioLogin") ?? "SISTEMA";

            try
            {
                var respuesta = await _usuarioAdminService.CrearAsync(
                    request,
                    creadoPor,
                    HttpContext.Connection.RemoteIpAddress?.ToString());

                return StatusCode(StatusCodes.Status201Created, respuesta);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
