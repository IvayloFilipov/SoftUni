using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarDealer.DTO
{
    public class CarsInputModelDTO
    {
        [JsonProperty("make")]
        public string Make { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("travelledDistance")]
        public long TravelledDistance { get; set; }

        [JsonProperty("partsId")]
        public IEnumerable<int> PartsId { get; set; }
    }
}
//[
//  {
//    "make": "Opel",
//    "model": "Omega",
//    "travelledDistance": 176664996,
//    "partsId": [
//      38,
//      102,
//      23,
//      116,
//      46,
//      68,
//      88,
//      104,
//      71,
//      32,
//      114
//    ]
//  },
//  {
//    "make": "Opel",
//    "model": "Omega",
//    "travelledDistance": 176664996,
//    "partsId"
//  }
//]