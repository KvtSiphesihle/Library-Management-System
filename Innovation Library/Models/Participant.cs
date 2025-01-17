using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Innovation_Library.Models
{
    public class Participant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ParticipantID { get; set; }
        public string UserID { get; set; }
        public int EventID { get; set; }
        public virtual Event Event { get; set; }
    }
}