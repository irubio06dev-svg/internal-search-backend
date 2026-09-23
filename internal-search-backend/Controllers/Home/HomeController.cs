using internal_search_backend.Business.Services.Menu;
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

        public HomeController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [Authorize]
        [HttpGet("get-routes")]
        public async Task<IActionResult> GetRoutes([FromQuery] int cod_role)
        {
            var menus = await _menuService
                .ObtenerMenusPorRolAsync(cod_role);

            return Ok(menus);
        }
    }
}
