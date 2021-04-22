using CarDealer.Models;
using CarDealer.DTO.Import;

using AutoMapper;
using CarDealer.DTO.Export;
using System.Linq;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            // -- Import -- //
            // Query 9. Import Suppliers
            this.CreateMap<SuppliersImportDTO, Supplier>();

            // Query 10. Import Parts
            this.CreateMap<PartsImportDTO, Part>();

            // Query 11. Import Cars - no auto mapper used

            // Query 12. Import Customers 
            this.CreateMap<CustomersImportDTO, Customer>();

            // Query 13. Import Sales
            this.CreateMap<SalesImportDTO, Sale>();

            // -- Export -- //
            // Query 14. Cars With Distance
            this.CreateMap<Car, ExportCarsWithDistanceDTO>();

            // Query 15. Cars from make BMW
            this.CreateMap<Car, ExportGetCarsFromMakeBmwDTO>();

            // Query 16. Local Suppliers
            this.CreateMap<Supplier, ExportGetLocalSuppliersDTO>()
                .ForMember(x => x.PartsCount, y => y.MapFrom(s => s.Parts.Count));

            // Query 17. Cars with Their List of Parts
            // must make this Select and OrderByDescending here inside of the Profiler
            this.CreateMap<Car, ExportGetCarsWithTheirListOfPartsDTO>()
                .ForMember(x => x.Parts, y => y.MapFrom(s => s.PartCars.Select(pc => pc.Part)
                                                                       .OrderByDescending(p => p.Price)));

            this.CreateMap<Part, ExportInsideListOfPartsDTO>();

            // Query 18. Total Sales by Customer
            this.CreateMap<Customer, ExportGetTotalSalesByCustomerDTO>()
                .ForMember(x => x.FullName, y => y.MapFrom(s => s.Name))
                .ForMember(x => x.BoughtCars, y => y.MapFrom(s => s.Sales.Count))
                //.ForMember(x => x.SpentMoney, y => y.MapFrom(s => s.Sales.Select(sl => sl.Car.PartCars.Select(p => p.Part)
                //                                                                            .Sum(pc => pc.Price)).Sum())); //ok
                .ForMember(x => x.SpentMoney, y => y.MapFrom(s => s.Sales.Select(sl => sl.Car)
                                                                         .SelectMany(p => p.PartCars)
                                                                         .Sum(pc => pc.Part.Price))); //ok

            // Query 19. Sales with Applied Discount - no need for manual mapping in this case
                // if use Automapper =>
            //this.CreateMap<Sale, ExportSalesWithDiscountDTO>();
            //this.CreateMap<Car, ExportSalesWithDiscountDTO>();
        }
    }
}
