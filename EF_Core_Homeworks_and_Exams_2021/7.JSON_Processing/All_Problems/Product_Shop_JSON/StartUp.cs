using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DataTransferObjects;
using ProductShop.DataTransferObjects.Users;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        private static IMapper mapper;
        private static string ResultsDirectoryPath = "../../../Datasets/Results"; //create path as constant

        public static void Main(string[] args)
        {
            var db = new ProductShopContext();

            //ResetDatabase(db);

            // first take/read data from users.json (here can put relative or absolute path).The relative work folder path of a .NET Core App (path) is this case is (Name of the App(ProductShop) -> ProductShop/bin/Debug/netcoreapp2.1/ProductShop.dll) It work into the .dll file. So the relative path is one step back (../) to go to Debug, one more to go to bin, one more to go to ProductShop, and then enter into folderName(Datasets) and read .json file.

            //do not use the absolute path because it is hard cold and will NOT worc on other computer
            //absolute path -> C: \Users\User\source\repos\Product_Shop_JSON\Datasets / users.json

            // --- import into DB and deserialize ----

            //var inputJson = File.ReadAllText("../../../Datasets/users.json");
            //var result = ImportUsers(db, inputJson);

            //var productsJson = File.ReadAllText("../../../Datasets/products.json");
            //var result = ImportProducts(db, productsJson);

            //var categoriesJson = File.ReadAllText("../../../Datasets/categories.json");
            //var result = ImportCategories(db, categoriesJson);

            //var categProdJson = File.ReadAllText("../../../Datasets/categories-products.json");
            //var result = ImportCategoryProducts(db, categProdJson);

            // --- export from DB and serialize ----

            //var resultAsJson = GetProductsInRange(db);
            //var resultAsJson = GetSoldProducts(db);
            //var resultAsJson = GetCategoriesByProductsCount(db);
            //var resultAsJson = GetUsersWithProducts(db);


            EnsureDirectoryExists(ResultsDirectoryPath);

            File.WriteAllText(ResultsDirectoryPath + "/users-and-products.json", resultAsJson); //here just channge the final path name

            Console.WriteLine(resultAsJson);
        }




        // create ResetDatabase or use Window PowerShel to create migration an update(create) DB
        private static void ResetDatabase(ProductShopContext db)
        {
            // this ensure that I have epty database
            db.Database.EnsureDeleted();
            Console.WriteLine("Database was deleted!");

            db.Database.EnsureCreated();
            Console.WriteLine("Database was created!");
        }

        // ------ Query and Export Data ---------

        // Judge - Problem 1. - Query 2. Import Users - ok - 100/100 -
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            InitializeAutoMapper();

            //Datasets are the JSON file info/models that will go to DB. Import the users from the provided file users.json.
            // The users.json is array from object. So cant map users.json to User.cs. To solve this must use <List<User>> or to <User[]>

            //List<User> allUsers = JsonConvert.DeserializeObject<List<User>>(inputJson); //contain collection of users

            //User[] allUsers = JsonConvert.DeserializeObject<User[]>(inputJson); // add DTO into the <> as IEnumerable

            //First Deserialize from inputJson (The json files that have to be deserialized are in eatch file into Dataset folder) to UserInputModelDTO. It is represented as parameter (...,string inputJson). Read the json as shown above in Main => File.ReadAllText();
            IEnumerable<UserInputModelDTO> usersDTO = JsonConvert.DeserializeObject<IEnumerable<UserInputModelDTO>>(inputJson);

            var allUsers = mapper.Map<IEnumerable<User>>(usersDTO); // NO this. in front of the mapper due to it is static
            //In order to add allUsers (object who will go to the DB) into the DB I need to map the data from usersDTO (as type of  IEnumerable<UserInputModelDTO>) to IEnumerable<User>. The data will come from usersDTO.
            //IEnumerable<User> is a valid object to be enter into the DB
            //Go and create map into the ProductShopProfile too. Use AutoMapper.
            //AND HERE I HAVE TO map usersDTO to IEnumerable<User>.

            context.Users.AddRange(allUsers); // AddRange -> due to this array (users.json) contain multiple objects. Users is the name of the table into the DB(DbSest). It will not see this in that way due to has different layers in a real project. Secont, it is not OK to comunicate directly with our entityes from the DB.
            //It is good to have some additional model. Add folder DataTransferObject, inside add class/DTO UserInputModel


            context.SaveChanges();

            return $"Successfully imported {allUsers.Count()}"; // .Length -> it is array, if List -> .Count or .Count() if it is IEnumerable
        }

        // Judge - Problem 2. Query 3. Import Products - ok - 100/100 -
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            InitializeAutoMapper();

            //in general -> here I deserialise the inputJson to IEnumerable<ProductInputModelDTO>
            var productsDTO = JsonConvert.DeserializeObject<IEnumerable<ProductInputModelDTO>>(inputJson);

            var allProducts = mapper.Map<IEnumerable<Product>>(productsDTO); // NO this.mapper... due to it is static

            context.Products.AddRange(allProducts);
            context.SaveChanges();

            return $"Successfully imported {allProducts.Count()}";
        }

        // Judge - Problem 3. Query 4. Import Categories - ok 100/100 - 
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            InitializeAutoMapper();

            var categoriesDTO = JsonConvert
                .DeserializeObject<IEnumerable<CategoriesInputModelDTO>>(inputJson) // IEnumerable<> -> collection
                .Where(x => x.Name != null) // if Name is null, skip the record from json
                .ToList();


            // or can use JsonSerializerSettings()
            //JsonSerializerSettings settings = new JsonSerializerSettings()
            //{
            //    NullValueHandling = NullValueHandling.Ignore, //like this, it is ignore just the property value not the object 
            //};
            //var categoriesDTO = JsonConvert
            //    .DeserializeObject<IEnumerable<CategoriesInputModelDTO>>(inputJson, settings);

            var allCategories = mapper.Map<IEnumerable<Category>>(categoriesDTO); //It is a MUST to add IEnumerable<> -> collection

            context.Categories.AddRange(allCategories);
            context.SaveChanges();

            return $"Successfully imported {allCategories.Count()}";
        }

        // Judge - Problem 4 - Query 5. Import Categories and Products - ok 100/100 -
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            InitializeAutoMapper();

            var catProdDTO = JsonConvert.DeserializeObject<IEnumerable<CategoriesProductsInputModelDTO>>(inputJson);

            // the data go to <IEnumerable<CategoryProduct>> and come from catProdDTO
            var allCatProd = mapper.Map<IEnumerable<CategoryProduct>>(catProdDTO);

            context.CategoryProducts.AddRange(allCatProd);
            context.SaveChanges();

            return $"Successfully imported {allCatProd.Count()}";
        }


        // ------ Query and Export Data ---------

        // Judge - Problem 5 Query 6. Export Products in Range - ok 100/100 - 
        public static string GetProductsInRange(ProductShopContext context)
        {
            var allProducts = context
                .Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .OrderBy(x => x.Price)
                .Select(x => new
                {
                    name = x.Name,
                    price = x.Price, //can add .ToString("f2") -> in order to have correct output
                    seller = x.Seller.FirstName + " " + x.Seller.LastName
                })
                .ToList();

            var jsonProducts = JsonConvert.SerializeObject(allProducts, Formatting.Indented); //None when make Minifiing

            return jsonProducts;
        }


        // Judge - Problem 6 Query 7. Export Successfully Sold Products - ok 100/100
        public static string GetSoldProducts(ProductShopContext context)
        {
            //InitializeAutoMapper();

            var soldProducts = context
                .Users
                //.Where(x => x.ProductsSold.Count > 0) //  50/100
                .Where(x => x.ProductsSold.Any(p => p.Buyer != null)) // 100/100
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.LastName)
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    soldProducts = u.ProductsSold
                        .Where(p => p.Buyer != null)//add this check due to task description -> sold products (products with buyers)
                        .Select(ps => new
                        {
                            name = ps.Name,
                            price = ps.Price,
                            buyerFirstName = ps.Buyer.FirstName,
                            buyerLastName = ps.Buyer.LastName,
                        })
                        .ToArray()
                })
                //.ProjectTo<UserWithSoldProductsDTO>()// solution with DTO
                .OrderBy(x => x.lastName)
                .ThenBy(x => x.firstName)
                .ToArray();

            var jsonUsers = JsonConvert.SerializeObject(soldProducts, Formatting.Indented);

            return jsonUsers;
        }

        // Judge - Problem 7 Query 8. Export Categories by Products Count - ok 100/100 -
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            //var allCategories = context
            //    .Categories
            //    .Select(x => new
            //    {
            //        category = x.Name,
            //        productsCount = x.CategoryProducts.Select(p => p.Product).Count(),
            //        averagePrice = x.CategoryProducts.Select(p => p.Product.Price).Average().ToString("f2"),
            //        totalRevenue = x.CategoryProducts.Select(p => p.Product.Price).Sum().ToString("f2")
            //    })
            //    .OrderByDescending(x => x.productsCount)
            //    .ToArray();

            //or
            var allCategories = context
                .Categories
                .Select(x => new
                {
                    category = x.Name,
                    productsCount = x.CategoryProducts.Count(),
                    averagePrice = x.CategoryProducts
                        .Average(y => y.Product.Price)//change avg with math calculation due to SQL Profiler queries number
                        .ToString("f2"),
                    totalRevenue = x.CategoryProducts
                        .Sum(y => y.Product.Price)
                        .ToString("f2")
                })
                .OrderByDescending(x => x.productsCount)
                .ToArray();

            var jsonCategories = JsonConvert.SerializeObject(allCategories, Formatting.Indented);

            return jsonCategories;
        }


        // Judge - Problem 8 Query 9. Export Users and Products
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var usersWithProducts = context
                .Users.Include(x => x.ProductsSold).ToArray() // use .Include(x => x.ProductsSold).ToArray() -> due to Judge
                //.Where(x => x.ProductsSold.Any(p => p.Buyer != null))
                .Where(x => x.ProductsSold.Count > 0 ) //&& x.ProductsSold.Any(b => b.Buyer != null))
                //.OrderByDescending(x => x.ProductsSold.Count(y => y.Buyer != null))
                //.OrderByDescending(x => x.ProductsSold.Where(y => y.Buyer != null).Count())
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    age = u.Age,
                    soldProducts = new
                    {
                        count = u.ProductsSold.Where(x => x.Buyer != null).Count(),
                        products = u.ProductsSold.Where(x => x.Buyer != null)
                        .Select(p => new
                        {
                            name = p.Name,
                            price = p.Price
                        })
                    }
                })
                .OrderByDescending(x => x.soldProducts.count)
                .ToArray();

            // the shell(крайната обвивка) (finalResult) is anonimous object -> this is the complicated part of the problem
            var finalResult = new
            {
                usersCount = usersWithProducts.Length,
                users = usersWithProducts
            };

            // create json settings -> ignore null values(this skip the object, not the value) and formatting indented
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
            };

            string jsonUsersWithProducts = JsonConvert.SerializeObject(finalResult, settings);

            return jsonUsersWithProducts;
        }



        // ----------------The method is placed here and it is called into the ImportUsers due to Judge --------------
        private static void InitializeAutoMapper()
        {
            // register/configure the mapper. dont forget to add field private static IMapper mapper;
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });

            mapper = config.CreateMapper();
        }

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