using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductShop.DataTransferObjects.Users
{
    public class UserWithSoldProductsDTO
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }


        [JsonProperty("lastName")]
        public string LastName { get; set; }

        //this below is an array from inner DTO
        [JsonProperty("soldProducts")]
        public UserSoldProductsDTO[] SoldProducts { get; set; }
    }
}
