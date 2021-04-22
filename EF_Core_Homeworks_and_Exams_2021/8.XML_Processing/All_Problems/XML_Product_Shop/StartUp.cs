using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using AutoMapper;
using AutoMapper.QueryableExtensions;

using ProductShop.Data;
using ProductShop.DTO.Exp;
using ProductShop.DTO.Imp;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        private static IMapper mapper;
        private static string DatasetsPath = "../../../Datasets/";
        private static string ResultsDirectoryPath = "../../../Datasets/Results/";

        public static void Main(string[] args)
        {
            var db = new ProductShopContext();

            EnsureDirectoryExists(ResultsDirectoryPath);

            //ResetDatabase(db);

            // -- IMPORT -- //
            //var importUsers = File.ReadAllText(DatasetsPath + "users.xml");
            //var importProducts = File.ReadAllText(DatasetsPath + "products.xml");
            //var importCategories = File.ReadAllText(DatasetsPath + "categories.xml");
            //var importCategoriesProducts = File.ReadAllText(DatasetsPath + "categories-products.xml");

            //var result = ImportUsers(db, importUsers);
            //var result = ImportProducts(db, importProducts);
            //var result = ImportCategories(db, importCategories);
            //var result = ImportCategoryProducts(db, importCategoriesProducts);

            //Console.WriteLine(result);

            // -- EXPORT -- //
            //var exportProducts = GetProductsInRange(db);
            //var soldProducts = GetSoldProducts(db);
            //var categoryByProductCount = GetCategoriesByProductsCount(db);
            var usersWithProducts = GetUsersWithProducts(db);

            //File.WriteAllText(ResultsDirectoryPath + "products-in-range.xml", exportProducts);
            //File.WriteAllText(ResultsDirectoryPath + "users-sold-products.xml", soldProducts);
            //File.WriteAllText(ResultsDirectoryPath + "categories-by-products.xml", categoryByProductCount);
            File.WriteAllText(ResultsDirectoryPath + "users-and-products.xml", usersWithProducts);

            Console.WriteLine(usersWithProducts);
        }
        // -- IMPORT -- //
        // Query 1. Import Users - 100/100 -
        public static string ImportUsers(ProductShopContext context, string inputXml)
        { 
            InitializeAutomapper();
            // 1.
            var xmlSerializer = new XmlSerializer(typeof(ImportUsersDTO[]), new XmlRootAttribute("Users"));
            // 2.
            using var textReader = new StringReader(inputXml);
            // 3.
            var supplierDTO = (ImportUsersDTO[])xmlSerializer.Deserialize(textReader);
            // 4. 
            var users = mapper.Map<User[]>(supplierDTO);

            // 5. 
            context.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Length}";
        }

        // Query 2. Import Products - 100/100 -
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            InitializeAutomapper();

            var xmlSerializer = new XmlSerializer(typeof(ImportProductsDTO[]), new XmlRootAttribute("Products"));

            using var textReader = new StringReader(inputXml);

            var productDTO = (ImportProductsDTO[])xmlSerializer.Deserialize(textReader);

            var products = mapper.Map<Product[]>(productDTO);

            context.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Length}";
        }

        // Query 3. Import Categories - 100/100 -
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            InitializeAutomapper();

            var xmlSerializer = new XmlSerializer(typeof(ImportCategoriesDTO[]), new XmlRootAttribute("Categories"));

            using var textReader = new StringReader(inputXml);

            var categoryDTO = (ImportCategoriesDTO[])xmlSerializer.Deserialize(textReader);

            //If some names are null, you don’t have to add them in the database and Skip the record.
            var categories = mapper.Map<Category[]>(categoryDTO)
                .Where(x => x.Name != null)
                .ToArray();

            context.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Length}";
        }

        // Query 4. Import Categories and Products - 100/100 -
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            InitializeAutomapper();

            var xmlSerializer = new XmlSerializer(typeof(ImportCategoryProductsDTO[]), new XmlRootAttribute("CategoryProducts"));

            var textReader = new StringReader(inputXml);

            var catecoryProductsDTO = (ImportCategoryProductsDTO[])xmlSerializer.Deserialize(textReader);

            //If provided category or product id, doesn’t exists, skip the whole entry!
            var categoryProducts = mapper.Map<CategoryProduct[]>(catecoryProductsDTO)
                .Where(x => context.Categories.Any(c => c.Id == x.CategoryId) && context.Products.Any(p => p.Id == x.ProductId))
                .ToArray();

            context.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Length}";
        }

        // -- EXPORT -- //
        // Query 5. Products In Range - 100/100
        public static string GetProductsInRange(ProductShopContext context)
        {
            InitializeAutomapper();

            var productsInRange = context
                .Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .ProjectTo<ExportProductsInRangeDTO>(mapper.ConfigurationProvider) // ok
                //manual mapping - 100/100 -
                //.Select(p => new ExportProductsInRangeDTO
                //{
                //    Name = p.Name,
                //    Price = p.Price,
                //    Buyer = p.Buyer.FirstName + " " + p.Buyer.LastName
                //})
                .OrderBy(x => x.Price)
                .Take(10)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportProductsInRangeDTO[]), new XmlRootAttribute("Products"));

            using var textWriter = new StringWriter();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            xmlSerializer.Serialize(textWriter, productsInRange, namespaces);

            var result = textWriter.ToString();

            return result;
        }

        // Query 6. Sold Products - 100/100 -
        public static string GetSoldProducts(ProductShopContext context)
        {
            InitializeAutomapper();

            var soldProducts = context
                .Users
                .Where(u => u.ProductsSold.Count >= 1)
                .ProjectTo<ExportSoldProductsDTO>(mapper.ConfigurationProvider) // ok
                //manual mapping  - 100/100 -
                //.Select(u => new ExportSoldProductsDTO
                //{
                //    FirstName = u.FirstName,
                //    LastName = u.LastName,
                //    ProductsSold = u.ProductsSold.Select(ps => new ProductsSoldDTO
                //    {
                //        Name = ps.Name,
                //        Price = ps.Price
                //    })
                //    .ToArray()
                //})
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Take(5)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportSoldProductsDTO[]), new XmlRootAttribute("Users"));

            var textWriter = new StringWriter();

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            xmlSerializer.Serialize(textWriter, soldProducts, ns);

            var result = textWriter.ToString();

            return result;
        }

        // Query 7. Categories By Products Count - 100/100 -
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            //Get all categories. For each category select its name, the number of products, the average price of those products and the total revenue (total price sum) of those products (regardless if they have a buyer or not).
            //Order them by the number of products (descending) then by total revenue.

            InitializeAutomapper();

            var allCategories = context
                .Categories
                //.ProjectTo<ExportCategoriesByProductsCountDTO>(mapper.ConfigurationProvider)
                //manual mapping - 100/100
                .Select(c => new ExportCategoriesByProductsCountDTO
                {
                    Name = c.Name,
                    Count = c.CategoryProducts.Count,
                    AveragePrice = c.CategoryProducts.Average(cp => cp.Product.Price),
                    TotalRevenue = c.CategoryProducts.Sum(cp => cp.Product.Price)
                })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.TotalRevenue)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportCategoriesByProductsCountDTO[]), new XmlRootAttribute("Categories"));

            var textWriter = new StringWriter();

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            xmlSerializer.Serialize(textWriter, allCategories, ns);

            var result = textWriter.ToString();

            return result;
        }

        // Query 8. Users and Products - 100/100 -
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            InitializeAutomapper();

            var users = context
                .Users
                .ToArray() // Judge need this (to materialize the records and then to make operations over them)
                .Where(u => u.ProductsSold.Count >= 1) // take those who has atlest one sold product
                .Select(u => new ExportUsersDTO
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    InnerSoldProducs = new InnerSoldProducsDTO
                    {
                        Count = u.ProductsSold.Count,
                        Products = u.ProductsSold.Select(pc => new InnerDTO
                        {
                            Name = pc.Name,
                            Price = pc.Price
                        })
                        .OrderByDescending(pc => pc.Price)
                        .ToArray()
                    }
                })
                .OrderByDescending(x => x.InnerSoldProducs.Count)
                .Take(10)
                .ToArray();


            var countAllUsers = context.Users.Count(x => x.ProductsSold.Any()); // take all existing users from DB

            var resultDTO = new RootUsersWithProductsDTO
            {
                Count = countAllUsers,
                Users = users
            };

            var xmlSerializer = new XmlSerializer(typeof(RootUsersWithProductsDTO), new XmlRootAttribute("Users"));

            var textWriter = new StringWriter();

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            xmlSerializer.Serialize(textWriter, resultDTO, ns);

            var result = textWriter.ToString();

            return result;
        }


        // --- Additional methods --- //
        private static void ResetDatabase(ProductShopContext db)
        {
            db.Database.EnsureDeleted(); // first - deleted
            Console.WriteLine("Database was deleted!");

            db.Database.EnsureCreated(); // second - created
            Console.WriteLine("Database was created!");
        }

        private static void InitializeAutomapper()
        {
            // register/configure the mapper. dont forget to add field private static IMapper mapper;
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
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