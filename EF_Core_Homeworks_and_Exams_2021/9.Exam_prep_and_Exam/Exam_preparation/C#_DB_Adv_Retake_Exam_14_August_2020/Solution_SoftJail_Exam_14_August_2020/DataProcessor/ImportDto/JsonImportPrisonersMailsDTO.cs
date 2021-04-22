using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SoftJail.DataProcessor.ImportDto
{
    public class JsonImportPrisonersMailsDTO
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string FullName { get; set; }
        [Required]
        [RegularExpression(@"^((The) [A-Z]{1}[a-z]{2,})$")]
        public string Nickname { get; set; }
        [Required]
        [Range(18, 65)]
        public int Age { get; set; }

        [Required]
        public string IncarcerationDate { get; set; }

        public string ReleaseDate { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal? Bail { get; set; }

        public int? CellId { get; set; }

        [Required]
        public InnerDtoMails[] Mails { get; set; }
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

    public class InnerDtoMails
    {
        [Required]
        public string Description { get; set; }

        [Required]
        public string Sender { get; set; }

        [Required]
        [RegularExpression(@"^((\w*\d*\s){1,}(str.))$")]
        public string Address { get; set; }
    }
    //	Id – integer, Primary Key
    //	Description– text (required)
    //	Sender – text(required)
    //	Address – text, consisting only of letters, spaces and numbers, which ends with “ str.” (required) (Example: “62 Muir Hill str.“)
    //	PrisonerId - integer, foreign key(required)
    //	Prisoner – the mail's Prisoner (required)

   // "Description": "Hello, my name is Mr. Null and I am invisible for computers",
//        "Sender": "Mr. Null",
//        "Address": "6 Riverside Trail str."
}
//{
//    "FullName": null,
//    "Nickname": "The Null",
//    "Age": 38,
//    "IncarcerationDate": "12/09/1967",
//    "ReleaseDate": "07/02/1989",
//    "Bail": 93934.2,
//    "CellId": 4,
//    "Mails": [
//      {
//        "Description": "Hello, my name is Mr. Null and I am invisible for computers",
//        "Sender": "Mr. Null",
//        "Address": "6 Riverside Trail str."
//      }
//    ]
//  },
