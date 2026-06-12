using DataIntegrationHub.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataIntegrationHub.Data;

public class ApplicationDbContext : DbContext
{
    // El constructor recibe las opciones de configuración (como la Connection String) 
    // y se las pasa a la clase base de EF Core.
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // Los DbSet representan las tablas en la base de datos.
    public DbSet<User> Users { get; set; }
    public DbSet<IntegrationLog> IntegrationLogs { get; set; }

    // Este método se ejecuta cuando EF Core está construyendo el modelo. 
    // Sirve para aplicar reglas adicionales a las tablas (como tamaños máximos o restricciones).
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ejemplo: Asegurar que el nombre de usuario sea único en la base de datos
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
    }
}