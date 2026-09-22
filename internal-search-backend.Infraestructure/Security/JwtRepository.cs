using internal_search.Domain.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
namespace internal_search_backend.Infrastructure.Security;

using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using internal_search.Domain.Entities;

public class JwtRepository : IJwtRepository 
{
    private readonly IConfiguration _configuration;

    public JwtRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerarToken(Usuario usuario)
    {
        var jwtKey = _configuration["Jwt:Key"]
                     ?? throw new InvalidOperationException(
                         "No se configuró Jwt:Key");

        var jwtIssuer = _configuration["Jwt:Issuer"]
                        ?? throw new InvalidOperationException(
                            "No se configuró Jwt:Issuer");

        var jwtAudience = _configuration["Jwt:Audience"]
                          ?? throw new InvalidOperationException(
                              "No se configuró Jwt:Audience");

        var expirationMinutes = int.Parse(
            _configuration["Jwt:ExpirationMinutes"] ?? "15");

        var claims = new List<Claim>
        {
            new System.Security.Claims.Claim(
                JwtRegisteredClaimNames.Sub,
                usuario.CodUsuario.ToString()
            ),

            new System.Security.Claims.Claim(
                "UsuarioLogin",
                usuario.UsuarioLogin
            )
        };
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey));

        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        var expiration = DateTime.UtcNow.AddMinutes(
            expirationMinutes);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}