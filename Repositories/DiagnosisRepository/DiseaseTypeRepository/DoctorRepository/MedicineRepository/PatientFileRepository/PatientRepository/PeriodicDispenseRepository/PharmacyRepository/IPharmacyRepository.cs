using Tadawi.Models;

namespace Tadawi.Repositories.PharmacyRepository
{
    public interface IPharmacyRepository
    {
      public  Task<List<Pharmacy>> GetAsync();
      public  Task<Pharmacy> GetByIdAsync(int id);
      public  Task<bool> AddAsync(Pharmacy pharmacy);
      public  Task<bool> UpdateAsync(Pharmacy pharmacy);
      public  Task<bool> DeleteAsync(int id);
    }
}
