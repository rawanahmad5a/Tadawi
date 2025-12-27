using Microsoft.EntityFrameworkCore;
using Tadawi.Data;
using Tadawi.Models;

namespace Tadawi.Repositories.PharmacyRepository
{
    public class PharmacyRepository : IPharmacyRepository
    {
        private readonly ApplicationDbContext _context;

        public PharmacyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Pharmacy>> GetAsync()
        {
            return await _context.Pharmacies
                .Include(p => p.Prescriptions)
                .ToListAsync();
        }

        public async Task<Pharmacy> GetByIdAsync(int id)
        {
            return await _context.Pharmacies
                .Include(p => p.Prescriptions)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> AddAsync(Pharmacy pharmacy)
        {
            if (pharmacy == null) return false;

            try
            {
                _context.Pharmacies.Add(pharmacy);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Pharmacy pharmacy)
        {
            if (pharmacy == null) return false;

            try
            {
                _context.Pharmacies.Update(pharmacy);
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
                var entity = await _context.Pharmacies.FindAsync(id);
                if (entity == null) return false;

                _context.Pharmacies.Remove(entity);
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
