using internal_search.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace internal_search_backend.Infraestructure.Security
{
    public class ContrasenaService : IContrasenaRepository
    {
        public bool Verificar(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
