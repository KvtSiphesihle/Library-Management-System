using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovation_Library.Models
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookId { get; set; }
        [Required]
        public string BookTitle { get; set; }
        [Required]
        public double BookPrice { get; set; }
        public string NoOfPages { get; set; }
        public string ShelfNo { get; set; }
        public string Row { get; set; }
        public int Quantity { get; set; }
        public string Publisher { get; set; }
        public string PublishedDate { get; set; }
        public int Views { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        [NotMapped]
        public HttpPostedFileBase ImageFile { get; set; }
        public int PurchaseCount { get; set; }
        public string ContentType { get; set; }

        public Book()
        {
            this.PurchaseCount = 0;
        }
    }
}