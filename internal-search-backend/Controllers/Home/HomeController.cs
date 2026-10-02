using internal_search_backend.Business.Services.Menu;
using internal_search_backend.Business.Services.Usuario;
using internal_search_backend.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace internal_search_backend.Controllers.Home
{
    [Route("system/home")]
    [ApiController]
    public class HomeController : ControllerBase
    {

        private readonly IMenuService _menuService;
        private readonly IUsuarioService _usuarioService;

        public HomeController(IMenuService menuService, IUsuarioService usuarioService)
        {
            _menuService = menuService;
            _usuarioService = usuarioService;
        }

        [Authorize]
        [HttpGet("get-routes")]
        public async Task<IActionResult> GetRoutes([FromQuery] int cod_role)
        {
            // Antes se confiaba en el rol que mandaba el cliente: solo se aceptan roles del propio usuario
            var usuario = await _usuarioService.ObtenerUsuarioSesionAsync(User.CodUsuario());
            if (usuario == null || !usuario.Roles.Any(r => r.CodigoRol == cod_role))
                return Forbid();

            var menus = await _menuService
                .ObtenerMenusPorRolAsync(cod_role);

            return Ok(menus);
        }
    }
}
