using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.DTO.Exp
{
    [XmlType("Product")]
    public class ProductsSoldDTO
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }
    }
}
//< Users > - root
//  < User > - outer DTO
//    < firstName > Almire </ firstName > - prop
//    < lastName > Ainslee </ lastName > - prop
//    < soldProducts >                      <- collection (name into outer DTO)
//      < Product > - inner DTO
//        < name > olio activ mouthwash </name> - prop
//        < price > 206.06 </price> - prop
//      </Product>  
//    < /soldProducts >
//  < /User >