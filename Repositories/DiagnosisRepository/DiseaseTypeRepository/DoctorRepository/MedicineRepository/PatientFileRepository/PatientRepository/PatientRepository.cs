using Microsoft.EntityFrameworkCore;
using Tadawi.Data;
using Tadawi.Models;
using Tadawi.Repositories.PatientFileRepository;

namespace Tadawi.Repositories.PatientRepository
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IPatientFileRepository _patientFileRepository;
        public PatientRepository(ApplicationDbContext context, IPatientFileRepository patientFileRepository)
        {
            _context = context;
            _patientFileRepository = patientFileRepository;
        }

        public async Task<List<Patient>> PainetList()
        {
            var patientList=await _context.Patients.Select(x => new Patient{ Id= x.Id, Name=x.Name}).ToListAsync();
            return patientList;
        }
        public async Task<List<Patient>> GetAsync()
        {
            return await _context.Patients
                .Include(p => p.PatientFile)
                .ToListAsync();
        }

        public async Task<Patient> GetByIdAsync(int id)
        {
            return await _context.Patients
                .Include(p => p.PatientFile)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> AddAsync(Patient patient)
        {
            if (patient == null)
                return false;

            int patientFileId = await _patientFileRepository.AddAsync(patient.Diagnosis);
            if(patientFileId == 0)
            {
                return false;
            }

            patient.PatientFileId = patientFileId;
            try
            {
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                await _patientFileRepository.DeleteAsync(patientFileId);
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Patient patient)
        {
            if (patient == null)
                return false;

            try
            {
                _context.Patients.Update(patient);
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
                var entity = await _context.Patients.FindAsync(id);
                if (entity == null)
                    return false;

                _context.Patients.Remove(entity);
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
