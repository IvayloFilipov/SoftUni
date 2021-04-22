namespace FastFood.Core.ViewModels.Employees
{
    public class EmployeesAllViewModel
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public string Address { get; set; }

        public string Position { get; set; } //this will not be anle to map to it. Must tell to mapper how to mapp it. Go to FastFoodModels and do mapping. 
    }
}
