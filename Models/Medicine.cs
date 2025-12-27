namespace Tadawi.Models
{
    public class Medicine
    {
        public int Id { get; set; }        // PK
        public string MedicineName { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }

        public ICollection<Prescription>? Prescriptions { get; set; }
    }
}
