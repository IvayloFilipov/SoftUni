using System;

namespace P03_SalesDatabase.Data.Models
{
    public class Sale
    {
        public int SaleId { get; set; }

        public DateTime Date { get; set; }

        public int ProductId { get; set; } // Foreign Key
        public Product Product { get; set; } // Navigational property

        public int CustomerId { get; set; } // Foreign Key
        public Customer Customer { get; set; } // Navigational property

        public int StoreId { get; set; } // Foreign Key
        public Store Store { get; set; } // Navigational property
    }
}
//	SaleId
//	Date
//	Product
//	Customer
//	Store

