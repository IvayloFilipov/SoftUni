using System.ComponentModel.DataAnnotations;

namespace FastFood.Core.ViewModels.Positions
{
    public class CreatePositionInputModel
    {
        // here also make validations
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string PositionName { get; set; }
    }
}
