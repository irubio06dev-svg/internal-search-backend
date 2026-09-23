using internal_search.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace internal_search.Domain.Interfaces.Auth
{
    public interface IJwtRepository
    {
        string GenerarToken(Usuarios usuario);

    }
}
