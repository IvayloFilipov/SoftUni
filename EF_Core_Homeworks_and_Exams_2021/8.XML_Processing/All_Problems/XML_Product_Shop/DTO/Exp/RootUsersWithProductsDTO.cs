using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.DTO.Exp
{
    [XmlType("Users")]
    public class RootUsersWithProductsDTO
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("users")]
        public ExportUsersDTO[] Users { get; set; }
    }

    [XmlType("User")]
    public class ExportUsersDTO
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlElement("age")]
        public int? Age { get; set; }

        [XmlElement("SoldProducts")]
        public InnerSoldProducsDTO InnerSoldProducs { get; set; }
    }

    [XmlType("SoldProducts")]
    public class InnerSoldProducsDTO
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("products")]
        public InnerDTO[] Products { get; set; }
    }

    [XmlType("Product")]
    public class InnerDTO
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }
    }
}
//< Users >                                      <- root / Outer DTO
//  < count > 54 </ count >                             <- prop
//  < users >                                    <- collection
//    < User >                                        <-midle DTO
//      < firstName > Cathee </ firstName >             <- prop
//      < lastName > Rallings </ lastName >             <- prop
//      < age > 33 </ age >                             <- prop
//      < SoldProducts >                              <- inner midle DTO
//        < count > 9 </ count >                        <- prop
//        < products >                            <- collection
//          < Product >                             <- inner DTO
//            < name > Fair Foundation SPF 15</name>    <- prop
//            < price >1394.24</ price >                <- prop
//          </Product>
