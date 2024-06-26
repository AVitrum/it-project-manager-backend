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
    Task<Company> GetByIdForOperations(long id);
    Task<List<Company>> GetAllByUserAsync(User user, string sortOrder);

    Task<Employee> GetEmployeeByUserAndCompanyAsync(User user, Company company);
    Task<List<Employee>> GetAllEmployeesByCompany(Company company);
    Task<Employee> GetEmployeeById(long employeeId);
    
    Task<PositionInCompany> GetPositionByIdAndCompanyIdAsync(long positionId, long companyId);
    Task<PositionInCompany> GetPositionByNameAndCompanyIdAsync(string name, long companyId);
    Task<List<PositionInCompany>> GetPositionsByCompanyIdAsync(long companyId);

    Task<double> GetAverageUserSalary(User user);
    Task<double> GetAverageSalaryInCompany(Company company);
    Task<double> GetMinSalaryInCompany(Company company);
    Task<double> GetMaxSalaryInCompany(Company company);
    Task<double> GetAllCostsInCompany(Company company);
    
    Task<bool> ExistsByNameAsync(string name);
    Task<bool> ExistsByUserAndCompanyAsync(User userToAdd, Company company);
    Task<bool> PositionExistsByNameAndCompanyIdAsync(string name, long companyId);
}