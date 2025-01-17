using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovation_Library.Models
{
    public class Ticket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TicketID { get; set; }
        public int EventID { get; set; }
        public virtual Event Event { get; set; }
        public string Address { get; set; }
        public string Time { get; set; }
        public string Date { get; set; }
        public string TicketNumber { get; set; }
        public string UserID { get; set; }
        public bool IsSoldOut { get; set; }
        public Ticket()
        {
            this.IsSoldOut = false;
        }
    }
}