using System;
using System.Collections.Generic;
using System.Text;

namespace ProductShop.DataTransferObjects
{
    public class CategoriesInputModelDTO
    {
        // here into the DTO we insert as property info from the json (categories.json), whitch is only 'name'
        public string Name { get; set; }
    }
}
