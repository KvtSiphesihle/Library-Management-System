using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovation_Library.Models
{
    public class Driver
    {
        public int Id { get; set; }
        public string RegistrationNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public int AssignedOrders { get; set; }
        public bool IsAvailable { get; set; }
        public string UserId { get; set; }

        public Driver()
        {
            Random _Random = new Random();
            var RegistrationNumber = "";

            for (int i = 0; i < 3; i++)
            {
                var _RandomNo = _Random.Next(0, 9);
                RegistrationNumber += Convert.ToString(_RandomNo);
            }

            this.RegistrationNumber = DateTime.Now.Year + RegistrationNumber;
            this.IsAvailable = true;
            this.AssignedOrders = 0;
        }
    }
}