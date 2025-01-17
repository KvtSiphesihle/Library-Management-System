using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovation_Library.Models
{
    public class Tutor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TutorId { get; set; }
        public string RegistrationNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string UserId { get; set; }

        public Tutor()
        {
            Random _Random = new Random();
            var RegistrationNumber = "";

            for (int i = 0; i < 3; i++)
            {
                var _RandomNo = _Random.Next(0, 9);
                RegistrationNumber += Convert.ToString(_RandomNo);
            }

            this.RegistrationNumber = DateTime.Now.Year + RegistrationNumber;
        }
    }
}