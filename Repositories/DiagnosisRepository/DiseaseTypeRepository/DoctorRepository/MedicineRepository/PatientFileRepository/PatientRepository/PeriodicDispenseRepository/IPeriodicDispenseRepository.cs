using Tadawi.Models;

namespace Tadawi.Repositories.PeriodicDispenseRepository
{
    public interface IPeriodicDispenseRepository
    {
       public Task<List<PeriodicDispense>> GetAsync();
       public Task<PeriodicDispense> GetByIdAsync(int id);
       public Task<bool> AddAsync(PeriodicDispense periodicDispense);
       public Task<bool> UpdateAsync(PeriodicDispense periodicDispense);
       public Task<bool> DeleteAsync(int id);
    }
}
