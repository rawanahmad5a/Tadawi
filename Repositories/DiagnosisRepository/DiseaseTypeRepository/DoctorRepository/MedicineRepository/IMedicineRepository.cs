using Tadawi.Models;

namespace Tadawi.Repositories.MedicineRepository
{
    public interface IMedicineRepository
    {
      public  Task<bool> AddAsync(Medicine medicine);
      public Task<bool> UpdateAsync(Medicine medicine);
      public Task<bool> DeleteAsync(int id);
      public Task<List<Medicine>> GetAsync();
      public Task<Medicine> GetByIdAsync(int id);
      public  Task<List<Medicine>> MedicineList();
    }
}
