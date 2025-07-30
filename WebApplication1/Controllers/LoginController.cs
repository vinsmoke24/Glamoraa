using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Models;
using WebApplication1.Models.DTOs;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;


namespace Glamora_Saloon.Controllers
{
    [RoutePrefix("api/login")]
    public class LoginController : ApiController
    {
        private readonly GlamoraaEntities db = new GlamoraaEntities();

        // 🔐 Password Hasher
        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password.Trim());
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        // 🔑 Token Generator
        private string GenerateJwtToken(string id, string name, string role, string salonId, string branchId)
        {
            var key = Encoding.ASCII.GetBytes(ConfigurationManager.AppSettings["JwtSecret"]);
            var tokenHandler = new JwtSecurityTokenHandler();

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, id),
            new Claim(ClaimTypes.Name, name),
            new Claim(ClaimTypes.Role, role),
            new Claim("SalonId", salonId ?? ""),
            new Claim("BranchId", branchId ?? "")
        }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(descriptor);
            return tokenHandler.WriteToken(token);
        }


        // ✅ Admin Login
        [HttpPost]
        [Route("admin")]
        public HttpResponseMessage AdminLogin(LoginDto login)
        {
            if (string.IsNullOrWhiteSpace(login.Username) || string.IsNullOrWhiteSpace(login.Password))
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Username and Password are required.");

            string inputHash = HashPassword(login.Password.Trim());
            string inputUsername = login.Username.Trim();

            var user = db.Users.FirstOrDefault(u =>
                u.Username == inputUsername &&
                u.PasswordHash == inputHash
            );

            if (user == null)
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid credentials");

            var roleName = db.Roles.FirstOrDefault(r => r.RoleId == user.RoleId)?.RoleName ?? "Admin";

            var owner = db.SalonOwners.FirstOrDefault(o => o.UserId == user.UserId);

            var salon = db.Salons.FirstOrDefault(s => s.OwnerId == owner.OwnerId);
            var salonId = salon?.SalonId.ToString() ?? "";

            var branchId = db.Branches.FirstOrDefault(b => b.SalonId == salon.SalonId)?.BranchId.ToString() ?? "";

            var token = GenerateJwtToken(user.UserId.ToString(), user.Username, roleName, salonId, branchId);


            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Token = token,
                Role = roleName,
                Username = user.Username,
                SalonId = salonId,
                BranchId = branchId
            });
        }

        // ✅ Staff Login
        [HttpPost]
        [Route("staff")]
        public HttpResponseMessage StaffLogin(LoginDto login)
        {
            if (string.IsNullOrWhiteSpace(login.Username) || string.IsNullOrWhiteSpace(login.Password))
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Username and Password are required.");

            string inputHash = HashPassword(login.Password.Trim());
            string inputEmail = login.Username.Trim().ToLower();

            var staff = db.Staffs.FirstOrDefault(s =>
                s.Email.Trim().ToLower() == inputEmail &&
                s.PasswordHash.Trim() == inputHash
            );

            if (staff == null)
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid credentials");

            if (!staff.IsActive.GetValueOrDefault())
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Account is not active");

            var salonId = staff.SalonId.ToString();
            var branchId = staff.BranchId?.ToString() ?? "";

            var token = GenerateJwtToken(staff.StaffId.ToString(), staff.FullName, "Staff", salonId, branchId);


            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Token = token,
                Role = "Staff",
                Name = staff.FullName,
                StaffId = staff.StaffId,
                SalonId = salonId,
                BranchId = branchId
            });
        }


        // ✅ Customer Login
        [HttpPost]
        [Route("customer")]
        public HttpResponseMessage CustomerLogin(LoginDto login)
        {
            if (string.IsNullOrWhiteSpace(login.Username) || string.IsNullOrWhiteSpace(login.Password))
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Username and Password are required.");

            string hash = HashPassword(login.Password);

            var cust = db.Customers.FirstOrDefault(c => c.Email == login.Username && c.PasswordHash == hash);
            if (cust == null)
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid credentials");

            if (cust.Status != "Active")
                return Request.CreateResponse(HttpStatusCode.Forbidden, "Account is not active");

            var salonId = cust.SalonId.ToString();
            var branchId = cust.BranchId?.ToString() ?? "";

            var token = GenerateJwtToken(cust.CustomerId.ToString(), cust.FullName, "Customer", salonId, branchId);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Token = token,
                Role = "Customer",
                Name = cust.FullName,
                CustomerId = cust.CustomerId,
                SalonId = salonId,
                BranchId = branchId
            });


            
        }

        // ✅ Staff Registration
        [HttpPost]
        [Route("register/staff")]
        public HttpResponseMessage RegisterStaff(StaffRegisterDto staffDto)
        {
            if (db.Staffs.Any(s => s.Email == staffDto.Email))
                return Request.CreateResponse(HttpStatusCode.Conflict, "Email already exists");

            var staff = new Staff
            {
                StaffId = Guid.NewGuid(),
                FullName = staffDto.FullName,
                Email = staffDto.Email,
                PhoneNumber = staffDto.PhoneNumber,
                PasswordHash = HashPassword(staffDto.Password),
                IsActive = true,
                DateOfJoining = DateTime.Now
                // Set BranchId or default values as needed
            };

            db.Staffs.Add(staff);
            db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.Created, "Staff registered");
        }

        // ✅ Customer Registration
        [HttpPost]
        [Route("register/customer")]
        public HttpResponseMessage RegisterCustomer(CustomerRegisterDto customerDto)
        {
            // 1) Parse your “desired” SalonId (a constant or from config/DTO)
            var salonGuid = new Guid("00000000-0000-0000-0000-000000000021");

            // 2) Check if that Salon already exists in the database.
            var salon = db.Salons.SingleOrDefault(s => s.SalonId == salonGuid);

            if (salon == null)
            {
                // If it does NOT exist, create it now (so the FK will succeed).
                salon = new Salon
                {
                    SalonId = salonGuid,
                    OwnerId = new Guid("00000000 - 0000 - 0000 - 0000 - 000000000011"),
                    SalonName = "Default Glamora Salon",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                    // … populate any other required columns …
                };
                db.Salons.Add(salon);
                // Note: You could call SaveChanges here, or wait until after adding the customer
                // as long as both new Salon and new Customer are saved together. We'll call SaveChanges once at the end.
            }

            // 3) Build your new Customer, pointing to that exact salonGuid:
            var customer = new Customer
            {
                CustomerId = Guid.NewGuid(),
                FullName = customerDto.FullName,
                PhoneNumber = customerDto.PhoneNumber,
                Email = customerDto.Email,
                Gender = customerDto.Gender,
                PasswordHash = HashPassword(customerDto.Password),
                CreatedAt = DateTime.UtcNow,
                Status = "Active",
                IsMember = false,
                AmountSpent = 0,

                SalonId = salonGuid
            };

            // 4) Add the customer to EF context
            db.Customers.Add(customer);

            try
            {
                // 5) Save both changes (new Salon if needed + new Customer) in one transaction.
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                // Something else went wrong—return a 500 or inspect ex.InnerException for details
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.Created, "Customer registered successfully");
        }

        //salonowner registration
        // ✅ Salon Owner Registration
        [HttpPost]
        [Route("register/owner")]
        public HttpResponseMessage RegisterSalonOwner([FromBody] SalonOwnerDto dto)
        {
            // 1) dto itself must not be null
            if (dto == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Request body cannot be empty.");
            }

            // 2) Validate using dto.Password (plain text), NOT dto.PasswordHash!
            if (string.IsNullOrWhiteSpace(dto.EmailAddress)
                || string.IsNullOrWhiteSpace(dto.Password))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Email and Password are required.");
            }

            // 3) Normalize email for duplicate checks
            var normalizedEmail = dto.EmailAddress.Trim().ToLower();

            if (db.Users.Any(u => u.Username.ToLower() == normalizedEmail))
                return Request.CreateResponse(HttpStatusCode.Conflict,
                    "Username or Email already exists.");

            if (db.SalonOwners.Any(o => o.Email.ToLower() == normalizedEmail))
                return Request.CreateResponse(HttpStatusCode.Conflict,
                    "Salon owner with this email already exists.");

            // 4) Hash the PLAINTEXT password from dto.Password
            var hashedPassword = HashPassword(dto.Password.Trim());

            // 5) Create the User record, assigning PasswordHash = hashedPassword
            var newUser = new User
            {
                UserId = Guid.NewGuid(),
                Username = dto.EmailAddress.Trim(),
                PasswordHash = hashedPassword,    // ← hashed dto.Password
                RoleId = db.Roles.FirstOrDefault(r => r.RoleName == "Admin")?.RoleId ?? 0,
                LastLogin = DateTime.UtcNow
            };
            db.Users.Add(newUser);

            // 6) Convert the servicesOffered string[] into a comma‐separated string
            string servicesCsv = "";
            if (dto.ServicesOffered != null && dto.ServicesOffered.Length > 0)
            {
                servicesCsv = string.Join(",", dto.ServicesOffered);
            }

            // 7) Create the SalonOwner entity, copying hashedPassword into its PasswordHash
            var owner = new SalonOwner
            {
                OwnerId = Guid.NewGuid(),
                UserId = newUser.UserId,
                FullName = dto.FullName?.Trim(),
                Email = dto.EmailAddress.Trim(),
                PhoneNumber = dto.PhoneNumber?.Trim(),
                Gender = dto.Gender?.Trim(),
                DateOfBirth = dto.DateOfBirth,
                SalonName = dto.SalonName?.Trim(),
                SalonType = dto.SalonType?.Trim(),
                GSTNumber = dto.GSTNumber?.Trim(),
                ServicesOffered = servicesCsv,
                StreetAddress = dto.StreetAddress?.Trim(),
                City = dto.City?.Trim(),
                State = dto.State?.Trim(),
                Country = dto.Country?.Trim(),
                PinCode = dto.PinCode?.Trim(),
                OpeningTime = dto.OpeningTime,
                ClosingTime = dto.ClosingTime,
                TermsAccepted = dto.TermsAccepted,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                PasswordHash = hashedPassword
            };
            db.SalonOwners.Add(owner);

            try
            {
                db.SaveChanges();

                // 8) Create a default Salon record for the new owner
                var salon = new Salon
                {
                    SalonId = Guid.NewGuid(),
                    OwnerId = owner.OwnerId,  // Linking to the newly created salon owner
                    SalonName = owner.SalonName, // Set the salon name from the owner DTO
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };
                db.Salons.Add(salon);

                // 9) Create a default Branch record for the new salon
                var branch = new Branch
                {
                    BranchId = Guid.NewGuid(),
                    SalonId = salon.SalonId,   // Linking to the newly created salon
                    BranchName = "Main Branch", // Default branch name
                    Address = owner.StreetAddress ?? "Default Address",  // Default address
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };
                db.Branches.Add(branch);

                // 10) Save both Salon and Branch after owner registration is successful
                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.Created,
                    "Salon owner, salon, and branch registered successfully.");
            }
            catch (DbEntityValidationException ex)
            {
                // Collect EF validation errors (e.g. missing required fields)
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);
                var fullErrorMessage = string.Join("; ", errorMessages);
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Validation failed: " + fullErrorMessage);
            }
            catch (Exception ex)
            {
                var baseException = ex.GetBaseException();  // get the root cause exception
                string errorMsg = baseException.Message;

                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    "Registration failed: " + errorMsg);
            }
        }




    }
}
