using Insurance.Api.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Insurance.Api.Data
{
    public class InsuranceDbContext : DbContext
    {
        public DbSet<ProductTypeSurchargeRule> ProductTypeSurchargeRules { get; set; }

        public InsuranceDbContext(DbContextOptions<InsuranceDbContext> options) : base(options)
        {
            
        }
    }


}
