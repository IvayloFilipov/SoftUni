using System.ComponentModel.DataAnnotations;

namespace FastFood.Core.ViewModels.Categories
{
    public class CreateCategoryInputModel
    {
        // supose to not be able to mapp Category class with this view model due to diff names properties (Name <-> CategoryName)
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string CategoryName { get; set; }
    }
}
