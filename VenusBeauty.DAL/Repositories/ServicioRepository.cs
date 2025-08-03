using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using VenusBeauty.DAL.Context;
using VenusBeauty.DAL.Entities;

namespace VenusBeauty.DAL.Repositories
{
    public class ServicioRepository : IServicioRepository
    {
        private readonly VenusBeautyContext _context;

        public ServicioRepository(VenusBeautyContext context)
        {
            _context = context;
        }

        public async Task<List<Servicio>> GetAllAsync()
        {
            return await _context.Servicios.ToListAsync();
        }

        public async Task<Servicio?> GetByIdAsync(int id)
        {
            return await _context.Servicios.FindAsync(id);
        }

        public async Task AddAsync(Servicio servicio)
        {
            await _context.Servicios.AddAsync(servicio);
        }

        public async Task UpdateAsync(Servicio servicio)
        {
            _context.Servicios.Update(servicio);
        }

        public async Task DeleteAsync(Servicio servicio)
        {
            _context.Servicios.Remove(servicio);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<bool> TieneCitasAsync(int idServicio)
        {
            return await _context.DetalleCitas.AnyAsync(dc => dc.IdServicio == idServicio);
        }

    }
}
