using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Stripe;
using StripeSubscribe.Data;

namespace StripeSubscribe.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IConfiguration _configuration;
        public ApplicationDbContext _database { get; }

        public PaymentController(IConfiguration configuration, ApplicationDbContext database)
        {
            _configuration = configuration;
            _database = database;
        }

        //public IActionResult Subscribe()
        //{
        //    ViewBag.stripeKey = _configuration["Stripe:publishable_key"];
        //    return View();
        //}
        [HttpPost]
        public IActionResult Upgrade(string subKey, string plan) {
            // Set your secret key. Remember to switch to your live secret key in production!
            // See your keys here: https://dashboard.stripe.com/account/apikeys
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

            var service = new SubscriptionService();
            Subscription subscription = service.Get(subKey);

            var planId = _configuration["Stripe:Daily5"];
            if (plan == "Daily10")
            {
                planId = _configuration["Stripe:Daily10"];
            }

            var items = new List<SubscriptionItemOptions> {
                new SubscriptionItemOptions {
                    Id = subscription.Items.Data[0].Id,
                    Price = planId,
                },
            };

            var options = new SubscriptionUpdateOptions
            {
                CancelAtPeriodEnd = false,
                ProrationBehavior = "create_prorations",
                Items = items,
            };
            subscription = service.Update(subKey, options);

            var subUpdate = _database.Subscriptions.Where(i => i.SubID == subKey).Single();

            subUpdate.PlanID = planId;

            _database.SaveChanges();
            return View("UpdateResult");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Subscribe(string cardEmail, string plan, string stripeToken)
        {
            var customerOptions = new CustomerCreateOptions
            {
                Email = cardEmail,
                Source = stripeToken,
            };

            var customerService = new CustomerService();
            var customer = customerService.Create(customerOptions);

            Models.Customer custToDb = new Models.Customer();
            custToDb.CustomerID = customer.Id;
            custToDb.CustomerEmail = cardEmail;
            _database.Customers.Add(custToDb);
            _database.SaveChanges();

            var planId = _configuration["Stripe:Daily5"];
            if (plan == "Daily10")
            {
                planId = _configuration["Stripe:Daily10"];
            }

            var subscriptionOptions = new SubscriptionCreateOptions
            {
                Customer = customer.Id,
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        Plan = planId
                    },
                },
            };
            subscriptionOptions.AddExpand("latest_invoice.payment_intent");

            var subscriptionService = new SubscriptionService();
            var subscription = subscriptionService.Create(subscriptionOptions);

            Models.Subscription subs = new Models.Subscription();
            subs.SubID = subscription.Id;
            subs.CustomerID = customer.Id;
            subs.PlanID = planId;
            _database.Subscriptions.Add(subs);
            _database.SaveChanges();
            ViewBag.stripeKey = _configuration["Stripe:PublishableKey"];
            ViewBag.subscription = subscription.ToJson();

            return View("SubscribeResult");
        }

        //[HttpPost]
        //public IActionResult SubscriptionWebhook()
        //{
        //    string signingSecret = _configuration["Stripe:SecretKey"];

        //    var json = new StreamReader(HttpContext.Request.Body).ReadToEnd();

        //    try
        //    {

        //        var stripeEvent = EventUtility.ConstructEvent(json,
        //            Request.Headers["Stripe-Signature"],
        //            signingSecret,
        //            throwOnApiVersionMismatch: true);

        //        if (stripeEvent == null)
        //        {
        //            return BadRequest("Event was null");
        //        }

        //        switch (stripeEvent.Type)
        //        {
        //            case "invoice.payment_succeeded":
        //                // Do something with the event for when the payment goes through
        //                Invoice successInvoice = (Invoice)stripeEvent.Data.Object;
        //                return Ok();
        //            case "invoice.payment_failed":
        //                // Do something with the event for when the payment fails
        //                Invoice failInvoice = (Invoice)stripeEvent.Data.Object;
        //                return Ok();
        //            default:
        //                return BadRequest("Event was not valid type");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e);
        //    }
        //}
    }
}
