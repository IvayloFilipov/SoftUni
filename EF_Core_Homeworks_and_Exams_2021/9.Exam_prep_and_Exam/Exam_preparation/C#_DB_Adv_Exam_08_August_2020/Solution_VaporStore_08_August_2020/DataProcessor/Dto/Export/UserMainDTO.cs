using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace VaporStore.DataProcessor.Dto.Export
{
    [XmlType("User")]
    public class UserMainDTO
    {
        [XmlAttribute("username")]
        public string Username { get; set; }

        [XmlArray("Purchases")]
        public PurchaseDTO[] Purchases { get; set; }

        [XmlElement("TotalSpent")]
        public decimal TotatlSpent { get; set; }
    }

    [XmlType("Purchase")]
    public class PurchaseDTO
    {
        [XmlElement("Card")]
        public string Card { get; set; }

        [XmlElement("Cvc")]
        public string Cvc { get; set; }

        [XmlElement("Date")]
        public string Date { get; set; } // was DateTime make it to string if needed

        [XmlElement("Game")]
        public GameDTO Game { get; set; }
    }

    [XmlType("Game")]
    public class GameDTO
    {
        [XmlAttribute("title")]
        public string Title { get; set; }

        [XmlElement("Genre")]
        public string Genre { get; set; }

        [XmlElement("Price")]
        public decimal Price { get; set; }
    }
}
