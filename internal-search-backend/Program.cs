using internal_search.Domain.Interfaces.Auth;
using internal_search.Domain.Interfaces.Buscador.empresas.individual;
using internal_search.Domain.Interfaces.Buscador.empresas.masivo;
using internal_search.Domain.Interfaces.Buscador.personas.individual;
using internal_search.Domain.Interfaces.Buscador.personas.masivo;
using internal_search.Domain.Interfaces.Menu;
using internal_search.Domain.Interfaces.Usuario;
using internal_search_backend.Business.Services.Buscador.empresa.individual;
using internal_search_backend.Business.Services.Buscador.empresa.masivo;
using internal_search_backend.Business.Services.Buscador.empresas.masivo;
using internal_search_backend.Business.Services.Buscador.personas.individual;
using internal_search_backend.Business.Services.Buscador.personas.masivos;
using internal_search_backend.Business.Services.Menu;
using internal_search_backend.Business.Services.Usuario;
using internal_search_backend.Infraestructure.Repositories.Buscador.empresas.individual;
using internal_search_backend.Infraestructure.Repositories.Buscador.empresas.masiva;
using internal_search_backend.Infraestructure.Repositories.Buscador.personas.individual;
using internal_search_backend.Infraestructure.Repositories.Buscador.personas.masiva;
using internal_search_backend.Infraestructure.Repositories.Menu;
using internal_search_backend.Infraestructure.Repositories.Usurio;
using internal_search_backend.Infraestructure.Security;
using internal_search_backend.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using internal_search.Domain.Interfaces.Auditoria;
using internal_search.Domain.Interfaces.Tokens;
using internal_search_backend.Business.Services.Auditoria;
using internal_search_backend.Business.Services.Tokens;
using internal_search_backend.Infraestructure.Repositories.Auditoria;
using internal_search_backend.Infraestructure.Repositories.Tokens;
using internal_search_backend.Security;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using internal_search.Domain.Configuration;
using internal_search.Domain.Interfaces.Notificaciones;
using internal_search_backend.Infraestructure.Notificaciones;


var builder = WebApplication.CreateBuilder(args);

var jwtKey = builder.Configuration["Jwt:Key"]
             ?? throw new InvalidOperationException(
                 "No se configuró Jwt:Key");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException(
                    "No se configuró Jwt:Issuer");

var jwtAudience = builder.Configuration["Jwt:Audience"]
                  ?? throw new InvalidOperationException(
                      "No se configuró Jwt:Audience");

// Authentication JWT
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            ),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        // Estado y roles se verifican en base de datos en cada petición: un usuario desactivado
        // o con el rol retirado pierde acceso al instante, sin esperar a que venza el token.
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var principal = context.Principal;
                var identity = principal?.Identity as ClaimsIdentity;

                if (identity == null ||
                    !int.TryParse(principal!.FindFirstValue(ClaimTypes.NameIdentifier), out var codUsuario))
                {
                    context.Fail("Token sin usuario.");
                    return;
                }

                var repo = context.HttpContext.RequestServices.GetRequiredService<IUsuarioRepository>();
                var sesion = await repo.ObtenerContextoSesionAsync(codUsuario);

                if (sesion is not { Activo: true })
                {
                    context.Fail("Usuario inactivo o inexistente.");
                    return;
                }

                foreach (var claim in identity.Claims
                    .Where(c => c.Type == ClaimTypes.Role || c.Type == "role").ToList())
                {
                    identity.RemoveClaim(claim);
                }

                foreach (var rol in sesion.Roles)
                    identity.AddClaim(new Claim(ClaimTypes.Role, rol));
            }
        };
    });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Dependency Injection
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IContrasenaRepository, ContrasenaService>();
builder.Services.AddScoped<IJwtRepository, JwtRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// Alta de usuarios y recuperación de contraseña
builder.Services.AddScoped<IPasswordResetRepository, PasswordResetRepository>();
builder.Services.AddScoped<IRecuperacionClaveService, RecuperacionClaveService>();
builder.Services.AddScoped<IUsuarioAdminService, UsuarioAdminService>();

// Tokens de consulta y auditoría
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();
builder.Services.AddScoped<IAuditoriaService, AuditoriaService>();

builder.Services.AddSingleton(
    builder.Configuration.GetSection("Recuperacion").Get<RecuperacionClaveOptions>()
    ?? new RecuperacionClaveOptions());

var emailOptions = builder.Configuration.GetSection("Email").Get<EmailOptions>() ?? new EmailOptions();
if (!string.IsNullOrWhiteSpace(emailOptions.Host))
{
    builder.Services.AddSingleton(emailOptions);
    builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
}
else
{
    var esDesarrollo = builder.Environment.IsDevelopment();
    builder.Services.AddScoped<IEmailSender>(sp =>
        new LogEmailSender(sp.GetRequiredService<ILogger<LogEmailSender>>(), esDesarrollo));
}

builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IMenuRepository, MenuRepository>();

builder.Services.AddScoped<IBuscadorRepository, IndividualRepository>();

builder.Services.AddScoped<IBuscadorMasivoRepository, BuscadorMasivoRepository>();
builder.Services.AddScoped<IMasivoExcelService, BuscadorMasivoService>();

builder.Services.AddScoped<IBuscadorMasivoExcelService,BuscadorMasivoExcelService>();

builder.Services.AddScoped<IHistorialRepository, HistorialRepository>();
builder.Services.AddScoped<IHistorialService, HistorialService>();

// Registro de repositorios y servicios de empresa (individual y masivo)
builder.Services.AddScoped<IEmpresaIndividualRepository, BuscadorIndividualRepository>();
builder.Services.AddScoped<internal_search_backend.Business.Services.Buscador.empresa.individual.IIndividualService, internal_search_backend.Business.Services.Buscador.empresa.individual.IndividualService>();

builder.Services.AddScoped<IBuscadorEmpresaMasivoRepository, BuscadorEmpresaMasivoRepository>();
builder.Services.AddScoped<IBuscadorEmpresaMasivoService, BuscadorEmpresaMasivoService>();

// Authorization
builder.Services.AddAuthorization(options =>
{
    // Administración de cuentas, tokens y auditoría
    options.AddPolicy("AdminGeneral", policy =>
        policy.RequireAssertion(ctx => ctx.User.EsAdminGeneral()));
});

// Límite de intentos por IP en login y recuperación (frena fuerza bruta y abuso del envío de correos)
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("auth", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "desconocida",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1)
            }));
});

// Controllers
builder.Services.AddControllers(options =>
    options.Filters.Add<ManejadorExcepcionesFilter>());
builder.Services.AddProblemDetails();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Internal Search API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingresa el token JWT así: Bearer {tu token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200", "https://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Errores no controlados: 500 genérico, sin trazas ni mensajes internos
    app.UseExceptionHandler();
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.UseRateLimiter();

// JWT
app.UseAuthentication();

// Authorization
app.UseAuthorization();

app.MapControllers();

app.Run();