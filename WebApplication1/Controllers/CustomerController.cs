using System.Web.Http.Cors;

using System;
using System.Linq;
using System.Web.Http;
using WebApplication1.Models;
using WebApplication1.Models.Dtos;


namespace WebApplication1.Controllers
{
    
    public class CustomerController : ApiController
    {
        private readonly GlamoraaEntities db = new GlamoraaEntities();

        // GET /api/customers
        [HttpGet]
        [Route("api/customers")]
        public IHttpActionResult GetCustomers(string name = null, string phoneNumber = null, string status = null)
        {
            var customersQuery = db.Customers.Where(c => c.Status == "Active").AsQueryable();


            // Search by Name (partial match)
            if (!string.IsNullOrEmpty(name))
            {
                customersQuery = customersQuery.Where(c => c.FullName.Contains(name));
            }

            // Search by Phone Number (exact match)
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                customersQuery = customersQuery.Where(c => c.PhoneNumber.Contains(phoneNumber));
            }

            // Filter by Status (Active/Inactive)
            if (!string.IsNullOrEmpty(status))
            {
                customersQuery = customersQuery.Where(c => c.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            }

            var customers = customersQuery.Select(c => new
            {
                c.CustomerId,
                c.FullName,
                c.PhoneNumber,
                c.Email,
                c.CreatedAt,
                c.Gender,
                c.IsMember,
                c.LastVisitDate,
                c.AmountSpent,
                c.Status,
                c.LoyaltyPoints,
                c.MembershipId
            }).ToList();

            return Ok(customers);
        }

        // GET /api/customers/{id}
        [HttpGet]
        [Route("api/customers/{id}")]
        public IHttpActionResult GetCustomerById(Guid id)
        {
            var customer = db.Customers
                             .Where(c => c.CustomerId == id)
                             .Select(c => new
                             {
                                 c.CustomerId,
                                 c.FullName,
                                 c.PhoneNumber,
                                 c.Email,
                                 c.CreatedAt,
                                 c.Gender,
                                 c.IsMember,
                                 c.LastVisitDate,
                                 c.AmountSpent,
                                 c.Status,
                                 c.LoyaltyPoints,
                                 c.MembershipId
                             })
                             .FirstOrDefault();

            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        // POST /api/customers
        [HttpPost]
        [Route("api/customers")]
        public IHttpActionResult AddCustomer([FromBody] CustomerDto customerDto)
        {
            if (customerDto == null)
                return BadRequest("Customer data cannot be empty.");
            var existing = db.Customers.FirstOrDefault(x => x.PhoneNumber == customerDto.PhoneNumber);
            if (existing != null)
            {
                return BadRequest("Phone number already exists!");
            }

            //Guid currentSaloonId = new Guid("00000000-0000-0000-0000-000000000021");

            //var CurrentSaloonId = currentSaloonId;  // NEW - fetch current salon
           
            var newCustomer = new Customer
            {
                CustomerId = Guid.NewGuid(),
                FullName = customerDto.FullName,
                PhoneNumber = customerDto.PhoneNumber,
                Email = customerDto.Email,
                Gender = customerDto.Gender,
                IsMember = customerDto.IsMember,
                LastVisitDate = customerDto.LastVisitDate,
                AmountSpent = customerDto.AmountSpent,
                Status = customerDto.Status ?? "Active",  // If empty, default Active
                LoyaltyPoints = 0,
                MembershipId = customerDto.MembershipId,
                //SalonId = currentSaloonId   // REQUIRED FIELD!
            };

            db.Customers.Add(newCustomer);
            db.SaveChanges();

            return Ok(new { Message = "Customer added successfully." });
        }


        // PUT /api/customers/{id}
        [HttpPut]
        [Route("api/customers/{id}")]
        public IHttpActionResult UpdateCustomer(Guid id, [FromBody] CustomerDto customerDto)
        {
            var customer = db.Customers.FirstOrDefault(c => c.CustomerId == id);
            if (customer == null)
                return NotFound();

            customer.FullName = customerDto.FullName;
            customer.PhoneNumber = customerDto.PhoneNumber;
            customer.Email = customerDto.Email;
            customer.Gender = customerDto.Gender;
            customer.IsMember = customerDto.IsMember;
            customer.LastVisitDate = customerDto.LastVisitDate;
            customer.AmountSpent = customerDto.AmountSpent;
            customer.Status = customerDto.Status;
            customer.MembershipId = customerDto.MembershipId;

            db.SaveChanges();

            return Ok(new { Message = "Customer updated successfully." });
        }

        // DELETE /api/customers/{id}
        [HttpDelete]
        [Route("api/customers/{id}")]
        public IHttpActionResult DeleteCustomer(Guid id)
        {
            var customer = db.Customers.FirstOrDefault(c => c.CustomerId == id);
            if (customer == null)
                return NotFound();

            // Optionally, mark customer as inactive
            customer.Status = "Inactive";  // Change status to Inactive instead of deleting

            db.SaveChanges();

            return Ok(new { Message = "Customer deactivated successfully." });
        }
    }
}
