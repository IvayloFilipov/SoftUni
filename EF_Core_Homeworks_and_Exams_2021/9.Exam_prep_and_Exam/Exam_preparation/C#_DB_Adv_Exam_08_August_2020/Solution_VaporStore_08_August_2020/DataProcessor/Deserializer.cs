namespace VaporStore.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.DataProcessor.Dto.Import;
    using VaporStore.ImportResults;

    public static class Deserializer
    {
        public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            var sb = new StringBuilder();

            // 1. deserialise the input json data
            var deserialiseData = JsonConvert.DeserializeObject<IEnumerable<ImportGamesDTO>>(jsonString);
            // 2. Make validations
            foreach (var currGame in deserialiseData)
            {
                if (!IsValid(currGame) || currGame.Tags.Count() == 0) //or !currGame.Tags.Any()
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                //NOTE: Genre is a class/table into DB -> comlicated object so check into DB whether exists a Genre with name like a genre name into the input jsonString;
                //genre is a object of type Genre
                var genre = context.Genres.FirstOrDefault(x => x.Name == currGame.Genre);
                if (genre == null)
                {
                    genre = new Genre { Name = currGame.Genre };
                }
                //NOTE: Developer is a comlicatedt object. DO the same as above.
                //developer is a object of type Developer
                var developer = context.Developers.FirstOrDefault(x => x.Name == currGame.Developer);
                if (developer == null)
                {
                    developer = new Developer { Name = currGame.Developer };
                }

                //var releaseDate = DateTime.ParseExact(currGame.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                var game = new Game
                {
                    Name = currGame.Name,
                    Price = currGame.Price,
                    //ReleaseDate = (DateTime)currGame.ReleaseDate, //or -> releaseDate,
                    ReleaseDate = currGame.ReleaseDate.Value,
                    Genre = genre,
                    Developer = developer,
                };

                foreach (var currTag in currGame.Tags)
                {
                    //това е обект от тип Tag, проверявам в базата има ли го, ако го няма го създавам
                    var tag = context.Tags.FirstOrDefault(x => x.Name == currTag);
                    if (tag == null)
                    {
                        tag = new Tag { Name = currTag };
                    }

                    game.GameTags.Add(new GameTag { Tag = tag });
                    //В Game се намира колекция ICollection<GameTag> GameTags. Минавам през GameTag класа(междинна таблица 'много към много') за да стигна до Tag и да добавя нов tag.
                    //На game, в колекцията с GameTags и правим/сетваме нов GameTag и чрез него сетвам в Tag нов tag, който съм го взел от някъде или съм го създал.
                }

                context.Games.Add(game);
                context.SaveChanges();
                //ако правя проверка в базата за съществуващ обект и той не съществува, създавам го и го добавям => тогава context.SaveChanges() веднага в цикъла.
                //ак не проверявам => context.SaveChanges() след/извън цикъла.

                sb.AppendLine($"Added {currGame.Name} ({currGame.Genre}) with {currGame.Tags.Count()} tags");
            }

            var result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
        {
            var sb = new StringBuilder();

            //1. convert json input data
            var allUsers = JsonConvert.DeserializeObject<IEnumerable<ImportUsersDTO>>(jsonString);
            //2. make validations
            foreach (var currUser in allUsers)
            {
                if (!IsValid(currUser) || !currUser.Cards.All(IsValid))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                //must do this below-> cards are innerDTO(collection) ->or make this check up into the first if statement
                //foreach (var currCard in currUser.Cards)
                //{
                //    if (!IsValid(currCard))
                //    {
                //        sb.AppendLine("Invalid Data");
                //        continue;
                //    }
                //}

                var user = new User
                {
                    FullName = currUser.FullName,
                    Username = currUser.Username,
                    Email = currUser.Email,
                    Age = currUser.Age,
                    Cards = currUser.Cards.Select(c => new Card
                    {
                        Number = c.Number,
                        Cvc = c.CVC,
                        Type = c.Type.Value
                    })
                    .ToList()
                };

                context.Users.Add(user);
                //context.SaveChanges();

                sb.AppendLine($"Imported {currUser.Username} with {currUser.Cards.Count()} cards");
            }

            //dont forget
            context.SaveChanges();

            var result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
        {
            var sb = new StringBuilder();

            //1.
            var xmlSerializer = new XmlSerializer(typeof(ImportPurchasesDTO[]), new XmlRootAttribute("Purchases"));

            //2.read input
            using var textReader = new StringReader(xmlString);

            //3.deserialize the data
            var allPurchases = xmlSerializer.Deserialize(textReader) as ImportPurchasesDTO[];

            //4. foreach and create object to insert
            foreach (var currPurchase in allPurchases)
            {
                if (!IsValid(currPurchase))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                //validate Date
                var isDateFormatValid = DateTime.TryParseExact(currPurchase.Date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date);
                if (!isDateFormatValid)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                //create object
                var purchase = new Purchase
                {
                    Type = currPurchase.Type.Value,
                    Date = date,
                    ProductKey = currPurchase.Key,
                    //Card = currPurchase.Card, -> cannot convert string into the ....DTO ...go into the DB ant take it
                    Card = context.Cards.FirstOrDefault(x => x.Number == currPurchase.Card),
                    Game = context.Games.FirstOrDefault(x => x.Name == currPurchase.GameName)
                };

                //add property into purchase object or like above directly into the object
                //     purchase.Card = context.Cards.FirstOrDefault(x => x.Number == currPurchase.Card);
                //add another property into purchase object or like above directly into the object
                //     purchase.Game = context.Games.FirstOrDefault(x => x.Name == currPurchase.GameName);

                context.Purchases.Add(purchase);

                //take the username who buy the cuurGame (made the currPurchase)
                var username = context.Users.Where(x => x.Id == purchase.Card.UserId).Select(x => x.Username).FirstOrDefault();

                //or
                var secondVariantUsername = context.Cards.FirstOrDefault(x => x.Number == currPurchase.Card);

                sb.AppendLine($"Imported {currPurchase.GameName} for {username}"); //secondVariantUsername
            }

            context.SaveChanges();

            var result = sb.ToString().TrimEnd();
            return result;
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}