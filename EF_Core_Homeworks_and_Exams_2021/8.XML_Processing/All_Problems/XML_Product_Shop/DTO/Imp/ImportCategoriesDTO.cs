using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.DTO.Imp
{
    [XmlType("Category")]
    public class ImportCategoriesDTO
    {
        [XmlElement("name")]
        public string Name { get; set; }
    }
}
//< Categories >
//    < Category >
//        < name > Drugs </ name >
//    </ Category >