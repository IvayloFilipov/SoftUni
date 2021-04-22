namespace FastFood.Core.ViewModels.Categories
{
    // this CategoryAllViewModel has only Name property, into the FastFood.Models / Category has Name too, soo mapping will be easy and the AutoMapper will do that along, the map shoul start from Category to this Model here -> CategoryAllViewModel
    public class CategoryAllViewModel
    {
        public string Name { get; set; }
    }
}
