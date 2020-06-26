using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace StripeSubscribe.Models
{
    public class Subscription
    {
        [Key]
        public int SubNo { get; set; }
        public string SubID { get; set; }
        public string CustomerID { get; set; }
        public string PlanID { get; set; }
        public Customer Customer { get; set; }
    }
}
