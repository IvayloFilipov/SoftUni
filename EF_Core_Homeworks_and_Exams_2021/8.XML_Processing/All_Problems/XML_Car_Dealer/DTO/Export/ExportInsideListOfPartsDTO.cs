using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DTO.Export
{
    [XmlType("part")]
    public class ExportInsideListOfPartsDTO
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("price")]
        public decimal Price { get; set; }
    }
}
//<cars>
//  <car make="Opel" model="Astra" travelled-distance="516628215">
//    <parts>
//      <part name="Master cylinder" price="130.99" />
//      <part name="Water tank" price="100.99" />
//      <part name="Front Right Side Inner door handle" price="100.99" />
//    </parts>
//  </car>
//</cars>

//< parts >
//           < part name = "Master cylinder" price = "130.99" />
//           < part name = "Water tank" price = "100.99" />
//           < part name = "Front Right Side Inner door handle" price = "100.99" />
//         </ parts >