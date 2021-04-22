using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DTO.Import
{
    [XmlType("Sale")]
    public class SalesImportDTO
    {
        [XmlElement("carId")]
        public int CarId { get; set; }

        [XmlElement("customerId")]
        public int CustomerId { get; set; }

        [XmlElement("discount")]
        public decimal Discount { get; set; }
    }
}
//< Sale >
//        < carId > 234 </ carId >
//        < customerId > 23 </ customerId >
//        < discount > 50 </ discount >
//    </ Sale >