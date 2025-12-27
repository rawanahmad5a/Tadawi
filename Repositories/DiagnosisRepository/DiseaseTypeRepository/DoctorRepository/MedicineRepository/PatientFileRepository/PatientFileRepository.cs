using Microsoft.EntityFrameworkCore;
using Tadawi.Data;
using Tadawi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tadawi.Repositories.PatientFileRepository
{
    public class PatientFileRepository : IPatientFileRepository
    {
        private readonly ApplicationDbContext _context;

        public PatientFileRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PatientFile>> GetAsync()
        {
            return await _context.PatientFiles
                .Include(p => p.Visits)
                .ToListAsync();
        }

        public async Task<PatientFile> GetByIdAsync(int id)
        {
            return await _context.PatientFiles
                .Include(p => p.Visits)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<int> AddAsync(string diagnosis)
        {
            if (string.IsNullOrEmpty(diagnosis))
                return 0;

            try
            {
                PatientFile patientFile = new PatientFile
                {
                    Diagnosis = diagnosis
                };
                _context.PatientFiles.Add(patientFile);
                await _context.SaveChangesAsync();
                return patientFile.Id;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<bool> UpdateAsync(PatientFile patientFile)
        {
            if (patientFile == null)
                return false;

            try
            {
                _context.PatientFiles.Update(patientFile);
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
                var entity = await _context.PatientFiles.FindAsync(id);
                if (entity == null)
                    return false;

                _context.PatientFiles.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // دالة جديدة مشابهة لدالة PainetList في PatientRepository
        public async Task<List<PatientFile>> PatientFileList()
        {
            var list = await _context.PatientFiles
                .Select(x => new PatientFile { Id = x.Id, Diagnosis = x.Diagnosis })
                .ToListAsync();
            return list;
        }
    }
}
