using System;

namespace P01_HospitalDatabase.Data.Models
{
    public class Visitation
    {
        public int VisitationId { get; set; }
        public DateTime Date { get; set; }
        public string Comments { get; set; }

        public int ParientId { get; set; }
        public Patient Patient { get; set; }
    }
}
//	VisitationId
//	Date
//	Comments(up to 250 characters, unicode)
//	Patient