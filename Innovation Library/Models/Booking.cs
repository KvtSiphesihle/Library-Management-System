using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovation_Library.Models
{
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookingId { get; set; }
        public string StudentId { get; set; }
        public int Duration { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public string DelayTime { get; set; }
        public int RoomId { get; set; }
        public string Status { get; set; }
        public string SigningCode { get; set; }
        public bool IsSessionExpired { get; set; }
        public bool IsSignedIn { get; set; }
        public bool IsSignedOut { get; set; }
        public Booking()
        {
            this.Status = "On-going";
            var guid = Guid.NewGuid().ToString();
            this.SigningCode = guid.Substring(0, 6).ToUpper();
            this.IsSessionExpired = false;
            this.IsSignedIn = false;
            this.IsSignedOut = false;
        }
    }
}