using CompanyApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyApi.Data
{
    public class DataBaseContext : DbContext
    {
        public DataBaseContext(DbContextOptions options):base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>(builder =>
            {
                builder.ToTable("Companies");
                builder
                       .HasMany(company => company.Employees)
                       .WithOne()
                       .HasForeignKey(employee => employee.CompanyId)
                       .IsRequired();
                builder
                    .HasData(new Company
                    {
                        Id = 1,
                        Name = "Mayuri",

                    });
                

            });
            modelBuilder.Entity<Employee>(builder =>
            {
                builder.ToTable("Employees");
                var employees = Enumerable
                                    .Range(1, 1000)
                                    .Select(id => new Employee
                                    {
                                        Id = id,
                                        Name = $"Employee #{id}",
                                        CompanyId = 1,
                                        Salary=100

                                    });
                builder.HasData(employees);
            });
        }
    }
}
