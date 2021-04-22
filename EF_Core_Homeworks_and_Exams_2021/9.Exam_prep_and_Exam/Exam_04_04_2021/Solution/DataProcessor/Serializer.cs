namespace TeisterMask.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.Data.Models.Enums;
    using TeisterMask.DataProcessor.ExportDto;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            var data = context
                .Projects.ToArray()
                .Where(x => x.Tasks.Any())
                //.Where(x => x.Tasks.Count > 0)
                .Select(x => new XmlExportProjectsDTO
                {
                    ProjectName = x.Name,
                    TasksCount = x.Tasks.Count(),
                    HasEndDate = x.DueDate == null ? "Yes" : "No",
                    Tasks = x.Tasks.Select(t => new InnerTasksDTO
                    {
                        Name = t.Name,
                        Label = t.LabelType.ToString()
                    })
                    .OrderBy(x => x.Name)
                    .ToArray()
                })
                .OrderByDescending(x => x.TasksCount)
                .ThenBy(x => x.ProjectName)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(XmlExportProjectsDTO[]), new XmlRootAttribute("Projects"));

            var sw = new StringWriter();

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            xmlSerializer.Serialize(sw, data, ns);

            return sw.ToString();
        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            var empl = context
                .Employees.ToArray()
                .Where(x => x.EmployeesTasks.Any(s => s.Task.OpenDate >= date))
                .Select(x => new 
                {
                    Username = x.Username,
                    Tasks = x.EmployeesTasks
                    .ToArray()
                    .Where(et => et.Task.OpenDate >= date)
                    .OrderByDescending(x => x.Task.DueDate)
                    .ThenBy(x => x.Task.Name)
                    .Select(t => new 
                    {
                        TaskName = t.Task.Name,
                        OpenDate = t.Task.OpenDate.ToString("d", CultureInfo.InvariantCulture),
                        DueDate = t.Task.DueDate.ToString("d", CultureInfo.InvariantCulture),
                        LabelType = t.Task.LabelType.ToString(),
                        ExecutionType = t.Task.ExecutionType.ToString()
                    })
                    .ToArray()
                })
                .ToArray()
                .OrderByDescending(x => x.Tasks.Count())
                .ThenBy(x => x.Username)
                .Take(10)
                .ToArray();

            return JsonConvert.SerializeObject(empl, Formatting.Indented);
        }
    }
}