using Tadawi.Models;

namespace Tadawi.Repositories.PatientFileRepository
{
    public interface IPatientFileRepository
    {
      public  Task<List<PatientFile>> GetAsync();
      public  Task<PatientFile> GetByIdAsync(int id);
      public  Task<int> AddAsync(string diagnosis);
      public  Task<bool> UpdateAsync(PatientFile patientFile);
      public  Task<bool> DeleteAsync(int id);
      public  Task<List<PatientFile>> PatientFileList();
    }
}
