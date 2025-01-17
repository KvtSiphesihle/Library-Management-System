using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovation_Library.Models
{
    public class Announcement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AnnouncementId { get; set; }
        public string UserId { get; set; }
        public string Id { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string AnnouncementTitle { get; set; }
        public DateTime PostedDate { get; set; }
        public string AnnouncementText { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AnnouncementComment> AnnouncementComments { get; set; }
        public Announcement()
        {
            this.PostedDate = DateTime.Now;
        }
    }
}