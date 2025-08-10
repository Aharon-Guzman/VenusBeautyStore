using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VenusBeauty.DAL.Entities;

namespace VenusBeauty.DAL.Context
{
    public class VenusBeautyContext : IdentityDbContext<ApplicationUser>
    {
        public VenusBeautyContext(DbContextOptions<VenusBeautyContext> options) : base(options)
        {

        }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<DetalleCita> DetalleCitas { get; set; }
        public DbSet<ReservaProducto> ReservaProductos { get; set; }
        public DbSet<Producto> Producto { get; set; }
        public DbSet<Trabajador> Trabajadores { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Cliente>()
                .HasOne(c => c.User)
                .WithOne()
                .HasForeignKey<Cliente>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Trabajador>()
                  .HasOne(t => t.User)
                  .WithOne()
                  .HasForeignKey<Trabajador>(t => t.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
                        builder.Entity<Cita>()
                  .HasOne(c => c.Trabajador)
                  .WithMany()                    // sin colección inversa
                  .HasForeignKey(c => c.IdUsuario)
                  .HasPrincipalKey("UserId");    // ← string, NO genérico
            builder.Entity<Cliente>()
    .HasOne(c => c.User)
    .WithOne()
    .HasForeignKey<Cliente>(c => c.UserId)
    .OnDelete(DeleteBehavior.Cascade);

            // Trabajador ↔ AspNetUsers (1:1) — cascade como ya lo tienes
            builder.Entity<Trabajador>()
                .HasOne(t => t.User)
                .WithOne()
                .HasForeignKey<Trabajador>(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1) Clave alterna en Trabajador por UserId (única)
            builder.Entity<Trabajador>()
                .HasAlternateKey(t => t.UserId);

            // 2) Relación Cita ► Trabajador por UserId (RESTRICT) usando la sobrecarga con string
            builder.Entity<Cita>()
                .HasOne(c => c.Trabajador)
                .WithMany()
                .HasForeignKey(c => c.IdUsuario)     // FK en Cita (string)
                .HasPrincipalKey("UserId")           // <-- usar string, no genérico
                .OnDelete(DeleteBehavior.Restrict);  // no borrar citas al borrar estilista
            // Cita → Cliente — CASCADE (tu BD actual borra citas al borrar cliente)
            builder.Entity<Cita>()
                .HasOne(c => c.Cliente)
                .WithMany()
                .HasForeignKey(c => c.IdCliente)
                .OnDelete(DeleteBehavior.Cascade);

            // Cita → DetalleCitas — CASCADE
            builder.Entity<Cita>()
                .HasMany(c => c.DetalleCitas)
                .WithOne(d => d.Cita)
                .HasForeignKey(d => d.IdCita)
                .OnDelete(DeleteBehavior.Cascade);

            // Cita → ReservaProductos — CASCADE
            builder.Entity<Cita>()
                .HasMany(c => c.ReservaProductos)
                .WithOne(rp => rp.Cita)
                .HasForeignKey(rp => rp.IdCita)
                .OnDelete(DeleteBehavior.Cascade);

            // ReservaProducto → Producto — CASCADE (tal como está en tu BD actual)
            builder.Entity<ReservaProducto>()
                .HasOne(rp => rp.Producto)
                .WithMany()
                .HasForeignKey(rp => rp.IdProducto)
                .OnDelete(DeleteBehavior.Cascade);

            // DetalleCita → Servicio — CASCADE (tal como está en tu BD actual)
            builder.Entity<DetalleCita>()
                .HasOne(d => d.Servicio)
                .WithMany()
                .HasForeignKey(d => d.IdServicio)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}

