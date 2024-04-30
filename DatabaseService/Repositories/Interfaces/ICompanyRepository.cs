using DatabaseService.Data.Models;

namespace DatabaseService.Repositories.Interfaces;

public interface ICompanyRepository
{
    Task<Company> CreateAsync(Company company);
    Task UpdateAsync(Company company);
    Task<bool> DeleteAsync(Company company);
    
    Task<PositionInCompany> CreatePositionAsync(PositionInCompany positionInCompany);
    
    Task SaveUserInCompanyAsync(UserCompany userCompany);

    Task<Company> GetByIdAsync(long id);
    Task<Company> GetByNameAsync(string name);
    Task<UserCompany> GetByUserAndCompanyAsync(User user, Company company);

    Task<PositionInCompany> GetPositionByIdAndCompanyIdAsync(long positionId, long companyId);
    Task<PositionInCompany> GetPositionByNameAndCompanyIdAsync(string name, long companyId);
    
    Task<bool> ExistsByUserAndCompanyAsync(User userToAdd, Company company);
}