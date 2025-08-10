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
        public async Task<bool> TieneReservasAsync(int idProducto)
        {
            return await _context.ReservaProductos.AnyAsync(rp => rp.IdProducto == idProducto);
        }
        public async Task ConsumirStockPorCitaAsync(int idCita)
        {
            // total a consumir por producto en esta cita
            var consumos = await _context.ReservaProductos
                .Where(rp => rp.IdCita == idCita)
                .GroupBy(rp => rp.IdProducto)
                .Select(g => new { IdProducto = g.Key, Cantidad = g.Sum(x => x.Cantidad) })
                .ToListAsync();

            if (consumos == null || consumos.Count == 0)
                return; // no hay productos asociados a la cita

            var ids = consumos.Select(c => c.IdProducto).ToList();

            var productos = await _context.Producto
                .Where(p => ids.Contains(p.IdProducto))
                .ToListAsync();

            // valida y descuenta
            foreach (var c in consumos)
            {
                var p = productos.First(x => x.IdProducto == c.IdProducto);
                if (p.Stock < c.Cantidad)
                    throw new InvalidOperationException(
                        $"stock insuficiente para consumir '{p.Nombre}'. físico: {p.Stock}, a consumir: {c.Cantidad}.");

                p.Stock -= c.Cantidad;
            }

            await _context.SaveChangesAsync();
        }
        public async Task<int> GetStockDisponibleAsync(int idProducto)
        {
            var prod = await _context.Producto
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.IdProducto == idProducto && p.Activo);

            if (prod is null)
                throw new InvalidOperationException("El producto no existe o está inactivo.");

            var bloqueado = await _context.ReservaProductos
                .Where(rp => rp.IdProducto == idProducto
                          && rp.Cita != null
                          && (rp.Cita.Estado == EstadoCita.Reservada
                           || rp.Cita.Estado == EstadoCita.Confirmada))
                .SumAsync(rp => (int?)rp.Cantidad) ?? 0;

            return prod.Stock - bloqueado;
        }

    }
}