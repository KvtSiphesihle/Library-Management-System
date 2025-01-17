using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovation_Library.Models
{
    public class Coapon
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string CoaponCode { get; set; }
        public bool IsExpired { get; set; }
        public DateTime Date { get; set; }
        public DateTime ExpiryDate { get; set; }
        public double Discount { get; set; }

        public Coapon()
        {
            this.IsExpired = false;
            this.Date = DateTime.Now;
            this.ExpiryDate = this.Date.AddMinutes(2);
        }
    }
}