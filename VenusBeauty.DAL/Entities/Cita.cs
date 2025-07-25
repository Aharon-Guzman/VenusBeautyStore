using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VenusBeauty.DAL.Entities
{
    public class Cita
    {
        [Key]
        public int IdCita { get; set; }

        [Required]
        public int IdCliente { get; set; }

        [Required]
        public string? IdUsuario { get; set; } // Trabajador (de Identity)

        [Required]
        public DateTime FechaHora { get; set; }

        [Required]
        [MaxLength(20)]
        public string? Estado { get; set; }

        [Required]
        public bool Activo { get; set; } = true;

        // Propiedades de navegación (FKs)
        [ForeignKey("IdCliente")]
        public Cliente? Cliente { get; set; }

        [NotMapped]
        public IdentityUser? Usuario { get; set; } // Trabajador (Identity)
    }
}
