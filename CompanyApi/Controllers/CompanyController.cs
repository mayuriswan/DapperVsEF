using CompanyApi.Data;
using CompanyApi.Models;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CompanyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly DataBaseContext _dbContext ;

        public CompanyController(DataBaseContext dbContext)
        {
            _dbContext = dbContext ;
        }

        [HttpPut("/increase-salaries")]
        public async Task<IActionResult> IncreaseSalaries(int comapnyId)
        {
            var company = await _dbContext
                                          .Set<Company>()
                                          .Include(c=>c.Employees)
                                          .FirstOrDefaultAsync(c=>c.Id==comapnyId);
            if(company == null)
            {
                return  NotFound($"this comapny with Id {comapnyId} doesnt exist ");
            }
            foreach(var employee in company.Employees)
            {
                employee.Salary += employee.Salary * 0.1m;
            }
            await _dbContext.Database.BeginTransactionAsync();
            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"Update Employees Set SALARY = Salary*1.1 WHERE CompanyId = {comapnyId}");
            company.LastSalaryUpdate= DateTime.Now;
            await _dbContext.SaveChangesAsync();
            await _dbContext.Database.CommitTransactionAsync();
            return  NoContent();

        }
        [HttpPut("/increase-salaries-dapper")]
        public async Task<IActionResult> IncreaseSalariesDapper(int comapnyId)
        {
            var company = await _dbContext
                                          .Set<Company>()
                                          .Include(c => c.Employees)
                                          .FirstOrDefaultAsync(c => c.Id == comapnyId);
            if (company == null)
            {
                return NotFound($"this comapny with Id {comapnyId} doesnt exist ");
            }
            foreach (var employee in company.Employees)
            {
                employee.Salary += employee.Salary * 0.1m;
            }
            var transaction = await _dbContext.Database.BeginTransactionAsync();
            await _dbContext.Database.GetDbConnection().ExecuteAsync(
                "Update Employees Set SALARY = Salary*1.1 WHERE CompanyId = @companyId",
                new {companyId = company.Id},
                transaction.GetDbTransaction()
                );
            company.LastSalaryUpdate = DateTime.Now;
            await _dbContext.SaveChangesAsync();
            await _dbContext.Database.CommitTransactionAsync();
            return NoContent();

        }

    }
}
