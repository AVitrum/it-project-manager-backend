using DatabaseService.Data.Models;
using Task = System.Threading.Tasks.Task;

namespace DatabaseService.Repositories.Interfaces;

public interface ICompanyRepository
{
    Task<Company> CreateAsync(Company company);
    Task UpdateAsync(Company company);
    Task<bool> DeleteAsync(Company company);
    
    Task<PositionInCompany> CreatePositionAsync(PositionInCompany positionInCompany);
    Task UpdatePositionAsync(PositionInCompany position);
    
    Task SaveUserInCompanyAsync(Employee employee);
    Task UpdateEmployeeAsync(Employee employee);
    Task RemoveUserFromCompanyAsync(Employee employee);

    Task<Company> GetByIdAsync(long id);
    Task<Company> GetByNameAsync(string name);
    Task<List<Company>> GetAllByUserAsync(User user);

    Task<Employee> GetEmployeeByUserAndCompanyAsync(User user, Company company);
    Task<List<Employee>> GetAllEmployeesByCompany(Company company);
    Task<Employee> GetEmployeeById(long employeeId);
    Task<PositionInCompany> GetPositionByIdAndCompanyIdAsync(long positionId, long companyId);
    Task<PositionInCompany> GetPositionByNameAndCompanyIdAsync(string name, long companyId);

    Task<bool> ExistsByNameAsync(string name);
    Task<bool> ExistsByUserAndCompanyAsync(User userToAdd, Company company);
    Task<bool> PositionExistsByNameAndCompanyIdAsync(string name, long companyId);
}