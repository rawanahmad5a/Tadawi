namespace Tadawi.Models
{
    public class Visit
    {
        public int Id { get; set; }               // PK
        public DateTime VisitDate { get; set; }
        public string VisitType { get; set; }
        // FK
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int DiagnosisId { get; set; }
        public int DiseaseTypeId { get; set; }

        // العلاقات
        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
        public Diagnosis Diagnosis { get; set; }
        public DiseaseType DiseaseType { get; set; }

        public ICollection<Prescription>? Prescriptions { get; set; }
    }
}