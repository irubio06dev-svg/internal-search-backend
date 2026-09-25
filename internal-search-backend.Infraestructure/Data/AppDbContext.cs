using internal_search.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuarios> Usuarios { get; set; }
    public DbSet<UsuarioRol> UsuarioRoles { get; set; }
    public DbSet<Rol> Roles { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<RolMenu> RolMenus { get; set; }


    public DbSet<Calificacion> Calificaciones { get; set; }
    public DbSet<Deuda> Deudas { get; set; }
    public DbSet<Movil> Movil { get; set; }
    public DbSet<LineaCredito> LineaCreditos { get; set; }

    public DbSet<Sueldo> Sueldos { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Relaciones existentes
        modelBuilder.Entity<UsuarioRol>()
            .HasOne(ur => ur.Usuario)
            .WithMany(u => u.UsuarioRoles)
            .HasForeignKey(ur => ur.CodUsuario);

        modelBuilder.Entity<UsuarioRol>()
            .HasOne(ur => ur.Rol)
            .WithMany(r => r.UsuarioRoles)
            .HasForeignKey(ur => ur.CodRol);

        // Tablas de consulta sin PK
        modelBuilder.Entity<Calificacion>()
            .HasNoKey();

        modelBuilder.Entity<Deuda>()
            .HasNoKey();

        modelBuilder.Entity<LineaCredito>()
            .HasNoKey();

        modelBuilder.Entity<Movil>()
            .HasNoKey();


        modelBuilder.Entity<Sueldo>()
            .HasNoKey();
    }



}