using System;
using System.ComponentModel.DataAnnotations;
using VenusBeauty.DAL.Entities;

namespace VenusBeautyStore.PL.Models
{
    public class CitaDeleteViewModel
    {
        [Required]
        public int IdCita { get; set; }

        // Solo para mostrar en la confirmación (lectura)
        public string? Cliente { get; set; }
        public string? Estilista { get; set; }
        public DateTime FechaHora { get; set; }
        public EstadoCita Estado { get; set; }

        public string Resumen =>
            $"{Cliente} — {Estilista} — {FechaHora:g} — {Estado}";
    }
}
