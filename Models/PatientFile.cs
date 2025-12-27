namespace Tadawi.Models
{
    public class PatientFile
    {
        public int Id { get; set; }     // PK
        public string Diagnosis { get; set; }

        // FK
      // public int PatientId { get; set; }

        // العلاقة 1:1
        //public Patient? Patient { get; set; }
        public List<Visit>? Visits { get; set; }
    }
}
