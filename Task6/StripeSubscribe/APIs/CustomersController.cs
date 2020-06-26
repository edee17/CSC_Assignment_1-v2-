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
    public class CustomersController : ControllerBase
    {
        public ApplicationDbContext Database { get; }

        public CustomersController(ApplicationDbContext database)
        {
            Database = database;
        }

        [HttpGet]
        public IActionResult Get()
        {
            List<object> customers = new List<object>();
            var allCustomers = Database.Customers;
            foreach (var oneCustomer in allCustomers)
            {
                customers.Add(new
                {
                    customerId = oneCustomer.CustomerID,
                    customerEmail = oneCustomer.CustomerEmail

                });
            }//end of foreach
            return Ok(customers);
        }
        [HttpPost("GetByEmail")]
        public IActionResult GetByEmail([FromForm]IFormCollection data)
        {
            string email = data["email"];
            //var oneCustomer = Database.Customers.First(x => x.CustomerEmail == email);
            Customer oneCustomer = new Customer();
            try
            {
                oneCustomer = Database.Customers.First(x => x.CustomerEmail == email);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok(oneCustomer);
        }
    }
}
