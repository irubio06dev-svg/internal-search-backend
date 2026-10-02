using internal_search.Domain.Entities;

namespace internal_search.Domain.Interfaces.Usuario
{
    public interface IPasswordResetRepository
    {
        Task InvalidarPendientesAsync(int codUsuario);
        Task CrearAsync(PasswordResetToken token);

        // Consume el token y cambia la clave de forma atómica. False si el token no es válido, ya se usó o expiró.
        Task<bool> RestablecerAsync(string tokenHash, string nuevaClaveHash, DateTime ahoraUtc);
    }
}
