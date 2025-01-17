using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovation_Library.Models
{
    public class LibrarySignIn
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string StudentEmail { get; set; }
        public string Name { get; set; }
        public string SignInTime { get; set; }
        public string SignOutTime { get; set; }
        public int SessionDuration { get; set; }
        public string SignInKey { get; set; }
        public int BookingID { get; set; }
        public string Status { get; set; }
        public LibrarySignIn()
        {
            this.SessionDuration = 1;
            var guid = Guid.NewGuid().ToString();
            this.SignInKey = guid.Substring(0, 8).ToUpper();
            this.Status = "On-going";
        }
    }
}