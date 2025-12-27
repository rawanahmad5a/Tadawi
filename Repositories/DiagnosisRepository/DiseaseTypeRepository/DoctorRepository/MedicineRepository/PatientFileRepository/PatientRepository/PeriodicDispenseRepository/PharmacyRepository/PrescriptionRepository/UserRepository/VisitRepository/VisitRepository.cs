using Microsoft.EntityFrameworkCore;
using Tadawi.Data;
using Tadawi.Models;

namespace Tadawi.Repositories.VisitRepository
{
    public class VisitRepository : IVisitRepository
    {
        private readonly ApplicationDbContext _context;

        public VisitRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Visit>> GetAsync()
        {
            return await _context.Visits
                .Include(v => v.Patient)
                .Include(v => v.Doctor)
                .Include(v => v.Diagnosis)
                .Include(v => v.DiseaseType)
                .ToListAsync();
        }

        public async Task<Visit> GetByIdAsync(int id)
        {
            return await _context.Visits
                .Include(v => v.Patient)
                .Include(v => v.Doctor)
                .Include(v => v.Diagnosis)
                .Include(v => v.DiseaseType)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<bool> AddAsync(Visit visit)
        {
            if (visit == null)
                return false;

            try
            {
                _context.Visits.Add(visit);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Visit visit)
        {
            if (visit == null)
                return false;

            try
            {
                _context.Visits.Update(visit);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var entity = await _context.Visits.FindAsync(id);
                if (entity == null)
                    return false;

                _context.Visits.Remove(entity);
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
