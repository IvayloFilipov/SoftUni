using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;

using CarDealer.Data;
using CarDealer.DTO.Export;
using CarDealer.DTO.Import;
using CarDealer.Models;

using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Text;

namespace CarDealer
{
    public class StartUp
    {
        private static IMapper mapper;

        //private static string DatasetsPath = "../../../Datasets/";
        private static string ResultsDirectoryPath = "../../../Datasets/Results/";

        public static void Main(string[] args)
        {
            var db = new CarDealerContext();

            //ResetDatabase(db);

            //reading input from Datasets
            //var suplierInputXml = File.ReadAllText(DatasetsPath + "suppliers.xml");
            //var partsInputXml = File.ReadAllText(DatasetsPath + "parts.xml");
            //var carsInputXml = File.ReadAllText(DatasetsPath + "cars.xml");
            //var customersInputXml = File.ReadAllText(DatasetsPath + "customers.xml");
            //var salesInputXml = File.ReadAllText(DatasetsPath + "sales.xml");

            //invoke the curr method
            //var result = ImportSuppliers(db, suplierInputXml);
            //var result = ImportParts(db, partsInputXml);
            //var result = ImportCars(db, carsInputXml);
            //var result = ImportCustomers(db, customersInputXml);
            //var result = ImportSales(db, salesInputXml);

            //print on Console
            //Console.WriteLine(result);

            // EXPORT //
            EnsureDirectoryExists(ResultsDirectoryPath);

            //invoke the curr method
            //var getDistance = GetCarsWithDistance(db);
            //var carsBMW = GetCarsFromMakeBmw(db);
            //var localeSupliers = GetLocalSuppliers(db);
            //var listOfParts = GetCarsWithTheirListOfParts(db);
            //var totalSalesByCustomer = GetTotalSalesByCustomer(db);
            var salesWithDiscount = GetSalesWithAppliedDiscount(db);

            //write the result into the desired directory
            //File.WriteAllText(ResultsDirectoryPath + "cars.xml", getDistance);
            //File.WriteAllText(ResultsDirectoryPath + "bmw-cars.xml", carsBMW);
            //File.WriteAllText(ResultsDirectoryPath + "local-suppliers.xml", localeSupliers);
            //File.WriteAllText(ResultsDirectoryPath + "cars-and-parts.xml", listOfParts);
            //File.WriteAllText(ResultsDirectoryPath + "customers-total-sales.xml", totalSalesByCustomer);
            File.WriteAllText(ResultsDirectoryPath + "sales-discounts.xml", salesWithDiscount);

            //print on Console
            Console.WriteLine(salesWithDiscount);

        }

        // IMPORT //
        // Query 9. Import Suppliers - 100/100
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            InitializeAutomapper();

            // 1. Create serializer
            var xmlSerializer = new XmlSerializer(typeof(SuppliersImportDTO[]), new XmlRootAttribute("Suppliers"));

            // 2. Create StrinReader -> read from input string
            using var textReader = new StringReader(inputXml);

            // 3. Deserialize the text
            var supplierDTO = xmlSerializer.Deserialize(textReader) as SuppliersImportDTO[];

            // 4. Map from supplierDTO to Supplier[]
            var suppliers = mapper.Map<Supplier[]>(supplierDTO);

            // 5. AddRange and save
            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Length}";
        }

        // Query 10. Import Parts - 100/100 -
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            InitializeAutomapper();

            var xmlSerializer = new XmlSerializer(typeof(PartsImportDTO[]), new XmlRootAttribute("Parts"));

            using var textReader = new StringReader(inputXml);

            var partsDTO = ((PartsImportDTO[])xmlSerializer.Deserialize(textReader))
                .Where(p => context.Suppliers.Any(s => s.Id == p.SupplierId));
            //.ToArray(); // -> work with or without ToArray();

            var parts = mapper.Map<Part[]>(partsDTO);

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Length}";
        }

        // Query 11. Import Cars - 100/100 -
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportCarsDTO[]), new XmlRootAttribute("Cars"));

            using var textReader = new StringReader(inputXml);

            var carsDtos = (ImportCarsDTO[])xmlSerializer.Deserialize(textReader);// as ImportCarsDTO[];

            var cars = new List<Car>();
            var partsOfCar = new List<PartCar>();

            foreach (var currCarDto in carsDtos)
            {
                //manual mapping
                var car = new Car()
                {
                    Make = currCarDto.Make,
                    Model = currCarDto.Model,
                    TravelledDistance = currCarDto.TravelledDistance
                };

                var distPars = currCarDto
                    .PartsCollection // proprety name from ImportCarsDTO -> represent all id's
                    .Where(pdtos => context.Parts.Any(p => p.Id == pdtos.Id))
                    .Select(p => p.Id)
                    .Distinct();

                foreach (var curPartId in distPars)
                {
                    //manual mapping
                    var parts = new PartCar()
                    {
                        PartId = curPartId,
                        Car = car
                    };

                    partsOfCar.Add(parts);
                }

                cars.Add(car);
            }

            context.AddRange(cars);
            context.AddRange(partsOfCar);

            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        // Query 12. Import Customers - 100/100 -
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            InitializeAutomapper();

            var xmlSerializer = new XmlSerializer(typeof(CustomersImportDTO[]), new XmlRootAttribute("Customers"));

            using var textReader = new StringReader(inputXml);

            var customersDTO = xmlSerializer.Deserialize(textReader) as CustomersImportDTO[];

            var customers = mapper.Map<Customer[]>(customersDTO);

            context.Customers.AddRange(customers);
            context.SaveChanges();


            return $"Successfully imported {customers.Length}";
        }

        // Query 13. Import Sales
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            InitializeAutomapper();

            var xmlSerializer = new XmlSerializer(typeof(SalesImportDTO[]), new XmlRootAttribute("Sales"));

            using var textReader = new StringReader(inputXml);

            var salesDTO = ((SalesImportDTO[])xmlSerializer.Deserialize(textReader))
                .Where(cdto => context.Cars.Any(c => c.Id == cdto.CarId))
                .ToArray();

            var sales = mapper.Map<Sale[]>(salesDTO);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Length}";
        }

        // EXPORT //
        // Query 14. Cars With Distance - 100/100 -
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            InitializeAutomapper();

            var allCars = context
                .Cars
                .Where(x => x.TravelledDistance > 2_000_000)
                .OrderBy(x => x.Make)
                .ThenBy(x => x.Model)
                // manual mapping
                //.Select(c => new ExportCarsWithDistanceDTO
                //{
                //    Make = c.Make,
                //    Model = c.Model,
                //    TravelledDistance = c.TravelledDistance
                //})
                .ProjectTo<ExportCarsWithDistanceDTO>(mapper.ConfigurationProvider) 
                .Take(10)
                .ToArray();

            var xmlSerialiser = new XmlSerializer(typeof(ExportCarsWithDistanceDTO[]), new XmlRootAttribute("cars"));

            using var textWriter = new StringWriter();

            //clear unnecessery namespaces into the project tag
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty); //or namespaces.Add("", "");

            xmlSerialiser.Serialize(textWriter, allCars, namespaces);

            var result = textWriter.ToString();

            return result;
        }

        // Query 15. Cars from make BMW - 100/100 -
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            InitializeAutomapper();

            var bmwCars = context
                .Cars
                .Where(x => x.Make == "BMW")
                .ProjectTo<ExportGetCarsFromMakeBmwDTO>(mapper.ConfigurationProvider)
                //manual mapping - 100/100
                //.Select(c => new ExportGetCarsFromMakeBmwDTO
                //{
                //    Id = c.Id,
                //    Model = c.Model,
                //    TravelledDistance = c.TravelledDistance
                //})
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportGetCarsFromMakeBmwDTO[]), new XmlRootAttribute("cars"));

            using var textWriter = new StringWriter();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            xmlSerializer.Serialize(textWriter, bmwCars, namespaces);

            var result = textWriter.ToString();

            return result;
        }

        // Query 16. Local Suppliers
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            InitializeAutomapper();

            var localSupliers = context
                .Suppliers
                .Where(s => s.IsImporter == false)
                .ProjectTo<ExportGetLocalSuppliersDTO>(mapper.ConfigurationProvider)
                //manual mapping - 100/100
                //.Select(s => new ExportGetLocalSuppliersDTO
                //{
                //    Id = s.Id,
                //    Name = s.Name,
                //    PartsCount = s.Parts.Count
                //})
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportGetLocalSuppliersDTO[]), new XmlRootAttribute("suppliers"));

            using var textWriter = new StringWriter();

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            xmlSerializer.Serialize(textWriter, localSupliers, ns);

            var result = textWriter.ToString();

            return result;
        }

        // Query 17. Cars with Their List of Parts - 100/100 -
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            InitializeAutomapper();

            var carsAndParts = context
                .Cars
                .ProjectTo<ExportGetCarsWithTheirListOfPartsDTO>(mapper.ConfigurationProvider)
                //manual mapping - 100/100
                //.Select(c => new ExportGetCarsWithTheirListOfPartsDTO
                //{
                //    Make = c.Make,
                //    Model = c.Model,
                //    TravelledDistance = c.TravelledDistance,
                //    Parts = c.PartCars.Select(p => new ExportInsideListOfPartsDTO
                //    {
                //        Name = p.Part.Name,
                //        Price = p.Part.Price
                //    })
                //    .OrderByDescending(x => x.Price)
                //    .ToArray()
                //})
                .OrderByDescending(x => x.TravelledDistance)
                .ThenBy(x => x.Model)
                .Take(5)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportGetCarsWithTheirListOfPartsDTO[]), new XmlRootAttribute("cars"));

            using var textWriter = new StringWriter();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            //var sb = new StringBuilder();

            xmlSerializer.Serialize(textWriter, carsAndParts, namespaces);
            //xmlSerializer.Serialize(new StringWriter(sb), carsAndParts, namespaces);

            var result = textWriter.ToString();
            //var result = sb.ToString().Trim();

            return result;
        }

        // Query 18. Total Sales by Customer - 100/100 -
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            InitializeAutomapper();

            var customersWithCars = context
                .Customers
                .Where(c => c.Sales.Count > 0)
                //.ProjectTo<ExportGetTotalSalesByCustomerDTO>(mapper.ConfigurationProvider)
                //manual mapping - 100/100 -
                .Select(c => new ExportGetTotalSalesByCustomerDTO
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count,
                    //SpentMoney = c.Sales.Select(s => s.Car.PartCars.Select(pc => pc.Part).Sum(p => p.Price)).Sum() //ok
                    SpentMoney = c.Sales.Select(s => s.Car).SelectMany(p => p.PartCars).Sum(pc => pc.Part.Price) //ok
                })
                .OrderByDescending(x => x.SpentMoney)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportGetTotalSalesByCustomerDTO[]), new XmlRootAttribute("customers"));

            using var textWriter = new StringWriter();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            xmlSerializer.Serialize(textWriter, customersWithCars, namespaces);

            var result = textWriter.ToString();

            return result;
        }

        // Query 19. Sales with Applied Discount
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            //Get all sales with information about the car, customer and price of the sale with and without discount.
            InitializeAutomapper();

            var sales = context
                .Sales
                //.ProjectTo<ExportSalesWithDiscountDTO>(mapper.ConfigurationProvider)
                //manual mapping - 100/100 -
                .Select(s => new ExportSalesWithDiscountDTO
                {
                    Discount = s.Discount,
                    CustomeName = s.Customer.Name,
                    Price = s.Car.PartCars.Sum(pc => pc.Part.Price),
                    PriceWithDiscount = s.Car.PartCars.Sum(pc => pc.Part.Price) -
                                        s.Car.PartCars.Sum(pc => pc.Part.Price) * s.Discount / 100,
                    CarInfo = new ExportIInnerSalesWithDiscountDTO
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TravelledDistance = s.Car.TravelledDistance
                    }
                })
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportSalesWithDiscountDTO[]), new XmlRootAttribute("sales"));

            var textWriter = new StringWriter();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            xmlSerializer.Serialize(textWriter, sales, namespaces);

            var result = textWriter.ToString();

            return result;
        }


        // --- additional methods --- //
        private static void ResetDatabase(CarDealerContext db)
        {
            db.Database.EnsureDeleted(); // first - deleted
            Console.WriteLine("Database was deleted!");

            db.Database.EnsureCreated(); // second - created
            Console.WriteLine("Database was created!");
        }

        private static void InitializeAutomapper()
        {
            // Configure the mapper. Dont forget to add field -> private static IMapper mapper;
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });

            //mapper = (Mapper)config.CreateMapper();
            mapper = config.CreateMapper();
        }


        private static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}