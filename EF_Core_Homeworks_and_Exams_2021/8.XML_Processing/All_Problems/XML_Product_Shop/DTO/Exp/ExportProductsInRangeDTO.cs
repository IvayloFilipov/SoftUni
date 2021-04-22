using System.Xml.Serialization;

namespace ProductShop.DTO.Exp
{
    [XmlType("Product")]
    public class ExportProductsInRangeDTO
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }

        [XmlElement("buyer")]
        public string Buyer { get; set; }
    }
}
//<Products>
//  <Product>
//    <name>TRAMADOL HYDROCHLORIDE</name>
//    <price>516.48</price>
//    <buyer>Wallas Duffyn</buyer>
//  </Product>
