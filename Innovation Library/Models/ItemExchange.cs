using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovation_Library.Models
{
    public class ItemExchange
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ItemExchangeID { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public double ItemPrice { get; set; }
        public string OrderNo { get; set; }
        public string ExchangeType { get; set; }
        public string ExchangeReason { get; set; }
        public string ExchangeOtherReason { get; set; }
    }
}