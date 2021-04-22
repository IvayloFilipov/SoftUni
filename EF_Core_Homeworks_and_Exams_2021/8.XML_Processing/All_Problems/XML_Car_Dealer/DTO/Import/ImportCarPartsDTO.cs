using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DTO.Import
{
    [XmlType("partId")]
    public class ImportCarPartsDTO
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}
//< parts > collection
//      < partId id = "39" />
//       < partId id = "62" />
//        < partId id = "72" />
//       </ parts >