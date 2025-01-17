using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovation_Library.Models
{
    public class Refund
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RefundID { get; set; }
        public string AccountHolderName { get; set; }
        public string AccountNumber { get; set; }
        public double RefundAmount { get; set; }
        public string UserID { get; set; }
    }
}