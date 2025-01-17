using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Innovation_Library.Models
{
    public class LearningContent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LearningContentId { get; set; }
        public string UserId { get; set; }
        public string Id { get; set; }
        public virtual ApplicationUser User { get; set; }
        [NotMapped]
        public HttpPostedFileBase ImageFile { get; set; }
        public string SessionCover { get; set; }
        public string Title { get; set; }
        public string YouTubeLink { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContentComment> ContentComments { get; set; }

    }
}