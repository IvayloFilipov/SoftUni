using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using CarDealer.DTO;
using CarDealer.DTO.ExportDTO;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            // --- inport json ---

            // 10. Import Suppliers - Automap from SupliersInputModelDTO to Suplier
            this.CreateMap<SupliersInputModelDTO, Supplier>();

            // 11. Import Parts - Automapper from  PartsInputModelDTO to Part
            this.CreateMap<PartsInputModelDTO, Part>();

            // 12. Import Cars - Automap from CarsInputModelDTO to Car
            this.CreateMap<CarsInputModelDTO, Car>();

            // 13. Import Customers - Automap from CustomersInputModelDTO to Customer
            this.CreateMap<CustomersInputModelDTO, Customer>();

            // 14. Import Sales - Automapp from SalesInputModelDTO to Sale
            this.CreateMap<SalesInputModelDTO, Sale>();


            // --- export C# obj to json ---

            // 16. Export Cars from Make Toyota - Automap from Car to ToyotaCarsDTO
            this.CreateMap<Car, ToyotaCarsDTO>();

            // 19.	Export Total Sales by Customer - Automap from Customer  to TotalSalesDTO
            //this.CreateMap<Customer, TotalSalesDTO>()
            //    .ForMember(x => x.FullName, y => y.MapFrom(s => s.Name))
            //    .ForMember(x => x.BoughtCars, y => y.MapFrom(s => s.Sales.Count))
            //    .ForMember(x => x.SpentMoney, y => y.MapFrom(s => s.Sales.Select(x => x.Car.PartCars.Select(pc => pc.Part).Sum(pc => pc.Price)).Sum()));

            this.CreateMap<Customer, TotalSalesDTO>()
                .ForMember(x => x.FullName, y => y.MapFrom(s => s.Name))
                .ForMember(x => x.BoughtCars, y => y.MapFrom(s => s.Sales.Count))
                .ForMember(x => x.SpentMoney, y => y.MapFrom(s => s.Sales
                                                                   .Select(z => z.Car
                                                                                 .PartCars
                                                                                 .Select(pc => pc.Part)
                                                                                 .Sum(pc => pc.Price))
                                                                   .Sum()));
        }
    }
}
