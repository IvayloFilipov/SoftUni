using System;
using System.Collections.Generic;
using System.Text;

namespace CarDealer.Models
{
    public class Part
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public int SupplierId { get; set; } // FK
        public Supplier Supplier { get; set; } // Navigational prop

        public ICollection<PartCar> PartCars { get; set; } = new HashSet<PartCar>();
    }
}
