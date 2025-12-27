namespace Tadawi.Models
{
    public class Pharmacy
    {
        public int Id { get; set; }        // PK
        public string Name { get; set; }
        public string Location { get; set; }
        public string Phone { get; set; }

        public ICollection<Prescription>? Prescriptions { get; set; }
    }
}
