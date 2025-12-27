namespace Tadawi.Models
{
    public class PeriodicDispense
    {
        public int PeriodicDispenseId { get; set; }   // PK
        public string Frequency { get; set; }
        public DateTime NextDispenseDate { get; set; }

        // FK
        public int PatientId { get; set; }
        public int MedicineId { get; set; }

        // العلاقات
        public Patient Patient { get; set; }
        public Medicine Medicine { get; set; }
    }
}
