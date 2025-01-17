using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovation_Library.Models
{
    public class OrderSchedule
    {
        public int Days { get; set; }
        public string OrderNumber { get; set; }
    }
}