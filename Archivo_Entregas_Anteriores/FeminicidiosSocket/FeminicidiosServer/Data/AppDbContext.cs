using Microsoft.EntityFrameworkCore;
using FeminicidiosServer.Models;

namespace FeminicidiosServer.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Provincia> Provincias { get; set; }
        public DbSet<Victima>   Victimas   { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=feminicidios_server.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
