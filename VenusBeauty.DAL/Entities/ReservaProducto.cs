using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VenusBeauty.DAL.Entities
{
    [Table("ReservaProductos")]
    public class ReservaProducto
    {
        [Key]
        public int IdReserva { get; set; }

        [Required]
        public int IdCita { get; set; }

        [Required]
        public int IdProducto { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal PrecioUnitario { get; set; }

        // Relaciones
        [ForeignKey("IdCita")]
        public Cita? Cita { get; set; }

        [ForeignKey("IdProducto")]
        public Producto? Producto { get; set; }
    }
}
