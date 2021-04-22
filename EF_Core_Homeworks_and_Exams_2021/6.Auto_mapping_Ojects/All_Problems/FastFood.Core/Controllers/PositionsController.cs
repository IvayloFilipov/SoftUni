namespace FastFood.Core.Controllers
{
    using System.Linq;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Data;
    using FastFood.Models;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels.Positions;

    public class PositionsController : Controller
    {
        private readonly FastFoodContext context;
        private readonly IMapper mapper;

        // AutoMapper can understand/depends on the name of the class what kind of controller is to search/ and what kind request to make -> PositionsController (it removes the second part of the name and find what it is remain -> Positions). Can check this out into the URL field bar of the initial/front page. The following word after the Positions/Create tell me what kind of method to search/use -> Create 
        public PositionsController(FastFoodContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        // this is GET request, If make GET request (check the enter field)
        public IActionResult Create()
        {
            //return this.View() -> means (depends on the names "View") is going to folder "Views" and enter into the Create.cshtml, in order to return this partiqulare view, specified into the Create.cshtml
            return this.View();
        }

        // this is POST request -> make POST request when enter some data into the Positions fiel and pres send/create ... This do some validations firs if has some validations written into CreatePositionInputModel.cs
        [HttpPost]
        public IActionResult Create(CreatePositionInputModel model)
        {
            // the ModelState has som validation(if have some, now dont have any, can put some by atributes) and can find it into the: ViewModels.Positins.CreatePositionInputModel
            if (!ModelState.IsValid)
            {
                //this is written correct(first is the Method-Error, then is the controller-Home) but redirect as reverce -> Home/Errror. I have into the FastFood -> Home controller and inside - Error method
                return this.RedirectToAction("Error", "Home"); 
            }

            var position = this.mapper.Map<Position>(model);

            this.context.Positions.Add(position);

            this.context.SaveChanges();


            //This redirect to the method below (public IActionResult All())
            return this.RedirectToAction("All", "Positions"); //controller -> Positions, method -> All. 
        }

        public IActionResult All()
        {
            var categories = this.context.Positions
                //using ProjectTo I can mapp entire DB collections
                .ProjectTo<PositionsAllViewModel>(mapper.ConfigurationProvider) //DO NOT forget (mapper.ConfigurationProvider)
                .ToList();

            return this.View(categories);
        }
    }
}
