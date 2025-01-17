using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Innovation_Library.Models
{
    public class Slides
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int SlideID { get; set; }
        public string UserID { get; set; }
        public string SlideTitle { get; set; }
        [NotMapped]
        public HttpPostedFileBase SlideFile { get; set; }
        public string SlideUrl { get; set; }
    }
}