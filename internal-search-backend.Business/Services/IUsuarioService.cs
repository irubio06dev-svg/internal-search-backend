using internal_search.Domain.DTOs;
using internal_search.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace internal_search_backend.Business.Services
{
    //conecta con domain
    public interface IUsuarioService
    {
        Task<LoginResponseDto> IniciarSesionAsync(IniciarSessionDto request);

        Task<UsuarioLoginDto?> ObtenerUsuarioSesionAsync(
            int codUsuario);
    }
}
