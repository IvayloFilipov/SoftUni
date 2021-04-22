using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarDealer.DTO
{
    public class PartsInputModelDTO
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("supplierId")]
        public int SupplierId { get; set; }
    }
}
    //"name": "Unexposed bumper",
    //"price": 1003.34,
    //"quantity": 10,
    //"supplierId": 12