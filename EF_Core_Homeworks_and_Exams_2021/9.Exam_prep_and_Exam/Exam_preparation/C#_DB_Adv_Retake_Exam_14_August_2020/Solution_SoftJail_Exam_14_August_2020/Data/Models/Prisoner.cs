using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SoftJail.Data.Models
{
    public class Prisoner
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Nickname { get; set; }

        public int Age { get; set; }
        public DateTime IncarcerationDate { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public decimal? Bail { get; set; }

        public int? CellId { get; set; } //here want nullable type
        public Cell Cell { get; set; }

        public ICollection<Mail> Mails { get; set; } = new HashSet<Mail>();
        public ICollection<OfficerPrisoner> PrisonerOfficers { get; set; } = new HashSet<OfficerPrisoner>();
    }
}
//	Id – integer, Primary Key
//	FullName – text with min length 3 and max length 20 (required)
//	Nickname – text starting with "The " and a single word only of letters with an uppercase letter for beginning(example: The Prisoner) (required)
//	Age – integer in the range[18, 65] (required)
//	IncarcerationDate ¬– Date(required)
//	ReleaseDate– Date
//	Bail– decimal(non - negative, minimum value: 0)
//	CellId - integer, foreign key
//	Cell – the prisoner's cell
//•	Mails - collection of type Mail
//•	PrisonerOfficers - collection of type OfficerPrisoner
