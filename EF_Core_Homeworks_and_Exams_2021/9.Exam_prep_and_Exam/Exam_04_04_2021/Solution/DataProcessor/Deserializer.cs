namespace TeisterMask.DataProcessor
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
    using TeisterMask.Data.Models;
    using TeisterMask.Data.Models.Enums;
    using TeisterMask.DataProcessor.ImportDto;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var validProjects = new List<Project>();

            var xmlSerializer = new XmlSerializer(typeof(XmlImportProjectDTO[]), new XmlRootAttribute("Projects"));
            using var textReader = new StringReader(xmlString);
            var allProjects = xmlSerializer.Deserialize(textReader) as XmlImportProjectDTO[];

            //•	If there are any validation errors for the project entity (such as invalid name or open date), do not import any part of the entity and append an error message to the method output.
            foreach (var currDto in allProjects)
            {
                if (!IsValid(currDto) || currDto.OpenDate == null) //was currDto.DueDate
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                
                //validate Pr Open date
                var isOpenDateFormatValid = DateTime.TryParseExact(currDto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var openDate);
                if (!isOpenDateFormatValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                //validate Pr Due date
                var isDueDateFormatValid = DateTime.TryParseExact(currDto.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dueDate);
                if (!isDueDateFormatValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                //•	If there are any validation errors for the task entity(such as invalid name, open or due date are missing, task open date is before project open date or task due date is after project due date), do not import it(only the task itself, not the whole project) and append an error message to the method output.
                
                foreach (var currTask in currDto.Tasks)
                {
                    var isOpenDateTaskFormatValid = DateTime.TryParseExact(currTask.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var openDateTask);
                    if (!isOpenDateTaskFormatValid)
                    {
                        sb.AppendLine("Invalid Data");
                        continue;
                    }

                    var isDueDateTaskFormatValid = DateTime.TryParseExact(currTask.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dueDateTask);
                    if (!isDueDateTaskFormatValid)
                    {
                        sb.AppendLine("Invalid Data");
                        continue;
                    }

                    //task open date is before project open date or task due date is after project due date)
                    if (openDateTask < openDate || dueDateTask > dueDate)
                    {
                        sb.AppendLine("Invalid Data");
                        continue;
                    }
                }

                var project = new Project
                {
                    Name = currDto.Name,
                    OpenDate = openDate,
                    DueDate = dueDate,
                    Tasks = currDto.Tasks.Select(t => new Task
                    {
                        Name = t.Name,
                        OpenDate = openDate,
                        DueDate = dueDate,
                        ExecutionType = (ExecutionType)t.ExecutionType,
                        LabelType = (LabelType)t.LabelType
                    })
                    .ToArray()
                };

                validProjects.Add(project);

                sb.AppendLine($"Successfully imported project - {currDto.Name} with {currDto.Tasks.Count()} tasks.");
            }


            context.Projects.AddRange(validProjects);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var validEmpl = new List<Employee>();

            var emplDto = JsonConvert.DeserializeObject<IEnumerable<JsonImportEmployeeDTO>>(jsonString);

            foreach (var currEmpl in emplDto)
            {
                if (!IsValid(currEmpl))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var employee = new Employee
                {
                    Username = currEmpl.Username,
                    Phone = currEmpl.Phone,
                    Email = currEmpl.Email
                };

                foreach (var currTask in currEmpl.Tasks.Distinct())
                {
                    var task = context.Tasks.FirstOrDefault(x => x.Id == currTask);

                    if(task == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    employee.EmployeesTasks.Add(new EmployeeTask
                    {
                        Employee = employee,
                        Task = task
                    });
                }

                validEmpl.Add(employee);
                sb.AppendLine($"Successfully imported employee - {currEmpl.Username} with {currEmpl.Tasks.Count()} tasks.");
            }

            context.Employees.AddRange(validEmpl);
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