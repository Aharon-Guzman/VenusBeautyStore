using System;
using System.Collections.Generic;

namespace VenusBeautyStore.PL.Models
{
    public class AgendaMesViewModel
    {
        public DateTime Mes { get; set; }   // Primer día del mes

        // Lista de semanas; cada semana es una lista de 7 días
        public List<List<DiaCalendarioDto>> Semanas { get; set; } = new();

        // Clase interna para representar cada celda del calendario
        public class DiaCalendarioDto
        {
            public DateTime? Fecha { get; set; } // null para celdas vacías
            public List<AgendaCitaDto> Citas { get; set; } = new();
        }
    }
}
