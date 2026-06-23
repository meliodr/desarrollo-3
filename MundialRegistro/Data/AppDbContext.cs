using Microsoft.EntityFrameworkCore;
using MundialRegistro.Models;

namespace MundialRegistro.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Partido> Partidos => Set<Partido>();
    public DbSet<Registro> Registros => Set<Registro>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Partido>(entity =>
        {
            entity.ToTable("Partidos");
            entity.Property(p => p.Fecha).IsRequired();
            entity.Property(p => p.Hora).HasMaxLength(10).IsRequired();
            entity.Property(p => p.EquipoLocal).HasMaxLength(80).IsRequired();
            entity.Property(p => p.EquipoVisitante).HasMaxLength(80).IsRequired();
            entity.Property(p => p.Estadio).HasMaxLength(120).IsRequired();
        });

        modelBuilder.Entity<Registro>(entity =>
        {
            entity.ToTable("Registros");
            entity.HasIndex(r => r.Codigo).IsUnique();
            entity.HasIndex(r => new { r.PartidoId, r.Asiento }).IsUnique();
            entity.Property(r => r.Codigo).HasMaxLength(30).IsRequired();
            entity.Property(r => r.TipoDocumento).HasMaxLength(30).IsRequired();
            entity.Property(r => r.Documento).HasMaxLength(30).IsRequired();
            entity.Property(r => r.Nombres).HasMaxLength(80).IsRequired();
            entity.Property(r => r.Apellidos).HasMaxLength(80).IsRequired();
            entity.Property(r => r.Sexo).HasMaxLength(20).IsRequired();
            entity.Property(r => r.Asiento).HasMaxLength(10).IsRequired();

            entity.HasOne(r => r.Partido)
                .WithMany(p => p.Registros)
                .HasForeignKey(r => r.PartidoId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
