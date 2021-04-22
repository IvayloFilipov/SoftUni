using P01_StudentSystem.Data.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace P01_StudentSystem.Data.Models
{
    public class Resource
    {
        public int ResourceId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        public ResourceType ResourceType { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        //private enum ResourceType
        //{
        //    Video = 1, 
        //    Presentation = 2, 
        //    Document = 3,
        //    Other = 4
        //}
    }
}
//o ResourceId
//o	Name (up to 50 characters, unicode)
//o Url(not unicode)
//o ResourceType(enum – can be Video, Presentation, Document or Other)
//o CourseId
