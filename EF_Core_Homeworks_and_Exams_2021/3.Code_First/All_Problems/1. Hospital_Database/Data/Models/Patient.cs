﻿using System.Collections.Generic;

namespace P01_HospitalDatabase.Data.Models
{
    public class Patient
    {
        public int PatientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public bool HasInsurance { get; set; }

        public ICollection<PatientMedicament> Prescriptions { get; set; } 
            = new HashSet<PatientMedicament>();

        public ICollection<Diagnose> Diagnoses { get; set; } 
            = new HashSet<Diagnose>();

        public ICollection<Visitation> Visitations { get; set; } 
            = new HashSet<Visitation>();
    }
}
//	PatientId
//	FirstName(up to 50 characters, unicode)
//	LastName(up to 50 characters, unicode)
//	Address(up to 250 characters, unicode)
//	Email(up to 80 characters, not unicode)
//	HasInsurance
