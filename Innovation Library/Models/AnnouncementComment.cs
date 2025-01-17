using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Innovation_Library.Models
{
    public class AnnouncementComment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentId { get; set; }
        public string CommentText { get; set; }
        public DateTime CommentedAt { get; set; }
        public int AnnouncementId { get; set; }
        public virtual Announcement Announcement { get; set; }
        public string Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public AnnouncementComment()
        {
            this.CommentedAt = DateTime.Now;
        }
    }
}