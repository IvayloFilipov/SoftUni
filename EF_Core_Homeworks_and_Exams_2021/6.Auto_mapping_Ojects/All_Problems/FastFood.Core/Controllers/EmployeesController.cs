namespace FastFood.Core.Controllers
{
    using System;
    using System.Linq;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Data;
    using FastFood.Models;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels.Employees;

    public class EmployeesController : Controller
    {
        private readonly FastFoodContext context;
        private readonly IMapper mapper;

        public EmployeesController(FastFoodContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        //GET request
        public IActionResult Register()
        {
            var positions = this.context
                .Positions
                .ProjectTo<RegisterEmployeeViewModel>(this.mapper.ConfigurationProvider)
                .ToList();

            //implement returns; pass to view positions
            return this.View(positions);

        }

        //POST request: this submit to DB
        [HttpPost]
        public IActionResult Register(RegisterEmployeeInputModel model)
        {
            //first make validation; if is not valid
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction("Error", "Home");
            }

            // manual mapper
            //var emploee1 = new Employee
            //{
            //    Name = model.Name,
            //    Address = model.Address,
            //    Age = model.Age,
            //    PositionId = model.PositionId
            //};

            var employee = mapper.Map<Employee>(model); //Auto Mapper

            this.context.Employees.Add(employee); //AMapper-employee, MMapper-emploee1

            this.context.SaveChanges();

            //This redirect to the method below (public IActionResult All())
            return this.RedirectToAction("All", "Employees"); //method-All, controler-Employees

        }

        //implement the method All; this method must come to the All.cshtml and rendering it
        public IActionResult All()
        {
            //after made mapping into FastFoodProfile-Employees must com here and take all employees
            var employees = this.context
                .Employees
                .ProjectTo<EmployeesAllViewModel>(this.mapper.ConfigurationProvider)
                .ToList();

            // finaly give the employees to the view and return them -> this.View(employees);
            return this.View(employees);

        }
    }
}
