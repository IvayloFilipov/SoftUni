namespace BookShop.Initializer
{
    using System;

    using Data;
    using Models;
    using Generators;

    public class DbInitializer
    {
        public static void ResetDatabase(BookShopContext context)
        {
            context.Database.EnsureDeleted(); //del db
            context.Database.EnsureCreated(); //create db

            Console.WriteLine("BookShop database created successfully.");

            Seed(context); //invoke method
        }

        public static void Seed(BookShopContext context)
        {
            Book[] books = BookGenerator.CreateBooks(); //Create books

            context.Books.AddRange(books); //add books

            context.SaveChanges(); //save changes

            Console.WriteLine("Sample data inserted successfully.");
        }
    }
}
