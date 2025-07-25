using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VenusBeauty.DAL.Entities
{
    public class DetalleCita
    {
        [Key]
        public int IdDetalle { get; set; }

        [Required]
        public int IdCita { get; set; }

        [Required]
        public int IdServicio { get; set; }

        // Relaciones
        [ForeignKey("IdCita")]
        public Cita? Cita { get; set; }

        [ForeignKey("IdServicio")]
        public Servicio? Servicio { get; set; }
    }
}
