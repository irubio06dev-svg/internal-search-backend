using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using internal_search.Domain.DTOs.Menu;
using internal_search.Domain.Interfaces.Menu;

namespace internal_search_backend.Infraestructure.Repositories.Menu
{
    public class MenuRepository : IMenuRepository
    {
        private readonly AppDbContext _context;

        public MenuRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MenuDto>> ObtenerMenusPorRolAsync(int codRol)
        {
            var planos = await _context.RolMenus
                .Where(rm =>
                    rm.CodRol == codRol &&
                    rm.PuedeVer == 1 &&
                    rm.Estado == 1 &&
                    rm.Menu.Estado == 1
                )
                .OrderBy(rm => rm.Menu.Orden)
                .Select(rm => new MenuDto
                {
                    CodMenu = rm.Menu.CodMenu,
                    CodMenuPadre = rm.Menu.CodMenuPadre,
                    NomMenu = rm.Menu.NomMenu,
                    Ruta = rm.Menu.Ruta,
                    Icono = rm.Menu.Icono,
                    Orden = rm.Menu.Orden,
                    PuedeVer = rm.PuedeVer,
                    PuedeCrear = rm.PuedeCrear,
                    PuedeEditar = rm.PuedeEditar,
                    PuedeEliminar = rm.PuedeEliminar
                })
                .ToListAsync();

            return ConstruirArbol(planos);
        }


        private static List<MenuDto> ConstruirArbol(List<MenuDto> planos)
        {
            var porId = planos.ToDictionary(m => m.CodMenu);
            var raices = new List<MenuDto>();

            foreach (var menu in planos.OrderBy(m => m.Orden))
            {
                if (menu.CodMenuPadre is int padreId && porId.TryGetValue(padreId, out var padre))
                    padre.Children.Add(menu);
                else
                    raices.Add(menu);
            }

            return raices;
        }
    }
}