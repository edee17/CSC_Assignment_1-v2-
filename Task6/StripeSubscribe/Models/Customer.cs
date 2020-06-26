using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StripeSubscribe.Models
{
    public class Customer
    {
        //public int CustomerNo { get; set; }
        public string CustomerID { get; set; }
        public string CustomerEmail { get; set; }
        public List<Subscription> Subscriptions { get; set; } 
    }
}
