﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.DTO.Imp
{
    [XmlType("User")]
    public class ImportUsersDTO
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlElement("age")]
        public int Age { get; set; }
    }
}
//< Users >
//    < User >
//        < firstName > Chrissy </ firstName >
//        < lastName > Falconbridge </ lastName >
//        < age > 50 </ age >
//    </ User >