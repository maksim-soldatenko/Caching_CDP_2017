using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CachingDemo.Models
{
    public class Entry
    {
        public int SalesOrderId { get; set; }
        public int ProductId { get; set; }
        public string CarrierTrackingNumber { get; set; }
        public decimal UnitPrice { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string ProductNumber { get; set; }
    }
}