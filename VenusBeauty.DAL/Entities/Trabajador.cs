using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VenusBeauty.DAL.Entities
{
    public class Trabajador
    {
        [Key]
        public int IdTrabajador { get; set; }

        // 🔹 No debe ser obligatorio porque se asigna en el servicio
        [MaxLength(450)]
        public string? UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Telefono { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Rol { get; set; } = string.Empty;

        // 🔹 Relación con Identity → Opcional
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

    }
}
