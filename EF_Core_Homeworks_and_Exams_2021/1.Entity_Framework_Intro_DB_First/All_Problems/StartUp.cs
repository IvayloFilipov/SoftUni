using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();

            // Invoke particular function and print result on console
            string result = RemoveTown(context);
            Console.WriteLine(result);
        }

        // Problem 2. - ok 100/100 - 

        //Problem 3 - ok 100/100 -
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employeesData = context
                .Employees
                .Select(e => new
                {
                    EmployeeId = e.EmployeeId,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    MidleName = e.MiddleName,
                    JobTitle = e.JobTitle,
                    Salary = e.Salary
                })
                .OrderBy(e => e.EmployeeId)
                .ToList();

            foreach (var employee in employeesData)
            {
                sb
                    .AppendLine($"{employee.FirstName} {employee.LastName} {employee.MidleName} {employee.JobTitle} {employee.Salary:F2}");
            }

            var result = sb.ToString().TrimEnd();
            return result;
        }

        // Problem 4 - ok 100/100 -
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employeeData = context
                .Employees
                .Where(e => e.Salary > 50000)
                .Select(e => new
                {
                    e.FirstName,
                    e.Salary
                })
                //.Where(e => e.Salary > 50000) // or here
                .OrderBy(e => e.FirstName)
                .ToList();

            foreach (var empl in employeeData)
            {
                sb
                    .AppendLine($"{empl.FirstName} - {empl.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        // Problem 5 - ok 100/100 -
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var emploeeData = context
                .Employees
                .Where(e => e.Department.Name == "Research and Development")
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    DepartmentName = e.Department.Name,
                    e.Salary
                })
                .OrderBy(x => x.Salary)
                .ThenByDescending(e => e.FirstName)
                .ToList();

            foreach (var empl in emploeeData)
            {
                sb
                    .AppendLine($"{empl.FirstName} {empl.LastName} from {empl.DepartmentName} - ${empl.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        // Problem 6 - ok 100/100 -
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            // create new Address
            Address newAddress = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            //Employee targetEmpl = context
            //    .Employees
            //    .Where(e => e.LastName == "Nakov")
            //    .FirstOrDefault();

            // or
            Employee targetEmpl = context
                .Employees
                .FirstOrDefault(e => e.LastName == "Nakov");

            // context.Addresses.Add(newAddress); // Can be omitted

            // or use this below to add Address to a certain person.
            targetEmpl.Address = newAddress;

            // or just update the targetEmpl.Address with the new one (after the targetEmpl was found above)
            //targetEmpl.Address = new Address 
            //{
            //    AddressText = "Vitoshka 15",
            //    TownId = 4
            //};

            context.SaveChanges();

            var addressData = context
                .Employees
                .OrderByDescending(e => e.AddressId)
                .Take(10)
                .Select(e => e.Address.AddressText)
                //.Select(e => new
                //{
                //    AddressText = e.Address.AddressText
                //})
                .ToList();

            foreach (var address in addressData)
            {
                sb
                    .AppendLine(address);
            }

            return sb.ToString().TrimEnd();
        }

        // Problem 7 - ok 100/100 -
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employeesData = context
                .Employees
                .Where(e => e.EmployeesProjects
                             .Any(ep => ep.Project.StartDate.Year >= 2001 &&
                                        ep.Project.StartDate.Year <= 2003))
                .Take(10)
                .Select(e => new 
                { 
                    e.FirstName,
                    e.LastName,
                    ManagerFirstName = e.Manager.FirstName,
                    ManagerLastName = e.Manager.LastName,
                    AllProjects = e.EmployeesProjects
                                   .Select(ep => new 
                                    { 
                                       ProjectName = ep.Project.Name,
                                       StartDate = ep.Project
                                                     .StartDate
                                                     .ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
                                       EndDate = ep.Project
                                                   .EndDate
                                                   .HasValue ? ep.Project
                                                                 .EndDate
                                                                 .Value
                                                                 .ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture) 
                                                             : "not finished"
                                   })
                })
                .ToList();

            foreach (var item in employeesData)
            {
                sb
                    .AppendLine($"{item.FirstName} {item.LastName} - Manager: {item.ManagerFirstName} {item.ManagerLastName}");

                foreach (var prj in item.AllProjects)
                {
                    sb
                        .AppendLine($"--{prj.ProjectName} - {prj.StartDate} - {prj.EndDate}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        // Problem 8 - ok 100/100 -
        public static string GetAddressesByTown(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employeeData = context
                .Addresses
                .Where(a => a.Employees.Count() > 0)
                .Select(t => new
                {
                    AddressText = t.AddressText,
                    TownName = t.Town.Name,
                    EmployeeCount = t.Employees.Count()
                })
                .OrderByDescending(x => x.EmployeeCount)
                .ThenBy(x => x.TownName)
                .ThenBy(x => x.AddressText)
                .Take(10)
                .ToList();

            foreach (var item in employeeData)
            {
                sb
                    .AppendLine($"{item.AddressText}, {item.TownName} - {item.EmployeeCount} employees");
            }
            return sb.ToString().TrimEnd();
        }

        // Problem 9 - ok 100/100 -
        public static string GetEmployee147(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var eployeesData = context
                .Employees
                .Where(e => e.EmployeeId == 147)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    ProjectNames = e.EmployeesProjects
                                    .Select(ep => ep.Project.Name)
                                    .OrderBy(ep => ep)
                                    .ToList()
                })
                .ToList();

            foreach (var item in eployeesData)
            {
                sb
                    .AppendLine($"{item.FirstName} {item.LastName} - {item.JobTitle}");

                foreach (var prj in item.ProjectNames)
                {
                    sb.AppendLine(prj);
                }
            }

            return sb.ToString().TrimEnd();
        }

        // Problem 10 - ok 100/100 -
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var departmentsData = context
                .Departments
                .Where(d => d.Employees.Count > 5)
                .OrderBy(d => d.Employees.Count)
                .ThenBy(d => d.Name)
                .Select(d => new
                {
                    DepartmentName = d.Name,
                    ManagerFirstName = d.Manager.FirstName,
                    ManagerLastName = d.Manager.LastName,
                    EmployeeData = d.Employees
                        .Select(e => new 
                        {
                            EmplFirstName = e.FirstName,
                            EmplLastName = e.LastName,
                            EmplJobTitle = e.JobTitle
                        })
                        .OrderBy(e => e.EmplFirstName)
                        .ThenBy(e => e.EmplLastName)
                        .ToList()
                })
                .ToList();

            foreach (var item in departmentsData)
            {
                sb
                    .AppendLine($"{item.DepartmentName} - {item.ManagerFirstName} {item.ManagerLastName}");

                foreach (var empl in item.EmployeeData)
                {
                    sb.AppendLine($"{empl.EmplFirstName} {empl.EmplLastName} - {empl.EmplJobTitle}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        // Problem 11 - ok 100/100 -
        public static string GetLatestProjects(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var projectsData = context
                .Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .Select(p => new
                {
                    p.Name,
                    p.Description,
                    p.StartDate
                })
                .OrderBy(p => p.Name)
                .ToList();

            foreach (var item in projectsData)
            {
                sb
                    .AppendLine(item.Name)
                    .AppendLine(item.Description)
                    .AppendLine(item.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture));
            }

            return sb.ToString().TrimEnd();
        }

        // Problem 12 - ok 100/100 -
        public static string IncreaseSalaries(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            string[] selectedDepartments = new string[] { "Engineering", "Tool Design", "Marketing", "Information Services" };

            // not materialized !!!
            var selectedEmployees = context
                .Employees
                .Where(d => selectedDepartments.Contains(d.Department.Name)); // here employees are still attached/tracked to DB -> IQueriable (due to I have not put .ToList() at the end)

            // here I can directly increase the salary into the DB of the selected employees
            foreach (var empl in selectedEmployees)
            {
                empl.Salary *= 1.12m;
            }

            context.SaveChanges(); // this command execute the changes on records from above selection directly into the DB

            //var emplAfterIncreasedSalary = context
            //    .Employees
            //    .Where(d => selectedDepartments.Contains(d.Department.Name))
            
                // or directly -> 
                var emplAfterIncreasedSalary = selectedEmployees 
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList();

            foreach (var item in emplAfterIncreasedSalary)
            {
                sb.AppendLine($"{item.FirstName} {item.LastName} (${item.Salary:f2})");
            }

            return sb.ToString().TrimEnd();
        }

        // Problem 13 - ok 100/100 -
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employeesData = context
                .Employees
                .Where(e => e.FirstName.StartsWith("Sa")) // here it is case insensitive (could be "sa" as well)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList();

            foreach (var item in employeesData)
            {
                sb
                    .AppendLine($"{item.FirstName} {item.LastName} - {item.JobTitle} - (${item.Salary:f2})"); 
            }

            return sb.ToString().TrimEnd();
        }

        // Problem 14 - ok 100/100 -
        public static string DeleteProjectById(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            //var projectToDelete = context
            //    .Projects
            //    .Where(p => p.ProjectId == 2);

            // or
            var projectToRemove = context
                .Projects
                .FirstOrDefault(p => p.ProjectId == 2);

            // collection of all EmployeeId related to ProjectId = 2
            var employeesIdToRemoveFromProjects = context
                .EmployeesProjects
                .Where(ep => ep.ProjectId == 2)
                .ToList();

            // remove all EmployeeId's related to ProjectId = 2 from EmployeesProjects 
            foreach (var employeeId in employeesIdToRemoveFromProjects) 
            {
                context.EmployeesProjects.Remove(employeeId); 
            }

            context.Projects.Remove(projectToRemove);
            context.SaveChanges();

            var projectsData = context
                .Projects
                .Select(p => p.Name)
                .Take(10)
                .ToList();

            foreach (var prjName in projectsData)
            {
                sb.AppendLine(prjName);
            }

            return sb.ToString().TrimEnd();
        }

        // Problem 15
        public static string RemoveTown(SoftUniContext context)
        {
            // no materialization !!!
            var townToRemove = context
                .Towns
                .FirstOrDefault(t => t.Name == "Seattle");

            var addressesInGivenTownToBeRemoved = context
                .Addresses
                .Where(t => t.Town.TownId == townToRemove.TownId);
	    
	    // or -> var addressesInGivenTownToBeRemoved = townToRemove.Addresses.Select(x => x.AddressId);

            // take the number of towns to be deleted
            int countDelTowns = addressesInGivenTownToBeRemoved.Count();

            var employeesAddressIdToSetNull = context
                .Employees
                .Where(a => addressesInGivenTownToBeRemoved.Any(e => e.AddressId == a.AddressId));
		// .Where(a => addressesInGivenTownToBeRemoved.Contains(a.AddressId.Value));

            // 1. set AddressId to Null 
            foreach (var item in employeesAddressIdToSetNull)
            {
                item.AddressId = null;
            }

            // 2. remove all the addresses from the context.Addresses
            foreach (var currAddress in addressesInGivenTownToBeRemoved)
            {
                context.Addresses.Remove(currAddress);
            }

            // 3. remove the given town
            context.Towns.Remove(townToRemove);

            context.SaveChanges();

            string result = $"{countDelTowns} addresses in {townToRemove.Name} were deleted";
            return result; ;
        }
    }
}
