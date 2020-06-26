using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StripeSubscribe.Data;
using StripeSubscribe.Models;

namespace StripeSubscribe.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionsController : ControllerBase
    {
        public ApplicationDbContext Database { get; }

        public SubscriptionsController (ApplicationDbContext database)
        {
            Database = database;
        }

        [HttpGet]
        public IActionResult Get()
        {
            List<object> subscriptions = new List<object>();
            var allSubcriptions = Database.Subscriptions;
            foreach (var oneSubscription in allSubcriptions)
            {
                subscriptions.Add(new
                {
                    subNo = oneSubscription.SubNo,
                    subId = oneSubscription.SubID,
                    planId = oneSubscription.PlanID,
                    customerID = oneSubscription.CustomerID

                });
            }//end of foreach
            return Ok(subscriptions);
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {

            //var oneSubscription = Database.Subscriptions.First(i => i.CustomerID == id);
            List<object> subscriptions = new List<object>();
            var allSubcriptions = Database.Subscriptions.Where(i => i.CustomerID == id);
            foreach (var oneSubscription in allSubcriptions)
            {
                subscriptions.Add(new
                {
                    subNo = oneSubscription.SubNo,
                    subId = oneSubscription.SubID,
                    planId = oneSubscription.PlanID,
                    customerID = oneSubscription.CustomerID

                });
            }//end of foreach

            return Ok(subscriptions);
        }
    }
}
