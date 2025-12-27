using Tadawi.Models;

namespace Tadawi.Models
{
    public class Diagnosis
    {
        public int Id { get; set; }       // PK
        public string Description { get; set; }

        // علاقة مع الزيارات
        public ICollection<Visit>? Visits { get; set; }
    }
}
