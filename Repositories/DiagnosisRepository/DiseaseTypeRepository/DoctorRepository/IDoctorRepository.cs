using Tadawi.Models;

namespace Tadawi.Repositories.DoctorRepository
{
    public interface IDoctorRepository
    {
     public   Task<bool> AddAsync(Doctor doctor);
     public  Task<bool> UpdateAsync(Doctor doctor);
     public Task<bool> DeleteAsync(int id);
     public Task<List<Doctor>> GetAsync();
     public  Task<Doctor> GetByIdAsync(int id);
     public  Task<List<Doctor>> DoctorList();
    }
}
