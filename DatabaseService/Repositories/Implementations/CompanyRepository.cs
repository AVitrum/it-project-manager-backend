using DatabaseService.Data.Models;
using DatabaseService.Exceptions;
using DatabaseService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatabaseService.Repositories.Implementations;

public class CompanyRepository(AppDbContext dbContext) : ICompanyRepository
{
    public async Task<Company> CreateAsync(Company company)
    {
        await dbContext.Companies.AddAsync(company);
        await dbContext.SaveChangesAsync();

        return await GetByNameAsync(company.Name);
    }

    public async Task UpdateAsync(Company company)
    {
        dbContext.Companies.Update(company);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(Company company)
    {
        dbContext.Companies.Remove(company);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<PositionInCompany> CreatePositionAsync(PositionInCompany positionInCompany)
    {
        await dbContext.PositionInCompanies.AddAsync(positionInCompany);
        await dbContext.SaveChangesAsync();

        return await GetPositionByNameAndCompanyIdAsync("CEO", positionInCompany.CompanyId);
    }

    public async Task UpdatePositionAsync(PositionInCompany position)
    {
        dbContext.PositionInCompanies.Update(position);
        await dbContext.SaveChangesAsync();
    }

    public async Task SaveUserInCompanyAsync(Employee employee)
    {
        await dbContext.Employees.AddAsync(employee);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateEmployeeAsync(Employee employee)
    {
        dbContext.Employees.Update(employee);
        await dbContext.SaveChangesAsync();
    }

    public async Task RemoveUserFromCompanyAsync(Employee employee)
    {
        dbContext.Employees.Remove(employee);
        await dbContext.SaveChangesAsync();
    }

    public async Task<Company> GetByIdAsync(long id)
    {
        return await dbContext.Companies
            .Include(e => e.Users)
            .Include(e => e.PositionInCompanies)
            .FirstOrDefaultAsync(e => e.Id == id) ?? throw new EntityNotFoundException(nameof(Company));
    }

    public async Task<Company> GetByNameAsync(string name)
    {
        return await dbContext.Companies
                   .Include(e => e.Users)
                   .FirstOrDefaultAsync(e => e.Name == name)
               ?? throw new EntityNotFoundException(nameof(Company));
    }

    public async Task<List<Company>> GetAllByUserAsync(User user)
    {
        return await dbContext.Companies
            .Include(company => company.Projects)
            .Where(company => company.Users!.Any(u => u.Id == user.Id))
            .ToListAsync();
    }

    public async Task<Employee> GetEmployeeByUserAndCompanyAsync(User user, Company company)
    {
        var employee = await dbContext.Employees
                .Include(e => e.PositionInCompany)
            .FirstOrDefaultAsync(e => e.UserId == user.Id && e.CompanyId == company.Id)
            ?? throw new EntityNotFoundException("Employee");
        return employee;
    }

    public async Task<Employee> GetEmployeeById(long employeeId)
    {
        var employee = await dbContext.Employees
                           .Include(e => e.PositionInCompany)
                           .Include(e => e.User)
                           .ThenInclude(e => e!.ProfilePhoto)
                           .FirstOrDefaultAsync(e => e.Id == employeeId)
                       ?? throw new EntityNotFoundException("Employee");
        return employee;
    }

    public async Task<List<Employee>> GetAllEmployeesByCompany(Company company)
    {
        var employees = await dbContext.Employees
            .Include(e => e.User)
            .ThenInclude(e => e!.ProfilePhoto)
            .Include(e => e.PositionInCompany)
            .Where(e => e.CompanyId == company.Id)
            .ToListAsync();

        return employees;
    }

    public async Task<PositionInCompany> GetPositionByIdAndCompanyIdAsync(long positionId, long companyId)
    {
        var position = await dbContext.PositionInCompanies
            .FirstOrDefaultAsync(e => e.CompanyId == companyId && e.Id == positionId) 
                       ?? throw new EntityNotFoundException(nameof(PositionInCompany));
        return position;
    }

    public async Task<PositionInCompany> GetPositionByNameAndCompanyIdAsync(string name, long companyId)
    {
        var position = await dbContext.PositionInCompanies
                           .FirstOrDefaultAsync(e => e.CompanyId == companyId && e.Name == name) 
                       ?? throw new EntityNotFoundException(nameof(PositionInCompany));
        return position;
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await dbContext.Companies
            .AnyAsync(e => e.Name == name);
    }

    public async Task<bool> ExistsByUserAndCompanyAsync(User userToAdd, Company company)
    {
        return await dbContext.Employees
            .AnyAsync(e => e.UserId == userToAdd.Id && e.CompanyId == company.Id);
    }

    public async Task<bool> PositionExistsByNameAndCompanyIdAsync(string name, long companyId)
    {
        var query =
            from positionInCompany in dbContext.PositionInCompanies
            where positionInCompany.Name == name && positionInCompany.CompanyId == companyId
            select new { positionInCompany };
        return await query.AnyAsync();
    }
}