using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovation_Library.Models
{
    public class Room
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public int Capacity { get; set; }
        public string Status { get; set; }
        public int TableDesk { get; set; }
        public int Whiteboard { get; set; }
        public int Projector { get; set; }
        public bool isAvailable { get; set; }
        public string Image { get; set; }
        [NotMapped]
        public HttpPostedFileBase ImageFile { get; set; }
        public Room()
        {
            this.Status = "Available";
            this.isAvailable = true;
        }
    }
}