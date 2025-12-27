using Microsoft.EntityFrameworkCore;
using Tadawi.Data;
using Tadawi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tadawi.Repositories.MedicineRepository
{
    public class MedicineRepository : IMedicineRepository
    {
        private readonly ApplicationDbContext _context;

        public MedicineRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Medicine medicine)
        {
            if (medicine == null) return false;

            try
            {
                _context.Medicines.Add(medicine);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Medicine medicine)
        {
            if (medicine == null) return false;

            try
            {
                _context.Medicines.Update(medicine);
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
                var entity = await _context.Medicines.FindAsync(id);
                if (entity == null) return false;

                _context.Medicines.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Medicine>> GetAsync()
        {
            return await _context.Medicines.ToListAsync();
        }

        public async Task<Medicine> GetByIdAsync(int id)
        {
            return await _context.Medicines.FirstOrDefaultAsync(m => m.Id == id);
        }

        // دالة قائمة مختصرة مشابهة لـ PatientList و DoctorList
        public async Task<List<Medicine>> MedicineList()
        {
            return await _context.Medicines
                .Select(m => new Medicine { Id = m.Id, MedicineName = m.MedicineName })
                .ToListAsync();
        }
    }
}
