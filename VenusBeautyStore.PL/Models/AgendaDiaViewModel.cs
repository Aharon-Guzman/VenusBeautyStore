using System;
using System.Collections.Generic;
using VenusBeauty.DAL.Entities;   // usa la entidad Cita por ahora

namespace VenusBeautyStore.PL.Models
{

    public class AgendaCitaDto
    {
        public DateTime Hora { get; set; }
        public string Cliente { get; set; } = "";
        public string Estilista { get; set; } = "";
        public string Servicios { get; set; } = "";
        public string Productos { get; set; } = "";
        public EstadoCita Estado { get; set; }
    }

    public class AgendaDiaViewModel
    {
        public DateTime Fecha { get; set; }
        public IEnumerable<AgendaCitaDto> Citas { get; set; } = new List<AgendaCitaDto>();
    }
}