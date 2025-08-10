using System.ComponentModel.DataAnnotations;
using VenusBeauty.DAL.Entities; // para EstadoCita

namespace VenusBeautyStore.PL.Models
{
    public class CitaEditEstadoViewModel
    {
        [Required]
        public int IdCita { get; set; }

        [Required]
        public EstadoCita Estado { get; set; }
    }
}
