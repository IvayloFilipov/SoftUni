using P01_HospitalDatabase.Data.Models;

namespace P01_HospitalDatabase.Data.Models
{
    public class Diagnose
    {
        public int DiagnoseId { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; }
    }
}
//	DiagnoseId
//	Name(up to 50 characters, unicode)
//	Comments(up to 250 characters, unicode)
//	Patient