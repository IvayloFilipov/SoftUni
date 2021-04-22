using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SoftJail.DataProcessor.ImportDto
{
    public class JsonImportDepAndCellDTO
    {
        [Required]
        [MinLength(3)]
        [MaxLength(25)]
        public string Name { get; set; }

        public InnerDTOCells[] Cells { get; set; }
    }
    //•	Name – text with min length 3 and max length 25 (required)

    public class InnerDTOCells
    {
        [Required]
        [Range(1, 1000)]
        public int CellNumber { get; set; }

        [Required]
        public bool HasWindow { get; set; }
    }
    //•	CellNumber – integer in the range [1, 1000] (required)
    //•	HasWindow – bool (required)

}
//{
//    "Name": "",
//    "Cells": [
//      {
//        "CellNumber": 101,
//        "HasWindow": true
//      },
//      {
//        "CellNumber": 102,
//        "HasWindow": false
//      }
//    ]
//  },
