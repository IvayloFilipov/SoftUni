namespace FastFood.Core.Controllers
{
    using System.Diagnostics;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels;

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            //return view that accept ErrorViewModel
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
