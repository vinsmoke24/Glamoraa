using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.Models.DTOs;
using WebApplication1.Models;
using WebApplication1.Models.Dtos;


namespace WebApplication1.Controllers
{
    [RoutePrefix("api/coupons")]
    public class CouponsController : ApiController
    {
        private readonly GlamoraaEntities db = new GlamoraaEntities();

        // GET: api/coupons
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAllCoupons()
        {
            var coupons = db.Coupons
                .Where(c => c.IsActive)
                .Select(c => new CouponDto
                {
                    CouponId = c.CouponId,
                    SalonId = c.SalonId,
                    Code = c.Code,
                    Description = c.Description,
                    DiscountAmount = c.DiscountAmount.HasValue ? c.DiscountAmount.Value : 0m,
                    IsPercentage = c.IsPercentage,
                    IsActive = c.IsActive
                })
                .ToList();

            if (!coupons.Any())
            {
                return NotFound();
            }

            return Ok(coupons);
        }

        // GET: api/coupons/{id}
        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult GetCouponById(int id)
        {
            var coupon = db.Coupons
                .Where(c => c.CouponId == id && c.IsActive)
                .Select(c => new CouponDto
                {
                    CouponId = c.CouponId,
                    SalonId = c.SalonId,
                    Code = c.Code,
                    Description = c.Description,
                    DiscountAmount = (decimal)c.DiscountAmount,
                    IsPercentage = c.IsPercentage,
                    IsActive = c.IsActive
                })
                .FirstOrDefault();

            if (coupon == null)
            {
                return NotFound();
            }

            return Ok(coupon);
        }

        // POST: api/coupons
        [HttpPost]
        [Route("")]
        public IHttpActionResult CreateCoupon([FromBody] CouponDto couponDto)
        {
            if (couponDto == null)
            {
                return BadRequest("Invalid coupon data.");
            }

            Guid currentSaloonId = new Guid("00000000-0000-0000-0000-000000000021");
            var CurrentSaloonId = currentSaloonId;
            var coupon = new Coupon
            {
                Code = couponDto.Code,
                Description = couponDto.Description,
                DiscountAmount = couponDto.DiscountAmount,
                IsPercentage = couponDto.IsPercentage,
                IsActive = true,  // Ensure new coupons are active by default
                                  // CreatedAt = DateTime.UtcNow
                SalonId = currentSaloonId
            };

            db.Coupons.Add(coupon);
            db.SaveChanges();

            return CreatedAtRoute("GetCouponById", new { id = coupon.CouponId }, couponDto);
        }

        // PUT: api/coupons/{id}
        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult UpdateCoupon(int id, [FromBody] CouponDto updatedCouponDto)
        {
            var coupon = db.Coupons.FirstOrDefault(c => c.CouponId == id);
            if (coupon == null)
            {
                return NotFound();
            }

            coupon.Code = updatedCouponDto.Code;
            coupon.Description = updatedCouponDto.Description;
            coupon.DiscountAmount = updatedCouponDto.DiscountAmount;
            coupon.IsPercentage = updatedCouponDto.IsPercentage;
            coupon.IsActive = updatedCouponDto.IsActive;
         //   coupon.UpdatedAt = DateTime.UtcNow;

            db.SaveChanges();
            return Ok(updatedCouponDto);
        }

        // DELETE: api/coupons/{id}
        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult DeleteCoupon(int id)
        {
            var coupon = db.Coupons.FirstOrDefault(c => c.CouponId == id);
            if (coupon == null)
            {
                return NotFound();
            }

            coupon.IsActive = false;  // Instead of deleting, we deactivate the coupon
            db.SaveChanges();
            return Ok(new { message = "Coupon deactivated successfully", couponId = coupon.CouponId });
        }

        // API to validate coupon during checkout
        [HttpGet]
        [Route("validate/{code}")]
        public IHttpActionResult ValidateCoupon(string code)
        {
            var coupon = db.Coupons
                .Where(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase) && c.IsActive)
                .Select(c => new CouponDto
                {
                    CouponId = c.CouponId,
                    SalonId = c.SalonId,
                    Code = c.Code,
                    Description = c.Description,
                    DiscountAmount = (decimal)c.DiscountAmount,
                    IsPercentage = c.IsPercentage,
                    IsActive = c.IsActive
                })
                .FirstOrDefault();

            if (coupon == null)
            {
                return BadRequest("Invalid or expired coupon.");
            }
            return Ok(coupon);
        }
    }
}
