using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VenusBeauty.DAL.Context;
using VenusBeauty.DAL.Entities;
using VenusBeauty.DAL.Repositories;

namespace VenusBeauty.DAL.Repositories
{
    //    public class ProductoRepository : IProductoRepository
    //    {
    //        private readonly VenusBeautyContext _context;

    //        public ProductoRepository(VenusBeautyContext context)
    //        {
    //            _context = context;
    //        }

    //        public async Task<List<Producto>> GetAllAsync()
    //        {
    //            return await _context.Productos.ToListAsync();
    //        }

    //        public async Task<Producto?> GetByIdAsync(int id)
    //        {
    //            return await _context.Productos.FindAsync(id);
    //        }

    //        public async Task AddAsync (Producto producto)
    //        {
    //            await _context.Productos.AddAsync(producto);
    //        }

    //        public async Task UpdateAsync(Producto producto)
    //        {
    //            _context.Productos.Update(producto);
    //        }
    //        public async Task DeleteAsync(Producto producto)
    //        {
    //            _context.Entry(producto).State = EntityState.Detached;
    //        }
    //        public async Task SaveChangesAsync()
    //        {
    //            await _context.SaveChangesAsync();
    //        }
    //    }
    //}
    public class ProductoRepository : IProductoRepository
    {
        private readonly VenusBeautyContext _context;

        public ProductoRepository(VenusBeautyContext context)
        {
            _context = context;
        }

        public async Task<List<Producto>> GetAllAsync()
        {
            return await _context.Producto.ToListAsync(); // ✅
        }

        public async Task<Producto?> GetByIdAsync(int id)
        {
            return await _context.Producto.FindAsync(id); // ✅
        }

        public async Task AddAsync(Producto producto)
        {
            await _context.Producto.AddAsync(producto); // ✅
        }

        public async Task UpdateAsync(Producto producto)
        {
            _context.Producto.Update(producto); // ✅
        }

        public async Task DeleteAsync(Producto producto)
        {
            _context.Entry(producto).State = EntityState.Deleted; // ⚠️ Cambia esto, antes lo habías puesto como Detached
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}