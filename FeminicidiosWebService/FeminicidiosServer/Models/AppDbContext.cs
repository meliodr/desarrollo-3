using Microsoft.EntityFrameworkCore;

namespace FeminicidiosServer.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Provincia> Provincias { get; set; }
        public DbSet<Victima> Victimas { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Indice unico en nombre de provincia
            modelBuilder.Entity<Provincia>()
                .HasIndex(p => p.Nombre)
                .IsUnique();

            // Relacion Provincia -> Victimas
            modelBuilder.Entity<Victima>()
                .HasOne(v => v.Provincia)
                .WithMany(p => p.Victimas)
                .HasForeignKey(v => v.IdProvincia);
        }
    }
}
