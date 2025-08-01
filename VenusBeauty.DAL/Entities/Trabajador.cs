using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace VenusBeauty.DAL.Entities
{
    public class Trabajador
    {
        [Key]
        public int IdTrabajador { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser Usuario { get; set; } // ✅ Navegación hacia Identity

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(100)]
        public string Apellido { get; set; }

        [MaxLength(50)]
        public string Telefono { get; set; }

        [Required]
        [MaxLength(50)]
        public string Rol { get; set; }
    }
}
