using internal_search.Domain.DTOs.Usuario;
using internal_search.Domain.Entities;
using internal_search.Domain.Interfaces.Auth;
using internal_search.Domain.Interfaces.Usuario;
using internal_search_backend.Business.helpers;
using System.Security.Cryptography;

namespace internal_search_backend.Business.Services.Usuario
{
    public class UsuarioAdminService : IUsuarioAdminService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IContrasenaRepository _contrasena;
        private readonly IRecuperacionClaveService _recuperacion;

        public UsuarioAdminService(
            IUsuarioRepository usuarioRepository,
            IContrasenaRepository contrasena,
            IRecuperacionClaveService recuperacion)
        {
            _usuarioRepository = usuarioRepository;
            _contrasena = contrasena;
            _recuperacion = recuperacion;
        }

        public async Task<CrearUsuarioResponseDto> CrearAsync(CrearUsuarioDto dto, string creadoPor, string? ip)
        {
            var login = dto.UsuarioLogin.Trim();
            var correo = dto.Correo.Trim();
            var conClaveInicial = !string.IsNullOrEmpty(dto.Clave);

            if (conClaveInicial)
            {
                var error = ValidadorClave.Validar(dto.Clave);
                if (error != null)
                    throw new ArgumentException(error);
            }

            if (await _usuarioRepository.ExisteLoginAsync(login))
                throw new InvalidOperationException("El usuario ya existe.");

            if (await _usuarioRepository.ExisteCorreoAsync(correo))
                throw new InvalidOperationException("El correo ya está registrado.");

            var rolesValidos = await _usuarioRepository.ObtenerRolesActivosAsync(dto.CodRoles);
            if (rolesValidos.Count != dto.CodRoles.Distinct().Count())
                throw new ArgumentException("Alguno de los roles no existe o está inactivo.");

            // Sin clave inicial se guarda el hash de un valor aleatorio que nadie conoce:
            // la cuenta solo se activa con el enlace de invitación.
            var clave = conClaveInicial
                ? dto.Clave!
                : Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

            var usuario = new Usuarios
            {
                Nombres = dto.Nombres.Trim(),
                ApePat = dto.ApePat.Trim(),
                ApeMat = dto.ApeMat?.Trim(),
                UsuarioLogin = login,
                Clave = _contrasena.Hashear(clave),
                Correo = correo,
                Dni = dto.Dni?.Trim(),
                Telefono = dto.Telefono?.Trim(),
                Estado = 1,
                UsuCreo = creadoPor.Length > 20 ? creadoPor[..20] : creadoPor,
                FechaCreo = DateTime.Now
            };

            var codUsuario = await _usuarioRepository.CrearAsync(usuario, rolesValidos);

            var invitacionEnviada = !conClaveInicial &&
                await _recuperacion.EnviarInvitacionAsync(usuario, ip);

            return new CrearUsuarioResponseDto
            {
                CodUsuario = codUsuario,
                UsuarioLogin = login,
                InvitacionEnviada = invitacionEnviada
            };
        }
    }
}
