using Server.Data.Models;

namespace Server.Repositories.Interfaces;

public interface ICompanyRepository
{
    Task CreateAsync(Company company);
    Task UpdateAsync(Company company);
    Task<bool> DeleteAsync(Company company);
    Task<Company> GetByIdAsync(long id);
    Task SaveUserInCompanyAsync(UserCompany userCompany);
    Task<UserCompany?> FindByUserAndCompanyAsync(User user, Company company);
}