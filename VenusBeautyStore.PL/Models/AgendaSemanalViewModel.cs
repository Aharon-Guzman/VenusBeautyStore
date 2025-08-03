using System;
using System.Collections.Generic;

namespace VenusBeautyStore.PL.Models
{
    public class AgendaSemanalViewModel
    {
        /// Fecha del lunes (o del día que marques como inicio).
        public DateTime Inicio { get; set; }

        /// Siete objetos AgendaDiaViewModel, uno por cada día.
        public IEnumerable<AgendaDiaViewModel> Dias { get; set; }
            = new List<AgendaDiaViewModel>();
    }
}
