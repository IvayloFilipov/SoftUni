using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VaporStore.Data.Models.Enums;

namespace VaporStore.DataProcessor.Dto.Import
{
    public class ImportUsersDTO
    {
        [Required]
        [RegularExpression(@"^[A-Z][a-z]{2,} [A-Z][a-z]{2,}$")] //was (@"^([A-Z]{1}[a-z]{2,}\s[A-Z]{1}[a-z]{2,})$")
        public string FullName { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Range(3, 103)]
        public int Age { get; set; }

        public IEnumerable<InputCardsInnerDTO> Cards { get; set; } // or InputCardsInnerDTO[]
}
    //•	Id – integer, Primary Key
    //•	Username – text with length [3, 20] (required)
    //•	FullName – text, which has two words, consisting of Latin letters. Both start with an upper letter and are followed by lower letters. The two words are separated by a single space (ex. "John Smith") (required)
    //•	Email – text(required)
    //•	Age – integer in the range[3, 103] (required)
    //•	Cards – collection of type Card
    public class InputCardsInnerDTO
    {
        [Required]
        [RegularExpression(@"^[0-9]{4} [0-9]{4} [0-9]{4} [0-9]{4}$")] //was (([0-9]{4}\s){3}[0-9]{4})
        public string Number { get; set; } 

        [Required]
        [RegularExpression(@"^[0-9]{3}$")]
        public string CVC { get; set; }

        [Required]
        public CardType? Type { get; set; } //enumeration -> if string, pars it to enumuration
    }
}
//Id – integer, Primary Key
//•	Number – text, which consists of 4 pairs of 4 digits, separated by spaces (ex. “1234 5678 9012 3456”) (required)
//•	Cvc – text, which consists of 3 digits (ex. “123”) (required)
//•	Type – enumeration of type CardType, with possible values (“Debit”, “Credit”) (required)
//•	UserId – integer, foreign key(required)
//•	User – the card’s user (required)
//•	Purchases – collection of type Purchase
//{
//    "FullName": "Anita Ruthven",
//    "Username": "aruthven",
//    "Email": "aruthven@gmail.com",
//    "Age": 75,
//    "Cards": [
//      {
//        "Number": "5208 8381 5687 8508",
//        "CVC": "624",
//        "Type": "Debit"
//      }
//    ]
//  }
