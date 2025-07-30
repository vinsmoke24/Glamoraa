using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.Models.DTOs;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [RoutePrefix("api/users")]


    public class UserController : ApiController
    {
        private readonly GlamoraaEntities db = new GlamoraaEntities();

        [HttpGet]
        public IHttpActionResult GetAllUsers()
        {
            var users = db.Users.Select(u => new
            {
                u.UserId,
                u.Username,
                RoleName = db.Roles.FirstOrDefault(r => r.RoleId == u.RoleId).RoleName,
                u.SalonId,
                u.BranchId,
                u.LastLogin
            }).ToList();

            return Ok(users);
        }

        [HttpPost]
        public IHttpActionResult AddUser([FromBody] Userdto userDto)
        {
            if (userDto == null)
                return BadRequest("User data cannot be empty.");

            var role = db.Roles.FirstOrDefault(r => r.RoleName == userDto.Role);
            if (role == null)
                return BadRequest("Invalid role specified.");

            var newUser = new User
            {
                Username = userDto.Username,
                PasswordHash = HashPassword(userDto.Password),
                RoleId = role.RoleId,
                SalonId = Guid.NewGuid(),
                BranchId = Guid.NewGuid(),
                LastLogin = DateTime.Now
            };

            db.Users.Add(newUser);
            db.SaveChanges();

            return Ok(new { Message = "User added successfully." });
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult UpdateUser(Guid id, [FromBody] Userdto userDto)
        {
            var user = db.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null)
                return NotFound();

            var role = db.Roles.FirstOrDefault(r => r.RoleName == userDto.Role);
            if (role == null)
                return BadRequest("Invalid role specified.");

            user.Username = userDto.Username;
            if (!string.IsNullOrEmpty(userDto.Password))
            {
                user.PasswordHash = HashPassword(userDto.Password);
            }
            user.RoleId = role.RoleId;

            db.SaveChanges();

            return Ok(new { Message = "User updated successfully." });
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeactivateUser(Guid id)
        {
            var user = db.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null)
                return NotFound();

            user.IsActive = false;
            db.SaveChanges();

            return Ok(new { Message = "User deactivated successfully." });
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }

}
