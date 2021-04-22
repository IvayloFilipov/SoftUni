namespace FastFood.Core.MappingConfiguration
{
    using AutoMapper;
    using FastFood.Core.ViewModels.Categories;
    using FastFood.Core.ViewModels.Employees;
    using FastFood.Core.ViewModels.Items;
    using FastFood.Core.ViewModels.Orders;
    using FastFood.Models;
    using System;
    using ViewModels.Positions;

    // class that take care of mapping
    public class FastFoodProfile : Profile
    {
        //here in the FastFoodProfile (Profile file) we will make the mapping configurations between objects
        public FastFoodProfile()
        {
            //Positions       
            //In CreateMap -> first is source(CreatePositionInputModel), then is target(Position)
            this.CreateMap<CreatePositionInputModel, Position>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.PositionName));

            //if map properties dont match names (names convention) we use .ForMemebers()
            //Position - source, PositionsAllViewModel - destination/target
            //x.Name - destination prop,   s.Name - source
            this.CreateMap<Position, PositionsAllViewModel>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.Name));

            // still work with Positions
            //Position - source, RegisterEmployeeViewModel - destination/target
            //x.PositionId - destination prop,   s.Id - source
            this.CreateMap<Position, RegisterEmployeeViewModel>()
                .ForMember(x => x.PositionId, y => y.MapFrom(s => s.Id))
                .ForMember(x => x.PositionName, y => y.MapFrom(s => s.Name));


            //Emploees      
            this.CreateMap<RegisterEmployeeInputModel, Employee>(); //RegisterEmployeeInputModel-source, Employee-target    

            //come from EmployeesAllViewModels and do mapping
            this.CreateMap<Employee, EmployeesAllViewModel>()
                .ForMember(x => x.Position, y => y.MapFrom(s => s.Position.Name));


            //Categories        
            this.CreateMap<CreateCategoryInputModel, Category>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.CategoryName));

            this.CreateMap<Category, CategoryAllViewModel>(); //no nead to add .ForMemeber due to the same property names

            this.CreateMap<Category, CreateItemViewModel>()
                .ForMember(x => x.CategoryId, y => y.MapFrom(s => s.Id));


            //Items
            this.CreateMap<CreateItemInputModel, Item>(); //here have full naming property match 

            this.CreateMap<Item, ItemsAllViewModels>()
                .ForMember(x => x.Category, y => y.MapFrom(s => s.Category.Name)); // due to in ItemsAllViewModels Category is of type string but in .Models/Item Categoty is from type Category

            //Order
            this.CreateMap<CreateOrderInputModel, Order>();
            //.ForMember(x => x.DateTime, y => y.MapFrom(s => DateTime.Now)); // NO  s.Xxxxx  here, just s => DateTime.Now

            this.CreateMap<Order, OrderAllViewModel>()
                .ForMember(x => x.Employee, y => y.MapFrom(s => s.Employee.Name))
                .ForMember(x => x.OrderId, y => y.MapFrom(s => s.Id));
                //.ForMember(x => x.DateTime, y => y.MapFrom(s => s.DateTime.ToString("d"))); //to visualise date az it is on comuter

            this.CreateMap<CreateOrderViewModel, Order>();
        }
    }
}
