namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.DataProcessor.ExportDto;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            var allPrisoners = context
                .Prisoners.ToList()
                .Where(x => ids.Contains(x.Id))
                .Select(p => new
                {
                    Id = p.Id,
                    Name = p.FullName,
                    CellNumber = p.Cell.CellNumber,
                    Officers = p.PrisonerOfficers
                                .Select(po => new
                                {
                                    OfficerName = po.Officer.FullName,
                                    Department = po.Officer.Department.Name
                                })
                                .OrderBy(x => x.OfficerName)
                                .ToArray(),
                    TotalOfficerSalary = p.PrisonerOfficers
                                          .Select(s => s.Officer.Salary)
                                          .Sum() //or p.PrisonerOfficers.Sum(s => s.Officer.Salary)

                })
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToList();

            var json = JsonConvert.SerializeObject(allPrisoners, Formatting.Indented);

            return json;
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            //Use the method provided in the project skeleton, which receives a string of comma-separated prisoner names. Export the prisoners: for each prisoner, export its id, name, incarcerationDate in the format “yyyy-MM-dd” and their encrypted mails. 
            //The encrypted algorithm you have to use is just to take each prisoner mail description and reverse it. 
            //Sort the prisoners by their name(ascending), then by their id(ascending).

            var inputNames = prisonersNames.Split(",", StringSplitOptions.RemoveEmptyEntries);

            var allPrisoners = context
                .Prisoners
                .Where(x => inputNames.Contains(x.FullName))
                .Select(p => new ExpotPrisonersXMLDTO
                {
                    Id = p.Id,
                    Name = p.FullName,
                    IncarcerationDate = p.IncarcerationDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    MessagesArr = p.Mails.Select(m => new InnerPrisonerMessagesDTO
                    {
                        DescriptionMsg = string.Join("", m.Description.Reverse())
                        //DescriptionMsg = Reverse(m.Description)
                    })
                    .ToArray()
                })
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExpotPrisonersXMLDTO[]), new XmlRootAttribute("Prisoners"));

            var textWriter = new StringWriter();

            var ns = new XmlSerializerNamespaces();
            ns.Add("","");

            xmlSerializer.Serialize(textWriter, allPrisoners, ns);

            var result = textWriter.ToString();

            return result;
        }

        //Method to reverse string
        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}