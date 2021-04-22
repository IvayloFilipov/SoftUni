namespace FastFood.Core.Controllers
{
    using System;
    using System.Linq;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Data;
    using FastFood.Models;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels.Categories;

    public class CategoriesController : Controller
    {
        private readonly FastFoodContext context;
        private readonly IMapper mapper;

        public CategoriesController(FastFoodContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        //GET
        public IActionResult Create()
        {
            //here it is OK like this (Create is rendered correctly). На това View не се подава никакъв модел
            return this.View();
        }

        //POST
        [HttpPost]
        public IActionResult Create(CreateCategoryInputModel model)
        {
            // first check whether the model is valid
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction("Error", "Home"); // first - action(Error), then - controller(Home)
            }

            // now must create Category and to mapp the model to the Category
            var category = this.mapper.Map<Category>(model);

            //now after I allready have the category I should add it to the 
            this.context.Categories.Add(category);

            // and now must Save the changes. This save the changes into the DB and is obligatory
            this.context.SaveChanges();

            //Finaly must return and redirect to the method below (public IActionResult All())
            return this.RedirectToAction("All", "Categories"); //the action is-All, the controler is-Categories
        }

        public IActionResult All()
        {
            //implement IActionResult All()
            //first take all categories
            var categories = this.context
                .Categories
                //in Views/Categories/All.cshtml -> the view expect CategoryAllViewModel, so make .ProjectTo<>() below
                .ProjectTo<CategoryAllViewModel>(this.mapper.ConfigurationProvider)
                .ToList();

            //finaly must return the View fill with categories
            return this.View(categories);
        }
    }
}
