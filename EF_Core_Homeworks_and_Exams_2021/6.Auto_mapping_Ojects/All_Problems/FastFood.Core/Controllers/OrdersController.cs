namespace FastFood.Core.Controllers
{
    using System;
    using System.Linq;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Data;
    using FastFood.Models;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels.Orders;

    public class OrdersController : Controller
    {
        private readonly FastFoodContext context;
        private readonly IMapper mapper;

        public OrdersController(FastFoodContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        //GET
        public IActionResult Create()
        {
            var viewOrder = new CreateOrderViewModel
            {
                Items = this.context.Items.Select(x => x.Id).ToList(),
                Employees = this.context.Employees.Select(x => x.Id).ToList(),
            };

            var order = this.mapper.Map<Order>(viewOrder);

            this.context.AddRange(order);

            this.context.SaveChanges();

            return this.View(viewOrder);
        }

        //POST
        [HttpPost]
        public IActionResult Create(CreateOrderInputModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction("Error", "Home");
            }

            var orders = this.mapper.Map<Order>(model);
            orders.OrderItems.Add(new OrderItem { ItemId = model.ItemId }); //manual mapping, not AutoMapper in FastFoodProfile

            this.context.Orders.Add(orders);

            this.context.SaveChanges();

            return this.RedirectToAction("All", "Orders");

            //do mapping config between CreateOrderInputModel and Order
        }

        public IActionResult All()
        {
            var allOrders = this.context
                .Orders
                .ProjectTo<OrderAllViewModel>(this.mapper.ConfigurationProvider)
                .ToList();

            return this.View(allOrders);

            //do mapping between Order to OrderAllViewModel
        }
    }
}
