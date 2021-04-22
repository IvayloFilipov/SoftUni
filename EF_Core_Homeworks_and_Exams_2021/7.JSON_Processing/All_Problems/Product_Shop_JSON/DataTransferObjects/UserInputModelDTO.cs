using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductShop.DataTransferObjects
{
    public class UserInputModelDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? Age { get; set; }

        //and now go to StartUp and add this DTO into the DesirializeObject<IEnumerable<UserInputModelDTO>>
    }
}
