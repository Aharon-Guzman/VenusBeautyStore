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
        public DbSet<ReservaProducto> ReservaProducto { get; set; }
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
        }
    }

}





//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;
//using VenusBeauty.DAL.Entities;

//public class VenusBeautyContext : IdentityDbContext<IdentityUser>
//{
//    public VenusBeautyContext(DbContextOptions<VenusBeautyContext> options) : base(options) { }

//    public DbSet<Cliente> Clientes { get; set; }
//    public DbSet<Servicio> Servicios { get; set; }
//    public DbSet<Cita> Citas { get; set; }
//    public DbSet<DetalleCita> DetalleCitas { get; set; }
//    public DbSet<ReservaProducto> ReservaProductos { get; set; }

//    public DbSet<Producto> Producto { get; set; } // ✅ Cambiado

//    protected override void OnModelCreating(ModelBuilder builder)
//    {
//        base.OnModelCreating(builder);

//        builder.Entity<Cliente>()
//            .HasOne(c => c.User)
//            .WithOne()
//            .HasForeignKey<Cliente>(c => c.UserId)
//            .OnDelete(DeleteBehavior.Cascade);
//    }
//}
