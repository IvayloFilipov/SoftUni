using System.Collections.Generic;

namespace P03_SalesDatabase.Data.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public double Quantity { get; set; }
        public decimal Price { get; set; }
        public ICollection<Sale> Sales { get; set; } = new HashSet<Sale>();
    }
}
//	ProductId
//	Name(up to 50 characters, unicode)
//	Quantity(real number)
//	Price
//	Sales
