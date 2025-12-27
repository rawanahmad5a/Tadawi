using System.Collections.Generic;
using System.Threading.Tasks;
using Tadawi.Models;

namespace Tadawi.Repositories.PrescriptionRepository
{
    public interface IPrescriptionRepository
    {
      public  Task<List<Prescription>> GetAsync();
      public  Task<Prescription> GetByIdAsync(int id);
      public  Task<bool> AddAsync(Prescription prescription);
      public  Task<bool> UpdateAsync(Prescription prescription);
      public  Task<bool> DeleteAsync(int id);
    }
}
