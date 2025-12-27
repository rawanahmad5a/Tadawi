namespace Tadawi.Models
{
    public class DiseaseType
    {
        public int Id { get; set; }   // PK
        public string DiseaseName { get; set; }
        public string Symptoms { get; set; }

        // لاحقًا نربطه بالزيارة
        public ICollection<Visit>? Visits { get; set; }
    }
}
