using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VenusBeauty.DAL.Entities
{
    public class Cita
    {
        [Key]
        public int IdCita { get; set; }

        [Required]
        public int IdCliente { get; set; }

        // Id del trabajador (usuario interno) que atenderá la cita
        [Required]
        public string IdUsuario { get; set; } = null!;

        [Required]
        public DateTime FechaHora { get; set; }

        // Nuevo: enum con los estados válidos
        public EstadoCita Estado { get; set; } = EstadoCita.Reservada;

        public bool Activo { get; set; } = true;

        // Nuevo: importe total de la cita
        public decimal Total { get; set; }

        /* ---------- Propiedades de navegación ---------- */
        [ForeignKey(nameof(IdCliente))]
        public Cliente? Cliente { get; set; }

        // Se cargará aparte si alguna vez lo necesitas, por eso [NotMapped]
        [NotMapped]
        public IdentityUser? Usuario { get; set; }

        // Detalles de servicios incluidos en la cita
        public ICollection<DetalleCita> DetalleCitas { get; set; } = new List<DetalleCita>();

        // Productos reservados junto con la cita
        public ICollection<ReservaProducto> ReservaProductos { get; set; } = new List<ReservaProducto>();
    }
}
