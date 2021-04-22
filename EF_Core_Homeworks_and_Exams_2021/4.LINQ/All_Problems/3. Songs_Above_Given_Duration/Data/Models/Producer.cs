using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MusicHub.Data.Models
{
    public class Producer
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        public string Pseudonym { get; set; }
        public string PhoneNumber { get; set; }

        public ICollection<Album> Albums { get; set; } = new HashSet<Album>();
    }
}
//•	Id – Integer, Primary Key
//•	Name – text with max length 30 (required)
//•	Pseudonym – text
//•	PhoneNumber – text
//•	Albums – collection of type Album
