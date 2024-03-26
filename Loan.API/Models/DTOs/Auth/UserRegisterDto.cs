namespace Loan.API.Models.DTOs.Auth
{
    public class UserRegisterDto : RegisterDto
    {
        public int Age { get; set; }
        public decimal Salary { get; set; }
    }
}
