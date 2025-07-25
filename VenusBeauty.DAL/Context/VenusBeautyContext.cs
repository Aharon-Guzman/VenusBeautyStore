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
    public class VenusBeautyContext : IdentityDbContext<IdentityUser>
    {
        public VenusBeautyContext(DbContextOptions<VenusBeautyContext> options) : base(options) 
        { 
           
        }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<DetalleCita> DetalleCitas { get; set; }
        public DbSet<ReservaProducto> ReservaProductos { get; set; }
    }
}
