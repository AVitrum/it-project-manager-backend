using Microsoft.EntityFrameworkCore;
using Server.Config;
using Server.Data.Models;
using Server.Exceptions;
using Server.Repositories.Interfaces;

namespace Server.Repositories.Implementations;

public class CompanyRepository(AppDbContext dbContext) : ICompanyRepository
{
    public async Task<Company> CreateAsync(Company company)
    {
        await dbContext.Companies.AddAsync(company);
        await dbContext.SaveChangesAsync();

        return await GetByNameAsync(company.Name);
    }

    public async Task<PositionInCompany> CreatePositionAsync(PositionInCompany positionInCompany)
    {
        await dbContext.PositionInCompanies.AddAsync(positionInCompany);
        await dbContext.SaveChangesAsync();

        return await FindPositionByNameAndCompanyIdAsync("CEO", positionInCompany.CompanyId);
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
                   .Include(e => e.UserCompanies)!
                   .ThenInclude(e => e.User)
                   .Include(e => e.PositionInCompanies)
                   .FirstOrDefaultAsync(e => e.Id == id)
               ?? throw new EntityNotFoundException(nameof(Company));
    }

    public async Task<Company> GetByNameAsync(string name)
    {
        return await dbContext.Companies
                   .Include(e => e.UserCompanies)!
                   .ThenInclude(e => e.User)
                   .FirstOrDefaultAsync(e => e.Name == name)
               ?? throw new EntityNotFoundException(nameof(Company));
    }

    public async Task<UserCompany> FindByUserAndCompanyAsync(User user, Company company)
    {
        var userCompany = await dbContext.UserCompanies
            .FirstOrDefaultAsync(e => e.UserId == user.Id && e.CompanyId == company.Id)
            ?? throw new EntityNotFoundException("Employee");
        return userCompany;
    }

    public async Task<PositionInCompany> FindPositionByIdAndCompanyIdAsync(long positionId, long companyId)
    {
        var position = await dbContext.PositionInCompanies
            .FirstOrDefaultAsync(e => e.CompanyId == companyId && e.Id == positionId) 
                       ?? throw new EntityNotFoundException(nameof(PositionInCompany));
        return position;
    }

    public async Task<bool> ExistsByUserAndCompanyAsync(User userToAdd, Company company)
    {
        return await dbContext.UserCompanies
            .AnyAsync(e => e.UserId == userToAdd.Id && e.CompanyId == company.Id);
    }

    public async Task<PositionInCompany> FindPositionByNameAndCompanyIdAsync(string name, long companyId)
    {
        var position = await dbContext.PositionInCompanies
                           .FirstOrDefaultAsync(e => e.CompanyId == companyId && e.Name == name) 
                       ?? throw new EntityNotFoundException(nameof(PositionInCompany));
        return position;
    }

    public async Task SaveUserInCompanyAsync(UserCompany userCompany)
    {
        await dbContext.UserCompanies.AddAsync(userCompany);
        await dbContext.SaveChangesAsync();
    }
}