using System;
using System.Collections.Generic;

public class SalonOwnerDto
{
    // 1) Rename “Email” → “EmailAddress”
    public string EmailAddress { get; set; }

    // 2) Rename “PasswordHash” → “Password”
    //    We will hash it on the server side → do not expect the client to send a SHA-256 hash.
    public string Password { get; set; }

    // 3) Profile fields
    public string FullName { get; set; }          // JS sends “fullName”
    public string PhoneNumber { get; set; }       // JS sends “phoneNumber”
    public string Gender { get; set; }            // JS sends “gender”
    public DateTime DateOfBirth { get; set; }     // JS sends “dateOfBirth” (yyyy-MM-dd)


    // 4) Salon Information
    public string SalonName { get; set; }            // JS sends “salonName”
    public string SalonType { get; set; }            // JS sends “salonType”
    public string GSTNumber { get; set; }            // JS sends “gstNumber”

    // 5) ServicesOffered: JS sends an array of strings → we can store as comma-separated
    public string[] ServicesOffered { get; set; }

    // 6) Address fields (rename to match JS keys)
    public string StreetAddress { get; set; }        // JS sends “streetAddress”
    public string Country { get; set; }              // JS sends “country”
    public string State { get; set; }                // JS sends “state”
    public string City { get; set; }                 // JS sends “city”
    public string PinCode { get; set; }              // JS sends “pinCode”

    // 7) Operating hours
    public TimeSpan OpeningTime { get; set; }        // JS sends “openingTime” as "HH:mm"
    public TimeSpan ClosingTime { get; set; }        // JS sends “closingTime” as "HH:mm"

    // 8) TermsAccepted (JS sends boolean)
    public bool TermsAccepted { get; set; }

  
}
public class SalonDto
{
    public Guid SalonId { get; set; }
    public Guid OwnerId { get; set; }            // Refers to the SalonOwner
    public string SalonName { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}

public class BranchDto
{
    public Guid BranchId { get; set; }
    public Guid SalonId { get; set; }             // Refers to the Salon
    public string BranchName { get; set; }
    public string Address { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}

public class SalonOwnerWithBranchesDto
{
    public string FullName { get; set; }
    public string SalonName { get; set; }
    public string PhoneNumber { get; set; }
    public string EmailAddress { get; set; }
    public List<SalonDto> Salons { get; set; }      // A list of salons
    public List<BranchDto> Branches { get; set; }   // A list of branches
}

