﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.DTO.Imp
{
    [XmlType("CategoryProduct")]
    public class ImportCategoryProductsDTO
    {
        [XmlElement("CategoryId")]
        public int? CategoryId { get; set; }

        [XmlElement("ProductId")]
        public int? ProductId { get; set; }
    }
}
//< CategoryProducts >
//    < CategoryProduct >
//        < CategoryId > 4 </ CategoryId >
//        < ProductId > 1 </ ProductId >
//    </ CategoryProduct >