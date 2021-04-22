namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using BookShopContext db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            //string inputCmd = Console.ReadLine();
            //string result = GetBooksByAgeRestriction(db, inputCmd);

            //string result = GetGoldenBooks(db);

            //string result = GetBooksByPrice(db);

            //int inputYear = int.Parse(Console.ReadLine());
            //string result = GetBooksNotReleasedIn(db, inputYear);

            //string input = Console.ReadLine();
            //string result = GetBooksByCategory(db, input);

            //string inputData = Console.ReadLine();
            //string result = GetBooksReleasedBefore(db, inputData);

            //string inputData = Console.ReadLine();
            //string result = GetAuthorNamesEndingIn(db, inputData);

            //string inputData = Console.ReadLine();
            //string result = GetBookTitlesContaining(db, inputData);

            //string inputData = Console.ReadLine();
            //string result = GetBooksByAuthor(db, inputData);

            //int inputLength = int.Parse(Console.ReadLine());
            //var result = CountBooks(db, inputLength);

            //string result = CountCopiesByAuthor(db);

            //string result = GetTotalProfitByCategory(db);

            //string result = GetMostRecentBooks(db);

            //IncreasePrices(db);    // void method to increase prices

            int result = RemoveBooks(db);

            Console.WriteLine(result);
        }

        // 2. Age Restriction - ok 100/100 -
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            StringBuilder sb = new StringBuilder();

            var bookTitles = context
                .Books.AsEnumerable()  //MUST to add .AsEnumerable()
                .Where(x => x.AgeRestriction.ToString().ToLower() == command.ToLower())
                .Select(x => x.Title)
                .OrderBy(x => x)
                .ToList();

            foreach (var currTitle in bookTitles)
            {
                sb
                    .AppendLine($"{currTitle}");
            }

            return sb.ToString().TrimEnd();

            //or
            //return String.Join(Environment.NewLine, bookTitles);

        }

        // 3. Golden Books - ok 100/100 -
        public static string GetGoldenBooks(BookShopContext context)
        {
            var bookInfo = context
                .Books.AsEnumerable()
                //.Where(b => b.Copies < 5000 && b.EditionType.ToString() == "Gold")
                .Where(b => b.Copies < 5000 && b.EditionType == EditionType.Gold) //add using BookShop.Models.Enums
                .OrderBy(x => x.BookId)
                .Select(b => new
                {
                    BookTitle = b.Title
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var currBook in bookInfo)
            {
                sb
                    .AppendLine($"{currBook.BookTitle}");
            }

            return sb.ToString().TrimEnd();

            //or -> return String.Join(Environment.NewLine, bookInfo); //without StringBuilder
        }

        // 4. Books by Price - ok 100/100 -
        public static string GetBooksByPrice(BookShopContext context)
        {
            var bookInfo = context
                .Books.AsEnumerable()
                .Where(b => b.Price > 40)
                .Select(b => new
                {
                    BookTitle = b.Title,
                    BookPrice = b.Price
                })
                .OrderByDescending(x => x.BookPrice)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var currBook in bookInfo)
            {
                sb
                    .AppendLine($"{currBook.BookTitle} - ${currBook.BookPrice:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        // 5. Not Released In - ok 100/100 -
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var bookInfo = context
                .Books        //NO need to add .AsEnumerable()
                .Where(b => b.ReleaseDate.Value.Year != year) //ReleaseDate in Book class is nullable(?) => ReleaseDate.Value.Year -> if not nullable can have => ReleaseDate.Year
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToList();

            return String.Join(Environment.NewLine, bookInfo);
        }

        // 6. Book Titles by Category - ok 100/100 -
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] inputCateg = input
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.ToLower()) // here make input categories as case insensitive
                .ToArray();

            //---------------------------------------------------------------
            var bookTitles = new List<string>();

            foreach (var currCat in inputCateg)
            {
                List<string> bookTitlesFromContext = context
                    .Books
                    .Where(b => b.BookCategories.Any(bc => bc.Category.Name.ToLower() == currCat))
                    .Select(b => b.Title)
                    .ToList();

                bookTitles.AddRange(bookTitlesFromContext); //add all fined titles into the bookTitles list
            }

            bookTitles = bookTitles.OrderBy(bt => bt).ToList(); // order all titles

            return String.Join(Environment.NewLine, bookTitles);

            //-------------------------------------------------------------------
            // same result as above code, but in Judge - 0/100 -

            //var bookTitles = context
            //    .Books
            //    .Where(b => b.BookCategories.Any(bc => inputCateg.Contains(bc.Category.Name)))
            //    .Select(b => b.Title)
            //    .OrderBy(bt => bt)
            //    .ToArray();

            //return String.Join(Environment.NewLine, bookTitles);

            //-------------------------------------------------------------------

            //-------------------------------------------------------------------
            // same result as above code

            //var alBookCategories = context
            //    .BooksCategories
            //    .Where(bc => inputCateg.Contains(bc.Category.Name.ToLower()))
            //    .Select(b => b.Book.Title)
            //    .OrderBy(bt => bt)
            //    .ToArray();

            //return String.Join(Environment.NewLine, alBookCategories);

            //-------------------------------------------------------------------

            //or print the result by using foreach and StringBuilder
            //StringBuilder sb = new StringBuilder();

            //foreach (var currTitle in bookTitles.OrderBy(bt => bt))
            //{
            //    sb
            //        .AppendLine(currTitle);
            //}

            //return sb.ToString().TrimEnd();
        }

        // 7. Released Before Date - ok 100/100 -
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var targetDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var bookInfo = context
                .Books
                .Where(b => b.ReleaseDate.Value < targetDate)
                .OrderByDescending(x => x.ReleaseDate)
                .Select(b => new
                {
                    BookTitle = b.Title,
                    EditionType = b.EditionType,
                    BookPrice = b.Price
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var currBook in bookInfo)
            {
                sb
                    .AppendLine($"{currBook.BookTitle} - {currBook.EditionType} - ${currBook.BookPrice:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        // 8. Author Search - ok 100/100 -
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            //var bookInfo = context
            //    .Authors
            //    .Where(a => a.FirstName.EndsWith(input))
            //    .OrderBy(a => a.FirstName)
            //    .Select(a => new
            //    {
            //        a.FirstName,
            //        a.LastName
            //    })
            //    .ToList();


            //StringBuilder sb = new StringBuilder();

            //foreach (var item in bookInfo)
            //{
            //    sb
            //        .AppendLine($"{item.FirstName} {item.LastName}");
            //}

            //return sb.ToString().TrimEnd();

            //this variant above return result in Judge due to the OrderBy() position -- 75/100 --

            var bookInfo = context
                .Authors
                .Where(a => a.FirstName.EndsWith(input))
                .Select(a => new
                {
                    FullName = a.FirstName + " " + a.LastName
                })
                .OrderBy(a => a.FullName)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var item in bookInfo)
            {
                sb
                    .AppendLine($"{item.FullName}");
            }

            return sb.ToString().TrimEnd();
        }

        // 9. Book Search - ok 100/100 -
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var bookInfo = context
                //.Books.AsEnumerable() // variant 1 - MUST add .AsEnumerable()
                //.Where(b => b.Title.Contains(input, StringComparison.InvariantCultureIgnoreCase))
                //.Select ...

                //or variant 2
                .Books
                .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToList();

            return String.Join(Environment.NewLine, bookInfo);
        }

        // 10. Book Search by Author - ok 100/100 -
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var bookInfo = context
                .Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(b => b.BookId)
                .Select(b => new
                {
                    BookName = b.Title,
                    BookAuthor = $"{b.Author.FirstName} {b.Author.LastName}"
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var currBook in bookInfo)
            {
                sb
                    .AppendLine($"{currBook.BookName} ({currBook.BookAuthor})");
            }

            return sb.ToString().TrimEnd();
        }

        // 11. Count Books - ok 100/100 -
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var bookInfo = context
                .Books
                .Where(b => b.Title.Length > lengthCheck)
                .ToList();

            int counter = bookInfo.Count;
           
            return counter;
        }

        // 12. Total Book Copies - ok 100/100 -
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var bookCopies = context
                .Authors
                .Select(a => new
                {
                    AuthorFullName = $"{a.FirstName} {a.LastName}",
                    TotalBooksByAuthor = a.Books.Sum(b => b.Copies)
                    //TotalBooksByAuthor = a.Books.Select(b => b.Copies).Sum() //same as above
                })
                .OrderByDescending(x => x.TotalBooksByAuthor)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var currAuthor in bookCopies)
            {
                sb
                    .AppendLine($"{currAuthor.AuthorFullName} - {currAuthor.TotalBooksByAuthor}");
            }

            return sb.ToString().TrimEnd();
        }

        // 13. Profit by Category - ok 100/100 -
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var bookInfo = context
                .Categories
                .Select(c => new
                {
                    CurrCategoryName = c.Name,
                    TotalProfit = c.CategoryBooks.Select(cb => new
                    {
                        BookProfit = cb.Book.Copies * cb.Book.Price
                    })
                    .Sum(b => b.BookProfit)
                })
                .OrderByDescending(x => x.TotalProfit)
                .ThenBy(x => x.CurrCategoryName)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var item in bookInfo)
            {
                sb
                    .AppendLine($"{item.CurrCategoryName} ${item.TotalProfit:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        // 14. Most Recent Books - ok 100/100 -
        public static string GetMostRecentBooks(BookShopContext context)
        {
            var recentBooks = context
                .Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    BookInfo = c.CategoryBooks.Select(cb => new
                    {
                        BookName = cb.Book.Title,
                        BookReleaseDate = cb.Book.ReleaseDate
                    })
                    .OrderByDescending(x => x.BookReleaseDate)
                    .Take(3)
                })
                .OrderBy(x => x.CategoryName)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var categ in recentBooks)
            {
                sb
                    .AppendLine($"--{categ.CategoryName}");

                foreach (var b in categ.BookInfo)
                {
                    sb
                        .AppendLine($"{b.BookName} ({b.BookReleaseDate.Value.Year})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        // 15. Increase Prices - ok 100/100 -
        public static void IncreasePrices(BookShopContext context)
        {
            var bookSet = context
                .Books
                .Where(b => b.ReleaseDate.Value.Year < 2010);

            foreach (var currBook in bookSet)
            {
                currBook.Price += 5;
            }

            context.SaveChanges();
        }

        // 16. Remove Books
        public static int RemoveBooks(BookShopContext context)
        {
            var deletedBooks = context
                .Books
                .Where(b => b.Copies < 4200);

            int counter = deletedBooks.Count();

            context.RemoveRange(deletedBooks);
            context.SaveChanges();

            return counter;
        }
    }
}
