using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.DTO.ExportDTO;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        private static IMapper mapper;
        private static string InputJsonFile = "../../../Datasets";
        private static string ResultDirectoryPath = "../../../Datasets/Results";
        public static void Main(string[] args)
        {
            var db = new CarDealerContext();

            InitializeAutomapper();

            //ResetDatabase(db);

            // --- import into DB and deserialize ----

            //var inputJsonSupliers = File.ReadAllText(InputJsonFile + "/suppliers.json"); // read the input json
            //var result1 = ImportSuppliers(db, inputJsonSupliers); // invoke the method
            //Console.WriteLine(result1);

            //var inputJsonParts = File.ReadAllText(InputJsonFile + "/parts.json");
            //var result2 = ImportParts(db, inputJsonParts);
            //Console.WriteLine(result2);

            //var inputJsonCars = File.ReadAllText(InputJsonFile + "/cars.json");
            //var result3 = ImportCars(db, inputJsonCars);
            //Console.WriteLine(result3);

            //var inputJsonCustomers = File.ReadAllText(InputJsonFile + "/customers.json");
            //var result4 = ImportCustomers(db, inputJsonCustomers);
            //Console.WriteLine(result4);

            //var inputJsonSales = File.ReadAllText(InputJsonFile + "/sales.json");
            //var result5 = ImportSales(db, inputJsonSales);
            //Console.WriteLine(result5);


            // -- Export Data -- //

            EnsureDirectoryExists(ResultDirectoryPath); // check directory exists

            //var resultAsJson = GetOrderedCustomers(db); // result from the curr method
            //var resultAsJson = GetCarsFromMakeToyota(db);
            //var resultAsJson = GetLocalSuppliers(db);
            //var resultAsJson = GetCarsWithTheirListOfParts(db);
            var resultAsJson = GetTotalSalesByCustomer(db);
            //var resultAsJson = GetSalesWithAppliedDiscount(db);

            //File.WriteAllText(ResultDirectoryPath + "/ordered-customers.json", resultAsJson); // write result into the directory
            //File.WriteAllText(ResultDirectoryPath + "/toyota-cars.json", resultAsJson);
            //File.WriteAllText(ResultDirectoryPath + "/local-suppliers.json", resultAsJson);
            //File.WriteAllText(ResultDirectoryPath + "/cars-and-parts.json", resultAsJson);
            File.WriteAllText(ResultDirectoryPath + "/customers-total-sales.json", resultAsJson);
            //File.WriteAllText(ResultDirectoryPath + "/sales-discounts.json", resultAsJson);

            Console.WriteLine(resultAsJson); //print curr result
        }

        // -- Import Data -- //
        // 10. Import Suppliers - ok 100/100 -
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            InitializeAutomapper();

            // create DTO obj supliersDTO/(IEnumerable<SupliersInputModelDTO>) -> convert/desserialized from inputJson to IEnumerable<SupliersInputModelDTO>
            var supliersDTO = JsonConvert.DeserializeObject<IEnumerable<SupliersInputModelDTO>>(inputJson);

            // make mapp from IEnumerable<SupliersInputModelDTO> to IEnumerable<Supplier>
            var allSupliers = mapper.Map<IEnumerable<Supplier>>(supliersDTO);

            //add range collection
            context.Suppliers.AddRange(allSupliers);
            //save changes
            context.SaveChanges();

            return $"Successfully imported {allSupliers.Count()}.";
        }

        // 11. Import Parts - ok 100/100 -
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            InitializeAutomapper();

            // get all SupliersIds
            var allSupliersIds = context
                .Suppliers
                .Select(s => s.Id)
                .ToList();

            // If the supplierId doesn’t exists, skip the record.
            var partsDTO = JsonConvert
                .DeserializeObject<IEnumerable<Part>>(inputJson) // put <Part> instead of <PartsInputModelDTO>
                .Where(s => allSupliersIds.Contains(s.SupplierId)) // ok
                //.Where(s => context.Suppliers.Any(p => p.Id == s.SupplierId)) // ok
                .ToList();

            var allParts = mapper.Map<IEnumerable<Part>>(partsDTO);
            
            context.Parts.AddRange(allParts);
            context.SaveChanges();

            return $"Successfully imported {allParts.Count()}.";
        }

        // 12. Import Cars - ok 100/100 -
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            InitializeAutomapper();

            var carsDTO = JsonConvert.DeserializeObject<IEnumerable<CarsInputModelDTO>>(inputJson);

            var listOfCars = new List<Car>(); // <- collection to add into the DB

            // - 100/100 - with .Distinct(), else 50/100
            foreach (var currCar in carsDTO)
            {
                var newCar = new Car()
                {
                    Make = currCar.Make,
                    Model = currCar.Model,
                    TravelledDistance = currCar.TravelledDistance
                };

                foreach (var currPartId in currCar.PartsId.Distinct())
                {
                    newCar.PartCars.Add(new PartCar
                    {
                        PartId = currPartId
                    });
                }

                listOfCars.Add(newCar);
            }

            context.Cars.AddRange(listOfCars);

            //var allCars = mapper.Map<IEnumerable<Car>>(carsDTO); // dont need to use this

            context.SaveChanges();

            return $"Successfully imported {listOfCars.Count()}.";
        }

        // 13. Import Customers - ok 100/100 -
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            InitializeAutomapper();

            var customersDTO = JsonConvert.DeserializeObject<IEnumerable<CustomersInputModelDTO>>(inputJson);

            var allCustomers = mapper.Map<IEnumerable<Customer>>(customersDTO);

            context.Customers.AddRange(allCustomers);
            context.SaveChanges();

            return $"Successfully imported {allCustomers.Count()}.";
        }

        // 14. Import Sales 
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            InitializeAutomapper();

            var salesDTO = JsonConvert.DeserializeObject<IEnumerable<SalesInputModelDTO>>(inputJson);

            var allSales = mapper.Map<IEnumerable<Sale>>(salesDTO);

            context.Sales.AddRange(allSales);
            context.SaveChanges();

            return $"Successfully imported {allSales.Count()}.";
        }


        // -- Export Data -- //

        // 15. Export Ordered Customers - ok 100/100 -
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var allCustomers = context
                .Customers
                .OrderBy(x => x.BirthDate)
                .ThenBy(x => x.IsYoungDriver)
                .Select(c => new
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy"),
                    IsYoungDriver = c.IsYoungDriver
                })
                .ToList();

            var result = JsonConvert.SerializeObject(allCustomers, Formatting.Indented);

            return result;
        }

        // 16. Export Cars from Make Toyota - ok 100/100 -
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            //var allToyotaCars = context
            //    .Cars
            //    .Where(x => x.Make == "Toyota")
            //    .OrderBy(x => x.Model)
            //    .ThenByDescending(x => x.TravelledDistance)
            //    .Select(c => new
            //    {
            //        Id = c.Id,
            //        Make = c.Make,
            //        Model = c.Model,
            //        TravelledDistance = c.TravelledDistance
            //    })
            //    .ToList();

            //or use DTO - ok 100/100 - 
            var allToyotaCars = context
                .Cars
                .Where(x => x.Make == "Toyota")
                .ProjectTo<ToyotaCarsDTO>()
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .ToList();

            var settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            };

            var resultAsJson = JsonConvert.SerializeObject(allToyotaCars, settings);

            return resultAsJson;
        }

        // 17.	Export Local Suppliers - ok 100/100 -
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var localSuppliers = context
                .Suppliers
                .Where(x => x.IsImporter == false)
                .Select(s => new
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToList();

            var settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            };

            var resultAsJson = JsonConvert.SerializeObject(localSuppliers, settings);

            return resultAsJson;
        }

        // 18.	Export Cars with Their List of Parts - ok 100/100 -
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carsWithParts = context
                .Cars
                .Select(c => new
                {
                    car = new
                    {
                        Make = c.Make,
                        Model = c.Model,
                        TravelledDistance = c.TravelledDistance
                    },
                    parts = c.PartCars.Select(pc => new 
                    {
                        Name = pc.Part.Name,
                        Price = pc.Part.Price.ToString("f2")
                    })
                    //.ToArray()
                })
                .ToList();

            var settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            };

            var resultAsJson = JsonConvert.SerializeObject(carsWithParts, settings);

            return resultAsJson;
        }

        // 19.	Export Total Sales by Customer - NOT WORKING
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            // return empty array
            var totalSales = context
                .Customers
                .Where(x => x.Sales.Count >= 1)
                .Select(x => new
                {
                    fullName = x.Name,
                    spentMoney = x.Sales.Count,
                    boughtCars = x.Sales.Select(s => s.Car.PartCars.Select(pc => pc.Part.Price).Sum())
                    //boughtCars = x.Sales.Select(s => s.Car.PartCars.Select(pc => pc.Part).Sum(p => p.Price)).Sum()
                })
                .OrderByDescending(x => x.spentMoney)
                .ThenByDescending(x => x.boughtCars)
                .ToList();

            //InitializeAutomapper();


            //var totalSales = context
            //        .Customers
            //        .ProjectTo<TotalSalesDTO>()
            //        .Where(c => c.BoughtCars >= 1)
            //        .OrderByDescending(c => c.SpentMoney)
            //        .ThenByDescending(c => c.BoughtCars)
            //        .ToList();

            var resultAsJson = JsonConvert.SerializeObject(totalSales, Formatting.Indented);

            return resultAsJson; //return ->  Mapper not initialized. Call Initialize with appropriate configuration. If you are trying to use mapper instances through a container or otherwise, make sure you do not have any calls to the static Mapper.Map methods, and if you're using ProjectTo or UseAsDataSource extension methods, make sure you pass in the appropriate IConfigurationProvider instance.

            // but in Judge - 100/100
        }

        // 20.	Export Sales with Applied Discount - ok 100/100 -
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context
                    .Sales
                    .Select(s => new CustomerSaleDTO()
                    {
                        Car = new SaleDTO()
                        {
                            Make = s.Car.Make,
                            Model = s.Car.Model,
                            TravelledDistance = s.Car.TravelledDistance
                        },
                        CustomerName = s.Customer.Name,
                        Discount = s.Discount.ToString("f2"),
                        Price = s.Car.PartCars.Sum(pc => pc.Part.Price).ToString("f2"),
                        PriceWithDiscount = (s.Car.PartCars.Sum(pc => pc.Part.Price) -
                                            s.Car.PartCars.Sum(pc => pc.Part.Price) * s.Discount / 100).ToString("f2")

                    })
                    .Take(10)
                    .ToList();

            var json = JsonConvert.SerializeObject(sales, Formatting.Indented);

            return json;

        }




        // ----------------------- additional methods ---------------------------- //

        // create method ResetDatabase
        private static void ResetDatabase(CarDealerContext db)
        {
            db.Database.EnsureDeleted(); // first - deleted
            Console.WriteLine("Database was deleted!");

            db.Database.EnsureCreated(); // second - created
            Console.WriteLine("Database was created!");
        }

        // create method Automapper - initialize it into every inport method (due to Judge)
        private static void InitializeAutomapper()
        {
            // register/configure the mapper. dont forget to add field private static IMapper mapper;
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });

            mapper = config.CreateMapper();
        }

        //or
        //private static void InitializeAutomapper()
        //{
        //    // register/configure the mapper. dont forget to add field private static IMapper mapper;
        //    Mapper.Initialize(cfg =>
        //    {
        //        cfg.AddProfile<CarDealerProfile>();
        //    });
        //}

        // --- Method to create directory ---------
        private static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}