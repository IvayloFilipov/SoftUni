namespace FastFood.Core.Controllers
{
    using System;
    using System.Linq;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Data;
    using FastFood.Models;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels.Items;

    public class ItemsController : Controller
    {
        private readonly FastFoodContext context;
        private readonly IMapper mapper;

        public ItemsController(FastFoodContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        //GET
        public IActionResult Create()
        {
            var categories = this.context.Categories
                .ProjectTo<CreateItemViewModel>(this.mapper.ConfigurationProvider)
                .ToList();

            return this.View(categories);
        }

        //POST
        [HttpPost]
        public IActionResult Create(CreateItemInputModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction("Error", "Home");
            }

            var item = this.mapper.Map<Item>(model);

            this.context.Items.Add(item);

            this.context.SaveChanges();

            return this.RedirectToAction("All", "Items");

            // and I need mapping between CreateItemInputModel and Item -> go to FastFoodProfile and create it
        }

        public IActionResult All()
        {
            var items = context
                .Items
                .ProjectTo<ItemsAllViewModels>(this.mapper.ConfigurationProvider)
                .ToList();

            return this.View(items);
        }
    }
}
