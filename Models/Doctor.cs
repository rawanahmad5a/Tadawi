namespace Tadawi.Models
{
    public class Doctor
    {
        public int Id { get; set; }       // PK
        public string Name { get; set; }
        public string Specialization { get; set; }
        public string Phone { get; set; }

        // علاقة مع الزيارات
        public ICollection<Visit>? Visits { get; set; }
    }
}
