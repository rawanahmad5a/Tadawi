using System.ComponentModel.DataAnnotations.Schema;

namespace Tadawi.Models
{
    public class Patient
    {
        public int Id { get; set; }      // PK
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }


        // FK لملف المريض
        public int? PatientFileId { get; set; }

        // العلاقات
        public PatientFile? PatientFile { get; set; }
        public ICollection<Visit>? Visits { get; set; }

        [NotMapped]
        public string Diagnosis { get; set; }
    }
}
