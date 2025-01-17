using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovation_Library.Models
{
    public class Exchanges
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExchangeId { get; set; }
        public string ExchangeKey { get; set; }
        public string ExchangeTime { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        public string Status { get; set; }
        public string UserId { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReturnItems> ReturnItems { get; set; }
        public double AmountDue { get; set; }
        public string DriverId { get; set; }
        public bool IsPickedUp { get; set; }
        public bool IsArrived { get; set; }
        public bool IsCollected { get; set; }

        public Exchanges()
        {
            this.Status = "Waiting For Pickup";
            this.ExchangeTime = DateTime.Now.ToLongTimeString();
            var guid = Guid.NewGuid().ToString();
            this.ExchangeKey = guid.Substring(0, 6).ToUpper();
            this.IsArrived = false;
            this.IsPickedUp = false;
            this.IsCollected = false;
        }
    }
}