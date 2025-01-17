using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovation_Library.Models
{
    public class Inspection
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string BillAmount { get; set; }
        public string Notes { get; set; }
        public bool IsReturnedOnTime { get; set; }
        public bool IsOriginalPackaged { get; set; }
        public bool IsWorking { get; set; }
        public bool IsScratched { get; set; }
        public bool IsWaterDamaged { get; set; }
        public int HiringId { get; set; }
        public string UserID { get; set; }
    }
}