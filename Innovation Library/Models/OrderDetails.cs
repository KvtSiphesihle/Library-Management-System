using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovation_Library.Models
{
    public class OrderDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderDetailsId { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        public int BookId { get; set; }
        public virtual Book Book { get; set; }
        public double? ProductPrice { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; }
    }
}