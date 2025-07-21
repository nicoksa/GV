using GV.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;


namespace GV.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Propiedad> Propiedades { get; set; }
        public DbSet<PropiedadCampo> PropiedadesCampo { get; set; }
        public DbSet<PropiedadUrbana> PropiedadesUrbanas { get; set; }
        public DbSet<Imagen> Imagenes { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Propiedad>()
                
                .HasDiscriminator<string>("Division")
                .HasValue<PropiedadCampo>("Campo")
                .HasValue<PropiedadUrbana>("Urbano");
       

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.EsAdmin).HasDefaultValue(false);
                entity.Property(u => u.FechaCreacion).HasDefaultValueSql("GETDATE()");
                entity.Property(u => u.FechaActualizacion).HasDefaultValueSql("GETDATE()");
            });
        }
    }
}
