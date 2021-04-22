using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarDealer.DTO.ExportDTO
{
    public class TotalSalesDTO
    {
        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [JsonProperty("boughtCars")]
        public int BoughtCars { get; set; }

        [JsonProperty("spentMoney")]
        public decimal SpentMoney { get; set; }
    }
}
    //"fullName": " Johnette Derryberry",
    //"boughtCars": 5,
    //"spentMoney": 13529.25
