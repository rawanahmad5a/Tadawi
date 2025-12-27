using Microsoft.EntityFrameworkCore;
using Tadawi.Data;
using Tadawi.Models;

namespace Tadawi.Repositories.DiagnosisRepository
{
    public class DiagnosisRepository : IDiagnosisRepository
    {
        private readonly ApplicationDbContext _context;

        public DiagnosisRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Diagnosis diagnosis)
        {
            if (diagnosis == null) return false;
            try
            {
                _context.Add(diagnosis);
                await _context.SaveChangesAsync();
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> UpdateAsync(Diagnosis diagnosis)
        {
            if (diagnosis == null) return false;
            try
            {
                _context.Update(diagnosis);
                await _context.SaveChangesAsync();
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var entity = await _context.Diagnoses.FindAsync(id);
                if (entity == null) return false;

                _context.Diagnoses.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch { return false; }
        }

        public async Task<List<Diagnosis>> GetAsync()
        {
            return await _context.Diagnoses.ToListAsync();
        }

        public async Task<Diagnosis> GetByIdAsync(int id)
        {
            return await _context.Diagnoses.FirstOrDefaultAsync(d => d.Id == id);
        }
    }
}
