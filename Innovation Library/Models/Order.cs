using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovation_Library.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OrderNo { get; set; }
        public string CustomerId { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string OrderDate { get; set; }
        public string Address { get; set; }
        public double Total { get; set; }
        public string Status { get; set; }
        public bool isPayed { get; set; }
        public bool IsPicked { get; set; }
        public bool IsOnRoute { get; set; }
        public bool isDelivered { get; set; }
        public bool IsDeliveryConfirmed { get; set; }
        public string ArrivalDate { get; set; }
        public bool IsAssigned { get; set; }
        public bool IsCancelled { get; set; }
        public string AssignedDriverId { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetails> OrderDetails { get; set; }

        public Order()
        {
            this.IsAssigned = false;
            this.AssignedDriverId = null;
            this.OrderDate = DateTime.Now.ToString();
            this.isPayed = false;
            this.IsPicked = false;
            this.IsOnRoute = true;
            this.isDelivered = false;
            this.Status = "Order Received";
            this.IsDeliveryConfirmed = false;
            this.ArrivalDate = DateTime.Now.AddMinutes(3).ToString();
            this.IsCancelled = false;
        }
    }
}