using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VenusBeauty.DAL.Entities
{
    public enum EstadoCita
    {
        Reservada,   // 0
        Confirmada,  // 1
        Cancelada,   // 2
        Completada,  // 3
        NoShow       // 4
    }
}