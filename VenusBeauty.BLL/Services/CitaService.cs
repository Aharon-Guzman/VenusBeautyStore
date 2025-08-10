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


        public async Task<int> CrearCitaAsync(
    int idCliente,
    string idUsuario,
    DateTime fechaHora,
    IEnumerable<int> idServicios,
    IDictionary<int, int>? productos = null)
        {
            // 1) Cargar servicios y calcular total y duración
            var servicios = new List<Servicio>();
            foreach (var id in idServicios)
            {
                var srv = await _servicioRepo.GetByIdAsync(id);
                if (srv is not null) servicios.Add(srv);
            }

            if (!servicios.Any())
                throw new InvalidOperationException("Debe seleccionar al menos un servicio.");

            decimal total = servicios.Sum(s => s.Precio);

            // Duración total y fin
            int duracionTotal = servicios.Sum(s => s.Duracion);
            DateTime fechaFin = fechaHora.AddMinutes(duracionTotal);

            // 2) Validar solapamientos para el estilista
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

            // 3) Cargar productos opcionales (VALIDANDO STOCK DISPONIBLE)
            var reservas = new List<ReservaProducto>();
            if (productos is not null && productos.Any())
            {
                foreach (var kvp in productos)
                {
                    int idProd = kvp.Key;
                    int cantSolicitada = kvp.Value;

                    // stock disponible = stock físico - reservas activas (Reservada/Confirmada)
                    int disponible = await _productoRepo.GetStockDisponibleAsync(idProd);
                    if (disponible < cantSolicitada)
                        throw new InvalidOperationException(
                            $"Stock insuficiente para el producto {idProd}. Disponible: {disponible}, solicitado: {cantSolicitada}.");

                    var prod = await _productoRepo.GetByIdAsync(idProd)
                              ?? throw new InvalidOperationException("Producto no válido.");

                    reservas.Add(new ReservaProducto
                    {
                        IdProducto = prod.IdProducto,
                        Cantidad = cantSolicitada,
                        PrecioUnitario = prod.Precio
                    });

                    total += prod.Precio * cantSolicitada;
                }
            }

            // 4) Construir cita
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

            // 5) Guardar y devolver Id
            await _citaRepo.AddAsync(cita);
            return cita.IdCita;
        }
        public async Task<bool> ActualizarEstadoAsync(int idCita, EstadoCita nuevoEstado)
        {
            var cita = await _citaRepo.GetByIdAsync(idCita);
            if (cita is null) return false;

            // estado terminal: no se puede mover desde Completada a otro
            if (cita.Estado == EstadoCita.Completada && nuevoEstado != EstadoCita.Completada)
                throw new InvalidOperationException("Una cita en estado Completada no se puede modificar.");

            // idempotencia
            if (cita.Estado == nuevoEstado) return true;

            // consumir stock solo al transicionar a Completada (primera vez)
            if (nuevoEstado == EstadoCita.Completada)
            {
                await _productoRepo.ConsumirStockPorCitaAsync(idCita);
            }

            cita.Estado = nuevoEstado;
            await _citaRepo.UpdateAsync(cita);
            return true;
        }
        public async Task<bool> EliminarCitaAsync(int idCita)
        {
            var cita = await _citaRepo.GetByIdAsync(idCita);
            if (cita is null) return false;

            // Regla: no permitir eliminar citas completadas (ya consumieron stock)
            if (cita.Estado == EstadoCita.Completada)
                throw new InvalidOperationException("No se puede eliminar una cita en estado Completada.");

            await _citaRepo.DeleteAsync(idCita); // eliminará detalles y reservas por cascada
            return true;
        }


        public Task<bool> CancelarCitaAsync(int idCita)
            => ActualizarEstadoAsync(idCita, EstadoCita.Cancelada);
    }
}
