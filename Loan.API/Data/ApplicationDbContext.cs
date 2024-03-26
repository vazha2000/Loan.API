using Loan.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Loan.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<LoanModel> Loans { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .Property(x => x.Salary)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<LoanModel>()
                .Property(x => x.Amount)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Name = "Accountant", ConcurrencyStamp = "1", NormalizedName = "ACCOUNTANT" },
                new IdentityRole { Name = "User", ConcurrencyStamp = "2", NormalizedName = "USER" }
                );
        }
    }
}
