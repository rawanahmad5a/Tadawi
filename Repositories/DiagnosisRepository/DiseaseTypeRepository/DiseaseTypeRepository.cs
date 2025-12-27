using Microsoft.EntityFrameworkCore;
using Tadawi.Data;
using Tadawi.Models;

namespace Tadawi.Repositories.DiseaseTypeRepository
{
    public class DiseaseTypeRepository : IDiseaseTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public DiseaseTypeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(DiseaseType diseaseType)
        {
            if(diseaseType == null)
            {
                return false;
            }

            try
            {
                _context.Add(diseaseType);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex) { 
                return false;
            }

        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var entity = await _context.DiseaseTypes.FindAsync(id);
                if (entity == null)
                    return false;

                _context.DiseaseTypes.Remove(entity);
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }


        public async Task<List<DiseaseType>> GetAsync()
        {
            return await _context.DiseaseTypes.ToListAsync();
        }

        public async Task<DiseaseType> GetByIdAsync(int id)
        {
            return await _context.DiseaseTypes
                .FirstOrDefaultAsync(d => d.Id == id);
        }


        public async Task<bool> UpdateAsync(DiseaseType diseaseType)
        {
            if (diseaseType == null)
            {
                return false;
            }

            try
            {
                _context.Update(diseaseType);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
