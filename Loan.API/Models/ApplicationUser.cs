using Microsoft.AspNetCore.Identity;

namespace Loan.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? Age { get; set; }
        public decimal? Salary { get; set; }
        public bool? IsBlocked { get; set; }
    }
}
