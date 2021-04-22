using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductShop.DataTransferObjects.Users
{
    // inner DTO
    public class UserSoldProductsDTO
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("buyerFirstName")]
        public string BuyerFirstName { get; set; }

        [JsonProperty("buyerLastName")]
        public string BuyerLastName { get; set; }

    }
}
        //name ": " Metoprolol Tartrate",
        //"price": 1405.74,
        //"buyerFirstName": "Bonnie",
        //"buyerLastName": "Fox"
