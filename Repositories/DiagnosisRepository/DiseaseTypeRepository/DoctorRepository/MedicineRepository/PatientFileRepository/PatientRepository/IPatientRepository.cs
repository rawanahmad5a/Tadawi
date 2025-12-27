using Tadawi.Models;

namespace Tadawi.Repositories.PatientRepository
{
    public interface IPatientRepository
    {
       public Task<List<Patient>> GetAsync();
       public Task<Patient> GetByIdAsync(int id);
       public Task<bool> AddAsync(Patient patient);
       public Task<bool> UpdateAsync(Patient patient);
       public  Task<bool> DeleteAsync(int id);
        public Task<List<Patient>> PainetList();
    }
}
