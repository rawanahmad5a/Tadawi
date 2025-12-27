using Tadawi.Models;

namespace Tadawi.Repositories.VisitRepository
{
    public interface IVisitRepository
    {
      public  Task<List<Visit>> GetAsync();
      public  Task<Visit> GetByIdAsync(int id);
      public  Task<bool> AddAsync(Visit visit);
      public  Task<bool> UpdateAsync(Visit visit);
      public  Task<bool> DeleteAsync(int id);
    }
}
