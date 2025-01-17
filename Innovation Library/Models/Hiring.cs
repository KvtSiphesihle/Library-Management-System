using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovation_Library.Models
{
    public class Hiring
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HiringId { get; set; }
        public string StudentId { get; set; }
        public string StudentEmail { get; set; }
        //public int Duration { get; set; }
        public string HireDate { get; set; }
        public string NoOfDays { get; set; }
        public string ExpectedReturn { get; set; }
        public double HireTotalAmount { get; set; }
        public int ItemId { get; set; }
        public string BillStatus { get; set; }
        public string ReferenceNo { get; set; }
        public bool IsPayed { get; set; }
        public bool IsAccepted { get; set; }

        public Hiring()
        {
            this.BillStatus = "Pending";
            var guid = Guid.NewGuid().ToString();
            this.ReferenceNo = guid.Substring(0, 8).ToUpper();
            this.IsPayed = false;
            this.IsAccepted = false;
        }
    }
}