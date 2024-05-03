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

    public async Task SaveUserInCompanyAsync(UserCompany userCompany)
    {
        await dbContext.UserCompanies.AddAsync(userCompany);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateEmployeeAsync(UserCompany userCompany)
    {
        dbContext.UserCompanies.Update(userCompany);
        await dbContext.SaveChangesAsync();
    }

    public async Task RemoveUserFromCompanyAsync(UserCompany userCompany)
    {
        dbContext.UserCompanies.Remove(userCompany);
        await dbContext.SaveChangesAsync();
    }

    public async Task<Company> GetByIdAsync(long id)
    {
        var query =
            from company in dbContext.Companies
            where company.Id == id
            select new
            {
                Company = company,
                UserCompanies = company.UserCompanies!.Select(uc => new
                {
                    UserCompany = uc, uc.User, uc.PositionInCompany
                })
            };
        var result = await query.FirstOrDefaultAsync() 
                     ?? throw new EntityNotFoundException(nameof(Company));

        var companyEntity = result.Company;
        companyEntity.UserCompanies = result.UserCompanies
            .Select(uc => uc.UserCompany).ToList();
        return companyEntity;
    }

    public async Task<Company> GetByNameAsync(string name)
    {
        return await dbContext.Companies
                   .Include(e => e.UserCompanies)!
                   .ThenInclude(e => e.User)
                   .FirstOrDefaultAsync(e => e.Name == name)
               ?? throw new EntityNotFoundException(nameof(Company));
    }

    public async Task<UserCompany> GetEmployeeByUserAndCompanyAsync(User user, Company company)
    {
        var userCompany = await dbContext.UserCompanies
            .FirstOrDefaultAsync(e => e.UserId == user.Id && e.CompanyId == company.Id)
            ?? throw new EntityNotFoundException("Employee");
        return userCompany;
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

    public async Task<bool> ExistsByUserAndCompanyAsync(User userToAdd, Company company)
    {
        return await dbContext.UserCompanies
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