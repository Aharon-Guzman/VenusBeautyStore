using Microsoft.EntityFrameworkCore;
using VenusBeauty.DAL.Context;
using VenusBeauty.DAL.Entities;

namespace VenusBeauty.DAL.Repositories
{
    public class TrabajadorRepository : ITrabajadorRepository
    {
        private readonly VenusBeautyContext _context;

        public TrabajadorRepository(VenusBeautyContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Trabajador>> GetAllAsync()
        {
            return await _context.Trabajadores.ToListAsync();
        }

        public async Task<Trabajador?> GetByIdAsync(int id)
        {
            return await _context.Trabajadores.FindAsync(id);
        }

        public async Task AddAsync(Trabajador trabajador)
        {
            await _context.Trabajadores.AddAsync(trabajador);
        }

        public async Task UpdateAsync(Trabajador trabajador)
        {
            _context.Trabajadores.Update(trabajador);
        }

        public async Task DeleteAsync(Trabajador trabajador)
        {
            _context.Trabajadores.Remove(trabajador);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

}
