using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovation_Library.Models
{
    public class ReturnItems
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int BookId { get; set; }
        public virtual Book Book { get; set; }
        public int ExchangeId { get; set; }
        public virtual Exchanges Exchanges { get; set; }       
    }
}