using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebApplication1.Models;
using WebApplication1.Models.Dtos;

namespace WebApplication1.Controllers
{
    [RoutePrefix("api/invoices")]
    public class InvoiceController : ApiController
    {
        private readonly GlamoraaEntities db = new GlamoraaEntities();

        // GET: api/invoices/services - Get all services for dropdown
        [HttpGet, Route("services")]
        public IHttpActionResult GetServices()
        {
            try
            {
                var services = db.Services
                    .Select(s => new
                    {
                        ServiceId = s.ServiceId,
                        ServiceName = s.ServiceName,
                       // Price = s.Price,
                      //  CategoryId = s.CategoryId,
                      //  CategoryName = s.Category != null ? s.Category.CategoryName : "",
                        SubServices = s.SubServices
                            .Select(ss => new
                            {
                                SubServiceId = ss.SubServiceId,
                                SubServiceName = ss.SubServiceName,
                                Price = ss.Price
                            }).ToList()
                    })
                    .ToList();

                return Ok(services);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/invoices/customer/{phone} - Get customer details by phone
        [HttpGet, Route("customer/{phone}")]
        public IHttpActionResult GetCustomerByPhone(string phone)
        {
            try
            {
                var customer = db.Customers
                    .Where(c => c.PhoneNumber == phone)
                    .Select(c => new
                    {
                        CustomerId = c.CustomerId,
                        FullName = c.FullName,
                        Phone = c.PhoneNumber,
                        Email = c.Email,
                        // Get recent appointments
                        Appointments = c.Appointments
                            .Where(a => a.Status == "Confirmed" || a.Status == "Completed")
                            .OrderByDescending(a => a.BookingDate)
                            .Take(5)
                            .Select(a => new
                            {
                                AppointmentId = a.AppointmentId,
                                AppointmentDate = a.BookingDate,
                                Status = a.Status,
                                Services = a.AppointmentServices.Select(aps => new
                                {
                                    AppointmentServiceId = aps.Id,
                                    ServiceId = aps.ServiceId,
                                    ServiceName = aps.Service.ServiceName,
                                    PriceAtBooking = aps.PriceAtBooking
                                }).ToList()
                            }).ToList()
                    })
                    .FirstOrDefault();

                if (customer == null)
                {
                    return NotFound();
                }

                return Ok(customer);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/invoices/products - Get all products for invoice
        [HttpGet, Route("products")]
        public IHttpActionResult GetProducts()
        {
            try
            {
                var products = db.Products
                    .Select(p => new
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        Price = p.Price,
                      //  Category = p.Category == null ? "" : p.Category.CategoryName,
                        StockQuantity = p.ProductStocks.Sum(ps => ps.Quantity),
                       // GstRate = p.GstRate
                    })
                    .ToList();

                return Ok(products);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/invoices/staff - Get all staff members
        [HttpGet, Route("staff")]
        public IHttpActionResult GetStaff()
        {
            try
            {
                var staff = db.Staffs
                    .Select(s => new
                    {
                        s.StaffId,
                        s.FullName,
                        s.Specializations
                    })
                    .ToList();

                return Ok(staff);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/invoices/next-number - Get next invoice number
        [HttpGet, Route("next-number")]
        public IHttpActionResult GetNextInvoiceNumber()
        {
            try
            {
                var lastInvoiceCount = db.Invoices.Count();
                var nextNumber = lastInvoiceCount + 1;
                var invoiceNumber = $"INV-{nextNumber:D3}";

                return Ok(new { InvoiceNumber = invoiceNumber });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/invoices
        [HttpPost, Route("")]
        public IHttpActionResult CreateInvoice([FromBody] CreateInvoiceDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // 1) Calculate subtotals
                var svcSub = dto.Services.Sum(s => s.PriceAtBooking);
                var prodSub = dto.Products.Sum(p => p.UnitPrice * p.Quantity);
                var subtotal = svcSub + prodSub;

                // 2) Apply coupon
                decimal discount = 0m;
                var coupon = !string.IsNullOrEmpty(dto.CouponCode)
                    ? db.Coupons.FirstOrDefault(c => c.Code == dto.CouponCode && c.IsActive)
                    : null;
                if (coupon != null)
                {
                    discount = (decimal)(coupon.IsPercentage
                        ? subtotal * coupon.DiscountAmount / 100m
                        : coupon.DiscountAmount);
                }

                // 3) Calculate GST total by summing each line's tax
                var taxServices = dto.Services.Sum(s => s.PriceAtBooking * s.GstRatePercent / 100m);
                var taxProducts = dto.Products.Sum(p => p.UnitPrice * p.Quantity * p.GstRatePercent / 100m);
                var tax = taxServices + taxProducts;

                // 4) Calculate final total
                var total = (subtotal - discount + tax) - dto.TokenPaid;

                // 5) Generate invoice number
                var invoiceCount = db.Invoices.Count();
                var nextNumber = invoiceCount + 1;
                var invoiceNumber = $"INV-{nextNumber:D3}";

                Guid currentSaloonId = new Guid("00000000-0000-0000-0000-000000000021");
                var CurrentSaloonId = currentSaloonId;
                Guid currentBranchId = new Guid("00000000-0000-0000-0000-000000000031");
                var CurrentBranchId = currentBranchId;
                // 6) Create invoice header
                var inv = new Invoice
                {
                    InvoiceId = Guid.NewGuid(),
                    CustomerId = dto.CustomerId,
                    AppointmentId = dto.AppointmentId,
                    InvoiceDate = DateTime.Now,
                    Subtotal = subtotal,
                    Discount = discount,
                    TaxAmount = tax,
                    TotalAmount = total,
                    PaymentStatus = "Pending",
                    CreatedBy = dto.CreatedBy,
                    SalonId = CurrentSaloonId,
                    BranchId = CurrentBranchId
                };
                db.Invoices.Add(inv);

                // 7) Insert service lines
                foreach (var s in dto.Services)
                {
                    db.InvoiceServices.Add(new InvoiceService
                    {
                        InvoiceId = inv.InvoiceId,
                        AppointmentServiceId = s.AppointmentServiceId,
                        StaffId = s.StaffId,
                        PriceAtBooking = s.PriceAtBooking,
                        GstRatePercent = s.GstRatePercent
                    });
                }

                // 8) Insert product lines and deduct stock
                foreach (var p in dto.Products)
                {
                    db.InvoiceProducts.Add(new InvoiceProduct
                    {
                        InvoiceId = inv.InvoiceId,
                        ProductId = p.ProductId,
                        StaffId = p.StaffId,
                        Quantity = p.Quantity,
                        UnitPrice = p.UnitPrice,
                        GstRatePercent = p.GstRatePercent
                    });

                    // stock deduction
                    db.ProductStocks.Add(new ProductStock
                    {
                        ProductId = p.ProductId,
                        Quantity = -p.Quantity,
                        Operation = "Sale",
                        ReferenceId = inv.InvoiceId
                    });
                }

                // 9) Record coupon usage
                if (coupon != null && discount > 0)
                {
                    db.CouponUsages.Add(new CouponUsage
                    {
                        CouponId = coupon.CouponId,
                        InvoiceId = inv.InvoiceId,
                        CustomerId = dto.CustomerId,
                        UsedOn = DateTime.Now,
                        DiscountAmount = discount
                    });
                }

                db.SaveChanges();

                // 10) Prepare detailed response
                var details = new InvoiceDetailsDto
                {
                    InvoiceId = inv.InvoiceId,
                    InvoiceDate = (DateTime)inv.InvoiceDate,
                    SubTotal = (decimal)inv.Subtotal,
                    Discount = (decimal)inv.Discount,
                    TaxAmount = (decimal)inv.TaxAmount,
                    TotalAmount = (decimal)inv.TotalAmount,
                    CouponCode = coupon?.Code,

                    // join to pull service names
                    ServiceLines = (from i in db.InvoiceServices
                                    join a in db.AppointmentServices on i.AppointmentServiceId equals a.Id
                                    join svc in db.Services on a.ServiceId equals svc.ServiceId
                                    where i.InvoiceId == inv.InvoiceId
                                    select new InvoiceServiceLineDto
                                    {
                                        AppointmentServiceId = i.AppointmentServiceId,
                                        ServiceName = svc.ServiceName,
                                        StaffId = i.StaffId,
                                        PriceAtBooking = i.PriceAtBooking,
                                        GstRatePercent = i.GstRatePercent
                                    }).ToList(),

                    // join to pull product names
                    ProductLines = (from ip in db.InvoiceProducts
                                    join p in db.Products on ip.ProductId equals p.ProductId
                                    where ip.InvoiceId == inv.InvoiceId
                                    select new InvoiceProductLineDto
                                    {
                                        ProductId = p.ProductId,
                                        ProductName = p.Name,
                                        StaffId = ip.StaffId,
                                        Quantity = ip.Quantity,
                                        UnitPrice = ip.UnitPrice,
                                        GstRatePercent = ip.GstRatePercent
                                    }).ToList()
                };

                return Ok(details);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/invoices - Get all invoices for previous invoices page
        [HttpGet, Route("")]
        public IHttpActionResult GetAllInvoices()
        {
            try
            {
                var invoices = db.Invoices
                    .OrderByDescending(i => i.InvoiceDate)
                    .Select(i => new
                    {
                        InvoiceId = i.InvoiceId,
                        InvoiceDate = i.InvoiceDate,
                        CustomerName = i.Customer.FullName,
                        CustomerPhone = i.Customer.PhoneNumber,
                        TotalAmount = i.TotalAmount,
                        PaymentStatus = i.PaymentStatus,
                        Services = i.InvoiceServices.Select(ins => ins.AppointmentService.Service.ServiceName).ToList()
                    })
                    .ToList();

                return Ok(invoices);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/invoices/{id}
        [HttpGet, Route("{id:guid}")]
        public IHttpActionResult GetInvoice(Guid id)
        {
            try
            {
                var inv = db.Invoices.Find(id);
                if (inv == null) return NotFound();

                var details = new InvoiceDetailsDto
                {
                    InvoiceId = inv.InvoiceId,
                    InvoiceDate = (DateTime)inv.InvoiceDate,
                    SubTotal = (decimal)inv.Subtotal,
                    Discount = (decimal)inv.Discount,
                    TaxAmount = (decimal)inv.TaxAmount,
                    TotalAmount = (decimal)inv.TotalAmount,

                    ServiceLines = (from i in db.InvoiceServices
                                    join a in db.AppointmentServices on i.AppointmentServiceId equals a.Id
                                    join svc in db.Services on a.ServiceId equals svc.ServiceId
                                    where i.InvoiceId == inv.InvoiceId
                                    select new InvoiceServiceLineDto
                                    {
                                        AppointmentServiceId = i.AppointmentServiceId,
                                        ServiceName = svc.ServiceName,
                                        StaffId = i.StaffId,
                                        PriceAtBooking = i.PriceAtBooking,
                                        GstRatePercent = i.GstRatePercent
                                    }).ToList(),

                    ProductLines = (from ip in db.InvoiceProducts
                                    join p in db.Products on ip.ProductId equals p.ProductId
                                    where ip.InvoiceId == inv.InvoiceId
                                    select new InvoiceProductLineDto
                                    {
                                        ProductId = p.ProductId,
                                        ProductName = p.Name,
                                        StaffId = ip.StaffId,
                                        Quantity = ip.Quantity,
                                        UnitPrice = ip.UnitPrice,
                                        GstRatePercent = ip.GstRatePercent
                                    }).ToList()
                };

                return Ok(details);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}