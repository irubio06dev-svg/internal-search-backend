using internal_search.Domain.DTOs.Menu;
using System;
using System.Collections.Generic;
using System.Text;

namespace internal_search.Domain.Interfaces.Menu
{
    public interface IMenuRepository
    {
        Task<List<MenuDto>> ObtenerMenusPorRolAsync(int codRol);
    }
}
