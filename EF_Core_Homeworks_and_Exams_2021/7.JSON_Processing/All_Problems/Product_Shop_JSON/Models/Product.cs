namespace ProductShop.Models
{
    using System.Collections.Generic;

    public class Product
    {
        public Product()
        {
            this.CategoryProducts = new List<CategoryProduct>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public int SellerId { get; set; } // FK
        public User Seller { get; set; } // Nav prop

        public int? BuyerId { get; set; } // FK
        public User Buyer { get; set; } // Nav prop

        public ICollection<CategoryProduct> CategoryProducts { get; set; }
    }
}