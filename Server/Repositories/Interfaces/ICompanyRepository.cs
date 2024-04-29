using Server.Data.Models;

namespace Server.Repositories.Interfaces;

public interface ICompanyRepository
{
    Task<Company> CreateAsync(Company company);
    Task<PositionInCompany> CreatePositionAsync(PositionInCompany positionInCompany);
    Task UpdateAsync(Company company);
    Task<bool> DeleteAsync(Company company);
    Task<Company> GetByIdAsync(long id);
    Task<Company> GetByNameAsync(string name);
    Task SaveUserInCompanyAsync(UserCompany userCompany);
    Task<UserCompany> FindByUserAndCompanyAsync(User user, Company company);
    Task<PositionInCompany> FindPositionByIdAndCompanyIdAsync(long positionId, long companyId);
    Task<PositionInCompany> FindPositionByNameAndCompanyIdAsync(string name, long companyId);
    Task<bool> ExistsByUserAndCompanyAsync(User userToAdd, Company company);
}