using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovation_Library.Models
{
    public class Shelves
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ShelfId { get; set; }
        [Required]
        public string ShelfNo { get; set; }
    }
}