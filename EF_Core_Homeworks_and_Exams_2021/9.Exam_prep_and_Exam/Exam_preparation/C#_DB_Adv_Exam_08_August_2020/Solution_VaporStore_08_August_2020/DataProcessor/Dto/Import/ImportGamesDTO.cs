using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VaporStore.ImportResults
{
    public class ImportGamesDTO
    {
        //repeate all attributes that were allready put on entities, whike creating DB

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Price { get; set; }

        [Required]
        public DateTime? ReleaseDate { get; set; } // combine DateTime? and [Required] or use string as a type and validate into the method using ParseExact/TryParseExact

        [Required]
        public string Developer { get; set; }

        [Required]
        public string Genre { get; set; } 

        //[Required] cant validate it like this by using [Required]. Make it into the method or use [MinLength(1)] attribute
        [MinLength(1)]      // <- check, that array is not empty
        public string[] Tags { get; set; } // array of strings (if was array of objects -> make another DTO). IEnumerable<string> Tags will work too.
    }

}
//         --> for constraints <----
//	Id – integer, Primary Key
//	Name – text (required)
//	Price – decimal(non - negative, minimum value: 0)(required)
//	ReleaseDate – Date(required)
//	DeveloperId – integer, foreign key(required)
//	Developer – the game’s developer (required)
//	GenreId – integer, foreign key(required)
//	Genre – the game’s genre (required)
//	Purchases - collection of type Purchase
//	GameTags - collection of type GameTag. Each game must have at least one tag.

//{         ---> for properties < ----
//    "Name": "Dota 2",
//    "Price": 0,
//    "ReleaseDate": "2013-07-09",
//    "Developer": "Valve",
//    "Genre": "Action",
//    "Tags": [
//      "Multi-player",
//      "Co-op",
//      "Steam Trading Cards",
//      "Steam Workshop",
//      "SteamVR Collectibles",
//      "In-App Purchases",
//      "Valve Anti-Cheat enabled"
//    ]
//},

