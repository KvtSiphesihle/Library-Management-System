using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovation_Library.Models
{
    public class OrderExchange
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderExchangeId { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        public string ExchangeType { get; set; }
        public string ExchangeReason { get; set; }
        public string ExchangeOtherReason { get; set; }
    }
}