using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VenusBeauty.DAL.Entities;

namespace VenusBeauty.DAL.Repositories
{
    public interface ICitaRepository
    {
        Task<Cita?> GetByIdAsync(int id);
        Task<IEnumerable<Cita>> GetAgendaAsync(DateTime desde, DateTime hasta, string? trabajadorId = null);
        Task AddAsync(Cita cita);
        Task UpdateAsync(Cita cita);
        Task DeleteAsync(int id);
    }
}
