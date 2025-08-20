using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VenusBeauty.DAL.Entities;
using VenusBeauty.DAL.Repositories;

namespace VenusBeauty.BLL.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepository;

        public ProductoService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public async Task<IEnumerable<Producto>> ObtenerProductosAsync()
        {
            return await _productoRepository.GetAllAsync();
        }

        public async Task<Producto?> ObtenerPorIdAsync(int id)
        {
            return await _productoRepository.GetByIdAsync(id);
        }

        public async Task<bool> CrearProductoAsync(Producto producto)
        {
            await _productoRepository.AddAsync(producto);
            await _productoRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EditarProductoAsync(int id, Producto producto)
        {
            var prodDb = await _productoRepository.GetByIdAsync(id);
            if (prodDb == null) return false;

            prodDb.Nombre = producto.Nombre;
            prodDb.Descripcion = producto.Descripcion;
            prodDb.Precio = producto.Precio;
            prodDb.Stock = producto.Stock;
            prodDb.Activo = producto.Activo;
            prodDb.ImagenUrl = producto.ImagenUrl;

            await _productoRepository.UpdateAsync(prodDb);
            await _productoRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EliminarProductoAsync(int id)
        {
            var prodDb = await _productoRepository.GetByIdAsync(id);
            if (prodDb == null)
                return false;

            bool tieneReservas = await _productoRepository.TieneReservasAsync(id);
            if (tieneReservas)
                return false; 

            await _productoRepository.DeleteAsync(prodDb);
            await _productoRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CambiarEstadoAsync(int id)
        {
            var prodDb = await _productoRepository.GetByIdAsync(id);
            if (prodDb == null) return false;

            prodDb.Activo = !prodDb.Activo;

            await _productoRepository.UpdateAsync(prodDb);
            await _productoRepository.SaveChangesAsync();

            return true;
        }
        public async Task<IEnumerable<Producto>> ObtenerProductosActivosAsync()
        {
            var productos = await _productoRepository.GetAllAsync();
            return productos.Where(p => p.Activo);
        }

    }
}
