using System;
using System.Collections.Generic;
using System.Text;

namespace ProductShop.DataTransferObjects
{
    public class ProductInputModelDTO
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int SellerId { get; set; }
        public int? BuyerId { get; set; } // important to add ? as it is in Product.cs
    }
}
