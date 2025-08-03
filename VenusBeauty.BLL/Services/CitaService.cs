using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VenusBeauty.DAL.Entities;
using VenusBeauty.DAL.Repositories;

namespace VenusBeauty.BLL.Services
{
    public class CitaService : ICitaService
    {
        private readonly ICitaRepository _citaRepo;
        private readonly IServicioRepository _servicioRepo;
        private readonly IProductoRepository _productoRepo;

        public CitaService(
            ICitaRepository citaRepo,
            IServicioRepository servicioRepo,
            IProductoRepository productoRepo)
        {
            _citaRepo = citaRepo;
            _servicioRepo = servicioRepo;
            _productoRepo = productoRepo;
        }

        /* ---------- Lectura ---------- */

        public Task<Cita?> ObtenerCitaAsync(int id) => _citaRepo.GetByIdAsync(id);

        public Task<IEnumerable<Cita>> ObtenerAgendaAsync(
            DateTime desde, DateTime hasta, string? idUsuario = null)
            => _citaRepo.GetAgendaAsync(desde, hasta, idUsuario);

        /* ---------- Escritura ---------- */

        //public async Task<int> CrearCitaAsync(
        //    int idCliente,
        //    string idUsuario,
        //    DateTime fechaHora,
        //    IEnumerable<int> idServicios,
        //    IDictionary<int, int>? productos = null)   // productoId → cantidad
        //{
        //    /* 1. Cargar servicios y calcular total */
        //    var servicios = new List<Servicio>();
        //    foreach (var id in idServicios)
        //    {
        //        var srv = await _servicioRepo.GetByIdAsync(id);
        //        if (srv is not null) servicios.Add(srv);
        //    }
        //    if (!servicios.Any())
        //        throw new InvalidOperationException("Debe seleccionar al menos un servicio.");

        //    decimal total = servicios.Sum(s => s.Precio);

        //    /* 2. Cargar productos opcionales */
        //    var reservas = new List<ReservaProducto>();
        //    if (productos is not null && productos.Any())
        //    {
        //        foreach (var kvp in productos)            // kvp.Key = idProducto, kvp.Value = cantidad
        //        {
        //            var prod = await _productoRepo.GetByIdAsync(kvp.Key);
        //            if (prod is null) continue;

        //            reservas.Add(new ReservaProducto
        //            {
        //                IdProducto = prod.IdProducto,
        //                Cantidad = kvp.Value,
        //                PrecioUnitario = prod.Precio
        //            });
        //            total += prod.Precio * kvp.Value;
        //        }
        //    }

        //    /* 3. Construir cita */
        //    var cita = new Cita
        //    {
        //        IdCliente = idCliente,
        //        IdUsuario = idUsuario,
        //        FechaHora = fechaHora,
        //        Estado = EstadoCita.Reservada,   // ← enum
        //        Total = total,
        //        DetalleCitas = servicios.Select(s => new DetalleCita
        //        {
        //            IdServicio = s.IdServicio
        //        }).ToList(),
        //        ReservaProductos = reservas
        //    };

        //    /* 4. Guardar y devolver Id */
        //    await _citaRepo.AddAsync(cita);
        //    return cita.IdCita;
        //}
        public async Task<int> CrearCitaAsync(
    int idCliente,
    string idUsuario,
    DateTime fechaHora,
    IEnumerable<int> idServicios,
    IDictionary<int, int>? productos = null)
        {
            // 1. Cargar servicios y calcular total y duración
            var servicios = new List<Servicio>();
            foreach (var id in idServicios)
            {
                var srv = await _servicioRepo.GetByIdAsync(id);
                if (srv is not null) servicios.Add(srv);
            }

            if (!servicios.Any())
                throw new InvalidOperationException("Debe seleccionar al menos un servicio.");

            decimal total = servicios.Sum(s => s.Precio);

            // 🔹 Calcular duración total de la cita
            int duracionTotal = servicios.Sum(s => s.Duracion);
            DateTime fechaFin = fechaHora.AddMinutes(duracionTotal);

            // 🔹 Validar solapamientos
            var citasExistentes = await _citaRepo.GetAgendaAsync(
                fechaHora.AddHours(-12), fechaFin.AddHours(12), idUsuario);

            bool haySolape = citasExistentes.Any(c =>
            {
                var duracionExistente = c.DetalleCitas.Sum(d => d.Servicio.Duracion);
                var inicioExistente = c.FechaHora;
                var finExistente = inicioExistente.AddMinutes(duracionExistente);

                return inicioExistente < fechaFin && finExistente > fechaHora;
            });

            if (haySolape)
                throw new InvalidOperationException("El estilista ya tiene una cita en ese horario.");

            // 2. Cargar productos opcionales
            var reservas = new List<ReservaProducto>();
            if (productos is not null && productos.Any())
            {
                foreach (var kvp in productos)
                {
                    var prod = await _productoRepo.GetByIdAsync(kvp.Key);
                    if (prod is null) continue;

                    reservas.Add(new ReservaProducto
                    {
                        IdProducto = prod.IdProducto,
                        Cantidad = kvp.Value,
                        PrecioUnitario = prod.Precio
                    });
                    total += prod.Precio * kvp.Value;
                }
            }

            // 3. Construir cita
            var cita = new Cita
            {
                IdCliente = idCliente,
                IdUsuario = idUsuario,
                FechaHora = fechaHora,
                Estado = EstadoCita.Reservada,
                Total = total,
                DetalleCitas = servicios.Select(s => new DetalleCita
                {
                    IdServicio = s.IdServicio
                }).ToList(),
                ReservaProductos = reservas
            };

            // 4. Guardar y devolver Id
            await _citaRepo.AddAsync(cita);
            return cita.IdCita;
        }


        public async Task<bool> ActualizarEstadoAsync(int idCita, EstadoCita nuevoEstado)
        {
            var cita = await _citaRepo.GetByIdAsync(idCita);
            if (cita is null) return false;

            cita.Estado = nuevoEstado;
            await _citaRepo.UpdateAsync(cita);
            return true;
        }

        public Task<bool> CancelarCitaAsync(int idCita)
            => ActualizarEstadoAsync(idCita, EstadoCita.Cancelada);
    }
}
