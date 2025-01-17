using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovation_Library.Models
{
    public class BookRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Author { get; set; }
        [Required]
        public string BookTitle { get; set; }
        public string PublicationYear { get; set; }
        public string ISBN { get; set; }
        public string UserId { get; set; }
        public string RequestDate { get; set; }
        public string Status { get; set; }
        public bool IsRequestApproved { get; set; }

        public BookRequest()
        {
            this.RequestDate = DateTime.Now.ToLongDateString();
            this.Status = "Pendind Approval";
            this.IsRequestApproved = false;
        }
    }
}