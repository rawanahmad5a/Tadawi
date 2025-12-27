using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tadawi.Data;
using Tadawi.Models;

namespace Tadawi.Repositories.PrescriptionRepository
{
    public class PrescriptionRepository : IPrescriptionRepository
    {
        private readonly ApplicationDbContext _context;

        public PrescriptionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Prescription>> GetAsync()
        {
            return await _context.Prescriptions
                .Include(p => p.Visit)
                .Include(p => p.Pharmacy)
                .Include(p => p.Medicine)
                .ToListAsync();
        }

        public async Task<Prescription> GetByIdAsync(int id)
        {
            return await _context.Prescriptions
                .Include(p => p.Visit)
                .Include(p => p.Pharmacy)
                .Include(p => p.Medicine)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> AddAsync(Prescription prescription)
        {
            if (prescription == null) return false;
            try
            {
                _context.Prescriptions.Add(prescription);
                await _context.SaveChangesAsync();
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> UpdateAsync(Prescription prescription)
        {
            if (prescription == null) return false;
            try
            {
                _context.Prescriptions.Update(prescription);
                await _context.SaveChangesAsync();
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var entity = await _context.Prescriptions.FindAsync(id);
                if (entity == null) return false;
                _context.Prescriptions.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch { return false; }
        }
    }
}
