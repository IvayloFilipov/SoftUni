using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeisterMask.DataProcessor.ImportDto
{
    public class JsonImportEmployeeDTO
    {
        [Required]
        [StringLength(40, MinimumLength = 3)]
        //[RegularExpression(@"^\d*\w*$")]
        [RegularExpression(@"^[0-9a-zA-Z]+$")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{3}-[0-9]{3}-[0-9]{4}$")]
        public string Phone { get; set; }

        public int[] Tasks { get; set; }
    }
    
}

