using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovation_Library.Models
{
    public class ContentComment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentId { get; set; }
        public DateTime CommentedAt { get; set; }
        public string CommentText { get; set; }
        public int LearningContentId { get; set; }
        public virtual LearningContent LearningContent { get; set; }
        public string Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public ContentComment()
        {
            this.CommentedAt = DateTime.Now;
        }
    }
}