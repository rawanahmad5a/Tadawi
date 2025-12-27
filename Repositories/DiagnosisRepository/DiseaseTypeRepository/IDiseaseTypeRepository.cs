using Microsoft.AspNetCore.Mvc;
using Tadawi.Models;

namespace Tadawi.Repositories.DiseaseTypeRepository
{
    public interface IDiseaseTypeRepository
    {
        public  Task<bool> AddAsync(DiseaseType diseaseType);
        public  Task<bool> UpdateAsync(DiseaseType diseaseType);
        public Task<bool> DeleteAsync(int id);
        public Task<List<DiseaseType>> GetAsync();
        public Task<DiseaseType> GetByIdAsync(int id);
    }
}
