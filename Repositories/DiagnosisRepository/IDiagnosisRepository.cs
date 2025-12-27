using Tadawi.Models;

namespace Tadawi.Repositories.DiagnosisRepository
{
    public interface IDiagnosisRepository
    {
      public Task<bool> AddAsync(Diagnosis diagnosis);
      public Task<bool> UpdateAsync(Diagnosis diagnosis);
      public Task<bool> DeleteAsync(int id);
      public Task<List<Diagnosis>> GetAsync();
      public Task<Diagnosis> GetByIdAsync(int id);
    }
}
