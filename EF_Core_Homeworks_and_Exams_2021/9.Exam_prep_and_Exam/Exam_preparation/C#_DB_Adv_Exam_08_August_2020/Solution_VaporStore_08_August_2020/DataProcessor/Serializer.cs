namespace VaporStore.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.DataProcessor.Dto.Export;

    public static class Serializer
    {
        public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
        {
            var givenGenresAndGames = context
                .Genres.ToList() // ToList() is here due to Judge
                .Where(x => genreNames.Contains(x.Name))
                .Select(g => new
                {
                    Id = g.Id,
                    Genre = g.Name,
                    Games = g.Games
                             .Where(x => x.Purchases.Any())
                             .Select(p => new
                             {
                                 Id = p.Id,
                                 Title = p.Name,
                                 Developer = p.Developer.Name,
                                 Tags = string.Join(", ", p.GameTags.Select(gt => gt.Tag.Name)),
                                 Players = p.Purchases.Count()
                             })
                             .OrderByDescending(x => x.Players)
                             .ThenBy(x => x.Id),
                    TotalPlayers = g.Games.Sum(x => x.Purchases.Count())
                    
                })
                .OrderByDescending(x => x.TotalPlayers)
                .ThenBy(x => x.Id)
                .ToList();

            var settings = new JsonSerializerSettings
            {
                //NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };

            var json = JsonConvert.SerializeObject(givenGenresAndGames, settings);

            return json;
        }

        public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
        {
            var userByPurchases = context
                .Users.ToList() //ToList() is here due to Jugje
                .Where(u => u.Cards.Any(c => c.Purchases.Any(p => p.Type.ToString() == storeType)))
                .Select(u => new UserMainDTO
                {
                    Username = u.Username,
                    TotatlSpent = u.Cards
                        .Sum(c => c.Purchases.Where(p => p.Type.ToString() == storeType)
                        .Sum(c => c.Game.Price)),
                    Purchases = u.Cards
                        .SelectMany(c => c.Purchases)
                        .Where(p => p.Type.ToString() == storeType)
                        .Select(p => new PurchaseDTO
                        {
                            Card = p.Card.Number,
                            Cvc = p.Card.Cvc,
                            Date = p.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                            Game = new GameDTO
                            {
                                Title = p.Game.Name,
                                Price = p.Game.Price,
                                Genre = p.Game.Genre.Name,
                            },
                        })
                        .OrderBy(x => x.Date)
                        .ToArray()
                })
                .OrderByDescending(x => x.TotatlSpent)
                .ThenBy(x => x.Username)
                .ToArray();

            var xmlSerialiser = new XmlSerializer(typeof(UserMainDTO[]), new XmlRootAttribute("Users"));

            var textWriter = new StringWriter();

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            xmlSerialiser.Serialize(textWriter, userByPurchases, ns);

            var result = textWriter.ToString();

            return result;
        }
    }
}