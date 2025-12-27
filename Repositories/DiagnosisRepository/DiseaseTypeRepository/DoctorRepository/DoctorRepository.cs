using Microsoft.EntityFrameworkCore;
using Tadawi.Data;
using Tadawi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tadawi.Repositories.DoctorRepository
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly ApplicationDbContext _context;

        public DoctorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Doctor doctor)
        {
            if (doctor == null) return false;

            try
            {
                _context.Doctors.Add(doctor);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Doctor doctor)
        {
            if (doctor == null) return false;

            try
            {
                _context.Doctors.Update(doctor);
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
                var entity = await _context.Doctors.FindAsync(id);
                if (entity == null) return false;

                _context.Doctors.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Doctor>> GetAsync()
        {
            return await _context.Doctors.ToListAsync();
        }

        public async Task<Doctor> GetByIdAsync(int id)
        {
            return await _context.Doctors.FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<List<Doctor>> DoctorList()
        {
            return await _context.Doctors
                .Select(d => new Doctor { Id = d.Id, Name = d.Name })
                .ToListAsync();
        }
    }
}
