using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VenusBeauty.DAL.Entities;
using VenusBeauty.DAL.Repositories;

namespace VenusBeauty.BLL.Services
{
    public class ServicioService : IServicioService
    {
        private readonly IServicioRepository _servicioRepository;

        public ServicioService(IServicioRepository servicioRepository)
        {
            _servicioRepository = servicioRepository;
        }

        public async Task<IEnumerable<Servicio>> ObtenerServiciosAsync()
        {
            return await _servicioRepository.GetAllAsync();
        }

        public async Task<Servicio?> ObtenerPorIdAsync(int id)
        {
            return await _servicioRepository.GetByIdAsync(id);
        }

        public async Task<bool> CrearServicioAsync(Servicio servicio)
        {
            await _servicioRepository.AddAsync(servicio);
            await _servicioRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EditarServicioAsync(int id, Servicio servicio)
        {
            var servDb = await _servicioRepository.GetByIdAsync(id);
            if (servDb == null) return false;

            servDb.Nombre = servicio.Nombre;
            servDb.Descripcion = servicio.Descripcion;
            servDb.Precio = servicio.Precio;
            servDb.Duracion = servicio.Duracion;
            servDb.Activo = servicio.Activo;

            await _servicioRepository.UpdateAsync(servDb);
            await _servicioRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarServicioAsync(int id)
        {
            var servDb = await _servicioRepository.GetByIdAsync(id);
            if (servDb == null) return false;

            await _servicioRepository.DeleteAsync(servDb);
            await _servicioRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CambiarEstadoAsync(int id)
        {
            var servDb = await _servicioRepository.GetByIdAsync(id);
            if (servDb == null) return false;

            servDb.Activo = !servDb.Activo;

            await _servicioRepository.UpdateAsync(servDb);
            await _servicioRepository.SaveChangesAsync();
            return true;
        }
    }
}
