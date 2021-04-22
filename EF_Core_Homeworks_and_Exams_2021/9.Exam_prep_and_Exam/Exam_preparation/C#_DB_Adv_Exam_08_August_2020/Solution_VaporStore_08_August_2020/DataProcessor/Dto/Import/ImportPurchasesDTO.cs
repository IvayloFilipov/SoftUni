using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;
using VaporStore.Data.Models;
using VaporStore.Data.Models.Enums;

namespace VaporStore.DataProcessor.Dto.Import
{
    [XmlType("Purchase")]
    public class ImportPurchasesDTO
    {
        [Required]
        [XmlAttribute("title")]
        public string GameName { get; set; }

        [Required]
        [XmlElement("Type")]
        public PurchaseType? Type { get; set; }

        [Required]
        [RegularExpression(@"^[0-9A-Z]{4}-[0-9A-Z]{4}-[0-9A-Z]{4}$")]
        [XmlElement("Key")]
        public string Key { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{4} [0-9]{4} [0-9]{4} [0-9]{4}$")]
        [XmlElement("Card")]
        public string Card { get; set; } //into the exaple-> the value tell me to use Regex to validate it

        [XmlElement("Date")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy HH:mm}")] ???
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "dd/MM/yyyy HH:mm")] ???
        public string Date { get; set; } //or use strin and parse it (DateTime.TryParseExact())
    }
}
//<Purchases>
//  <Purchase title="Dungeon Warfare 2">
//    <Type>Digital</Type>
//    <Key>ZTZ3-0D2S-G4TJ</Key>
//    <Card>1833 5024 0553 6211</Card>
//    <Date>07/12/2016 05:49</Date>
//  </Purchase>

//	Id – integer, Primary Key
//	Type – enumeration of type PurchaseType, with possible values (“Retail”, “Digital”) (required)
//•	ProductKey – text, which consists of 3 pairs of 4 uppercase Latin letters and digits, separated by dashes (ex. “ABCD-EFGH-1J3L”) (required)
//•	Date – Date(required)
//•	CardId – integer, foreign key(required)
//•	Card – the purchase’s card (required)
//•	GameId – integer, foreign key(required)
//•	Game – the purchase’s game (required)