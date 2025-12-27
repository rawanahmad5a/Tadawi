using Microsoft.EntityFrameworkCore;
using Tadawi.Data;
using Tadawi.Models;

namespace Tadawi.Repositories.PeriodicDispenseRepository
{
    public class PeriodicDispenseRepository : IPeriodicDispenseRepository
    {
        private readonly ApplicationDbContext _context;

        public PeriodicDispenseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PeriodicDispense>> GetAsync()
        {
            return await _context.PeriodicDispenses
                .Include(x => x.Patient)
                .Include(x => x.Medicine)
                .ToListAsync();
        }

        public async Task<PeriodicDispense> GetByIdAsync(int id)
        {
            return await _context.PeriodicDispenses
                .Include(x => x.Patient)
                .Include(x => x.Medicine)
                .FirstOrDefaultAsync(x => x.PeriodicDispenseId == id);
        }

        public async Task<bool> AddAsync(PeriodicDispense periodicDispense)
        {
            if (periodicDispense == null)
                return false;

            try
            {
                _context.PeriodicDispenses.Add(periodicDispense);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(PeriodicDispense periodicDispense)
        {
            if (periodicDispense == null)
                return false;

            try
            {
                _context.PeriodicDispenses.Update(periodicDispense);
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
                var entity = await _context.PeriodicDispenses.FindAsync(id);
                if (entity == null)
                    return false;

                _context.PeriodicDispenses.Remove(entity);
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
