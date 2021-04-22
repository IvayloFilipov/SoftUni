using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.DTO.Exp
{
    [XmlType("User")]
    public class ExportSoldProductsDTO
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlArray("soldProducts")]
        public ProductsSoldDTO[] ProductsSold { get; set; }
    }
}
//< Users > - root
//  < User > - outer DTO
//    < firstName > Almire </ firstName > - prop
//    < lastName > Ainslee </ lastName > - prop
//    < soldProducts >                      <- collection (name it here, into outer DTO)
//      < Product > - inner DTO
//        < name > olio activ mouthwash </name> - prop
//        < price > 206.06 </price> - prop
//      </Product>  
//    < /soldProducts >
//  < /User >