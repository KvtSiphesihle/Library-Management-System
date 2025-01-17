using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovation_Library.Models
{
    public class Borrower
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BorrowerId { get; set; }
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string ContactNo { get; set; }
        [DataType(DataType.Date)]
        public DateTime? BorrowedDate { get; set; }
        [DataType(DataType.Date)]
        public string ReturnedDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ExpectedReturnDate { get; set; }
        public string Status { get; set; }
        public int AllocatedBookId { get; set; }
        public string Author { get; set; }
        public string BookTitle { get; set; }
        public string ISBN { get; set; }
        public string BillStatus { get; set; }
        public bool IsPayed { get; set; }
        public bool IsReturned { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsOverDue { get; set; }
        public double BillAmount { get; set; }
        public Borrower()
        {
            this.Status = "Pending";
            this.IsOverDue = false;
            this.IsPayed = false;
            this.BillAmount = 0;
        }
    }
}