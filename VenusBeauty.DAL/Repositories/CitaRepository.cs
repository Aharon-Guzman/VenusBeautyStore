using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VenusBeauty.DAL.Context;
using VenusBeauty.DAL.Entities;

namespace VenusBeauty.DAL.Repositories
{
    /// <summary>
    /// Acceso a datos para la entidad Cita.
    /// </summary>
    public class CitaRepository : ICitaRepository
    {
        private readonly VenusBeautyContext _context;

        public CitaRepository(VenusBeautyContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Devuelve una cita completa (servicios y productos) por su Id.
        /// </summary>
        public async Task<Cita?> GetByIdAsync(int id)
        {
            return await _context.Citas
                .Include(c => c.Cliente)                  // cliente asociado
                .Include(c => c.DetalleCitas)             // lista de servicios
                    .ThenInclude(d => d.Servicio)
                .Include(c => c.ReservaProductos)          // lista de productos
                    .ThenInclude(r => r.Producto)
                .FirstOrDefaultAsync(c => c.IdCita == id);
        }

        /// <summary>
        /// Devuelve todas las citas en un rango de fechas.
        /// Si se pasa idUsuario, filtra solo las del estilista indicado.
        /// </summary>
        public async Task<IEnumerable<Cita>> GetAgendaAsync(
            DateTime desde,
            DateTime hasta,
            string? idUsuario = null)
        {
            var query = _context.Citas
                .Include(c => c.DetalleCitas)
                .Include(c => c.ReservaProductos)
                .Where(c => c.FechaHora >= desde && c.FechaHora < hasta);

            if (!string.IsNullOrEmpty(idUsuario))
            {
                query = query.Where(c => c.IdUsuario == idUsuario);
            }

            return await query.ToListAsync();
        }

        public async Task AddAsync(Cita cita)
        {
            await _context.Citas.AddAsync(cita);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Cita cita)
        {
            _context.Citas.Update(cita);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var cita = await _context.Citas.FindAsync(id);
            if (cita is null) return;

            _context.Citas.Remove(cita);
            await _context.SaveChangesAsync();
        }
    }
}
