using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VenusBeauty.DAL.Entities;

namespace VenusBeauty.DAL.Repositories
{
    public interface ITrabajadorRepository
    {
        Task<IEnumerable<Trabajador>> GetAllAsync();
        Task<Trabajador?> GetByIdAsync(int id);
        Task AddAsync(Trabajador trabajador);
        Task UpdateAsync(Trabajador trabajador);
        Task DeleteAsync(Trabajador trabajador);
        Task SaveChangesAsync();
        Task<IEnumerable<Trabajador>> GetEstilistasAsync();
        Task<IEnumerable<Trabajador>> GetEstilistasActivosAsync();
        Task<bool> TieneCitasAsync(string userId);

    }
}
