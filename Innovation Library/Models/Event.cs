using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovation_Library.Models
{
    public class Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime FullDate { get; set; }
        public string Location { get; set; }
        public int NumberOfSeats { get; set; }
        public virtual ICollection<Participant> Participants { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
        public bool IsTerminated { get; set; }
        public Event()
        {
            this.IsTerminated = false;
        }
    }
}