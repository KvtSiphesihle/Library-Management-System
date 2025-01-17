using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovation_Library.Models
{
    public class Student
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StudentId { get; set; }
        public string ProfilePic { get; set; }
        [NotMapped]
        public HttpPostedFileBase ImageFile { get; set; }        
        public string StudentGuid { get; set; }
        [Required]
        public string StudentName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string ContactNo { get; set; }

        
    }
    
}
