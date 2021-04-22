using P01_StudentSystem.Data.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace P01_StudentSystem.Data.Models
{
    public class Homework
    {
        public int HomeworkId { get; set; }
        public string Content { get; set; }
        public DateTime SubmissionTime { get; set; }

        public ContentType ContentType { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        //private enum ContentType
        //{
        //    Application = 1, 
        //    Pdf = 2, 
        //    Zip = 3
        //}
    }
}
//o HomeworkId
//o	Content (string, linking to a file, not unicode)
//o ContentType(enum – can be Application, Pdf or Zip)
//o SubmissionTime
//o	StudentId
//o	CourseId
