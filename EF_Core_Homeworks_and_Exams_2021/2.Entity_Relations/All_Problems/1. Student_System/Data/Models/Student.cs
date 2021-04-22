using System;
using System.Collections.Generic;
using System.Text;

namespace P01_StudentSystem.Data.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime RegisteredOn { get; set; }
        public DateTime? Birthday { get; set; }

        public ICollection<StudentCourse> CourseEnrollments { get; set; }
            = new HashSet<StudentCourse>();

        public ICollection<Homework> HomeworkSubmissions { get; set; }
            = new HashSet<Homework>();
    }
}
//o StudentId
//o	Name (up to 100 characters, unicode)
//o PhoneNumber(exactly 10 characters, not unicode, not required)
//o RegisteredOn
//o	Birthday (not required)
