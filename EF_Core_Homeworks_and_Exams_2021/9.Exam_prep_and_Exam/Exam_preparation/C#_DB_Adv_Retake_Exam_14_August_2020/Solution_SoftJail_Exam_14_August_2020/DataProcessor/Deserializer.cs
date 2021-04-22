namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            //•	If any validation errors occur (such as if a department name is too long/short or a cell number is out of range) proceed as described above
            //•	If a department is invalid, do not import its cells. -> !IsValid(currDto)
            //•	If a Department doesn’t have any Cells, he is invalid. -> !currDto.Cells.Any()
            //•	If one Cell has invalid CellNumber, don’t import the Department. -> !currDto.Cells.All(x => IsValid(x))

            var sb = new StringBuilder();
            var validDepartments = new List<Department>();

            //01.Create Json outher DTO (with inner DTO Cell)

            //1.deserialize data -> from jsoString input to DTO
            var jsonDeserializeData = JsonConvert.DeserializeObject<IEnumerable<JsonImportDepAndCellDTO>>(jsonString);
            //2.make validations
            foreach (var currDto in jsonDeserializeData)
            {
                if (!IsValid(currDto) || !currDto.Cells.Any() || !currDto.Cells.All(x => IsValid(x)))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var department = new Department
                {
                    Name = currDto.Name,
                    Cells = currDto.Cells.Select(x => new Cell
                    {
                        CellNumber = x.CellNumber,
                        HasWindow = x.HasWindow
                    })
                    .ToArray()
                };

                validDepartments.Add(department);

                sb.AppendLine($"Imported {currDto.Name} with {currDto.Cells.Length} cells");
            }

            context.Departments.AddRange(validDepartments);
            context.SaveChanges();

            var result = sb.ToString().TrimEnd();

            return result;
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            //•	The release and incarceration dates will be in the format “dd/MM/yyyy”. Make sure you use CultureInfo.InvariantCulture.
            //•	If any validation errors occur(such as invalid prisoner name or invalid nickname), ignore the entity and print  an     error message.
            //•	If a mail has incorrect address print error message and do not import the prisoner and his mails

            var sb = new StringBuilder();
            var validPrisoners = new List<Prisoner>();

            //1.deserialise input
            var prizonersMailsDtos = JsonConvert.DeserializeObject<IEnumerable<JsonImportPrisonersMailsDTO>>(jsonString);

            //2.make validations
            foreach (var currPris in prizonersMailsDtos)
            {
                if (!IsValid(currPris) || !currPris.Mails.Any(x => IsValid(x.Address)))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var incDate = DateTime.ParseExact(currPris.IncarcerationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                

                var isRelDateValid = DateTime.TryParseExact(currPris.IncarcerationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var relDate);

                var prisoner = new Prisoner
                {
                    FullName = currPris.FullName,
                    Nickname = currPris.Nickname,
                    Age = currPris.Age,
                    IncarcerationDate = incDate,
                    ReleaseDate = isRelDateValid ? (DateTime?)relDate : null,
                    Bail = currPris.Bail,
                    CellId = currPris.CellId,
                    Mails = currPris.Mails.Select(x => new Mail
                    {
                        Description = x.Description,
                        Sender = x.Sender,
                        Address = x.Address,
                    })
                    .ToArray()
                };

                validPrisoners.Add(prisoner);
                sb.AppendLine($"Imported {currPris.FullName} {currPris.Age} years old");
            }

            context.Prisoners.AddRange(validPrisoners);
            context.SaveChanges();

            var result = sb.ToString().TrimEnd();
            return result;
        }


        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            //•	If there are any validation errors (such as negative salary or invalid position/weapon), proceed as described above.
            //•	The prisoner Id will always be valid
            var sb = new StringBuilder();
            var validOfficers = new List<Officer>();

            //1.
            var xmlSerializer = new XmlSerializer(typeof(XmlImpotrOfficerPrisonerDTO[]), new XmlRootAttribute("Officers"));
            //2.
            using var textReadel = new StringReader(xmlString);
            //3.
            var ns = new XmlSerializerNamespaces();
            ns.Add("","");
            //4.
            var allData = (XmlImpotrOfficerPrisonerDTO[])xmlSerializer.Deserialize(textReadel);

            //5. validation
            foreach (var currDto in allData)
            {
                if (!IsValid(currDto))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var officer = new Officer
                {
                    FullName = currDto.Name,
                    Salary = currDto.Money,
                    Position = Enum.Parse<Position>(currDto.Position),
                    Weapon = Enum.Parse<Weapon>(currDto.Weapon),
                    DepartmentId = currDto.DepartmentId,
                    OfficerPrisoners = currDto.Prisoners.Select(x => new OfficerPrisoner
                    {
                        PrisonerId = x.PrisonerId
                    })
                    .ToArray()
                };

                validOfficers.Add(officer);
                sb.AppendLine($"Imported {currDto.Name} ({currDto.Prisoners.Count()} prisoners)");
            }

            context.Officers.AddRange(validOfficers);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}