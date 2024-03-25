using Microsoft.EntityFrameworkCore;
using Server.Config;
using Server.Data.Models;
using Server.Exceptions;
using Server.Repositories.Interfaces;

namespace Server.Repositories.Implementations;

public class CompanyRepository(AppDbContext dbContext) : ICompanyRepository
{
    public async Task CreateAsync(Company company)
    {
        await dbContext.Companies.AddAsync(company);
        await dbContext.SaveChangesAsync();
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

    public async Task<Company> GetByIdAsync(long id)
    {
        return await dbContext.Companies
                   .Include(e => e.UserCompanies)
                   .ThenInclude(e => e.User)
                   .FirstOrDefaultAsync(e => e.Id == id)
               ?? throw new EntityNotFoundException(nameof(Company));
    }

    public async Task<UserCompany> FindByUserAndCompanyAsync(User user, Company company)
    {
        var userCompany = await dbContext.UserCompanies
            .FirstOrDefaultAsync(e => e.UserId == user.Id && e.CompanyId == company.Id) 
                          ?? throw new EntityNotFoundException(nameof(UserCompany));
        return userCompany;
    }

    public async Task SaveUserInCompanyAsync(UserCompany userCompany)
    {
        await dbContext.UserCompanies.AddAsync(userCompany);
        await dbContext.SaveChangesAsync();
    }
}