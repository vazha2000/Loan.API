namespace Loan.API.Models.DTOs
{
    public class UserRegisterDto : RegisterDto
    {
        public int Age { get; set; }
        public decimal Salary { get; set; }
    }
}
