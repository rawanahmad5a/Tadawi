namespace Tadawi.Models
{
    public class Prescription
    {
        public int Id { get; set; }    // PK
        public string Dosage { get; set; }
        public DateTime DispenseDate { get; set; }

        // FK
        public int VisitId { get; set; }
        public int PharmacyId { get; set; }
        public int MedicineId { get; set; }

        // العلاقات
        public Visit Visit { get; set; }
        public Pharmacy Pharmacy { get; set; }
        public Medicine Medicine { get; set; }
    }
}
