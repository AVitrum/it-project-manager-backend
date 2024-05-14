using DatabaseService.Data.Enums;
using DatabaseService.Data.Models;
using DatabaseService.Exceptions;
using DatabaseService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace DatabaseService.Repositories.Implementations;

public class CompanyRepository(AppDbContext dbContext, IConfiguration configuration) : ICompanyRepository
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

        return await GetPositionByNameAndCompanyIdAsync(positionInCompany.Name, positionInCompany.CompanyId);
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
        await using var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync();
        
        await using var command = new NpgsqlCommand(
            """
            
                        SELECT 
                            t."Id", t."Budget", t."Description", t."Name", t."PictureLink", t."PictureName", 
                            t."RegistrationDate", p."Id", p."CompanyId", p."Name", p."Permissions", p."Priority", 
                            t0."Id", t0."CompanyId", t0."PositionInCompanyId", t0."Salary", t0."UserId", t0."Id0", 
                            t0."Email", t0."PasswordHash", t0."PasswordResetToken", t0."PasswordSalt", t0."PhoneNumber", 
                            t0."UserRegistrationDate", t0."ResetTokenExpires", t0."Username", t0."VerificationToken", t0."VerifiedAt"
                        FROM (
                            SELECT c."Id", c."Budget", c."Description", c."Name", c."PictureLink", c."PictureName", c."RegistrationDate"
                            FROM "Companies" AS c
                            WHERE c."Id" = @id
                            LIMIT 1
                        ) AS t
                        LEFT JOIN "PositionInCompanies" AS p ON t."Id" = p."CompanyId"
                        LEFT JOIN (
                            SELECT e."Id", e."CompanyId", e."PositionInCompanyId", e."Salary", e."UserId", 
                                   u."Id" AS "Id0", u."Email", u."PasswordHash", u."PasswordResetToken", 
                                   u."PasswordSalt", u."PhoneNumber", u."RegistrationDate" as "UserRegistrationDate", 
                                   u."ResetTokenExpires", u."Username", u."VerificationToken", u."VerifiedAt"
                            FROM "Employees" AS e
                            INNER JOIN "Users" AS u ON e."UserId" = u."Id"
                        ) AS t0 ON t."Id" = t0."CompanyId"
                        ORDER BY t."Id", p."Id", t0."Id"
                        
            """,
            connection);
        command.Parameters.AddWithValue("id", id);
        
        await using var reader = await command.ExecuteReaderAsync();
        
        Company? company = null;
        while (await reader.ReadAsync())
        {
            company ??= new Company
            {
                Id = reader.GetInt64(reader.GetOrdinal("Id")),
                Budget = reader.GetDouble(reader.GetOrdinal("Budget")),
                Description = (reader.IsDBNull(reader.GetOrdinal("Description"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Description")))!,
                Name = reader.GetString(reader.GetOrdinal("Name")),
                RegistrationDate = reader.GetDateTime(reader.GetOrdinal("RegistrationDate")),
                PictureLink = reader.IsDBNull(reader.GetOrdinal("PictureLink")) ? null
                    : reader.GetString(reader.GetOrdinal("PictureLink")),
                Users = new List<User>(),
                PositionInCompanies = new List<PositionInCompany>()
            };
        
            if (reader.IsDBNull(reader.GetOrdinal("UserId"))) continue;
            var email = reader.GetString(reader.GetOrdinal("Email"));
            if (company.Users!.Any(u => u.Email == email)) continue;
            var user = new User
            {
                Id = reader.GetInt64(reader.GetOrdinal("UserId")),
                Email = email,
                PasswordHash = (reader.IsDBNull(reader.GetOrdinal("PasswordHash")) ? null : reader.GetFieldValue<byte[]>(reader.GetOrdinal("PasswordHash")))!,
                PasswordSalt = (reader.IsDBNull(reader.GetOrdinal("PasswordSalt")) ? null : reader.GetFieldValue<byte[]>(reader.GetOrdinal("PasswordSalt")))!,
                PhoneNumber =  reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                RegistrationDate = reader.GetDateTime(reader.GetOrdinal("UserRegistrationDate")),
                Username = reader.GetString(reader.GetOrdinal("Username")),
                VerifiedAt = reader.GetDateTime(reader.GetOrdinal("VerifiedAt")),
            };
            company.Users!.Add(user);
        }

        while (await reader.ReadAsync())
        {
            if (reader.IsDBNull(reader.GetOrdinal("PositionInCompanyId"))) continue;
            var position = new PositionInCompany
            {
                CompanyId = company!.Id,
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Priority = reader.GetInt64(reader.GetOrdinal("Priority")),
                Permissions = (PositionPermissions) reader.GetInt64(reader.GetOrdinal("Permissions"))
            };
            
            company.PositionInCompanies!.Add(position);
        }
        return company ?? throw new EntityNotFoundException(nameof(Company));
    }

    public async Task<Company> GetByIdForOperations(long id)
    {
        return await dbContext.Companies
            .Include(e => e.Users)
            .Include(e => e.PositionInCompanies)
            .FirstOrDefaultAsync(e => e.Id == id) ?? throw new EntityNotFoundException(nameof(Company));
    }
    
    public async Task<Company> GetByNameAsync(string name)
    {
        await using var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            """
            
                        SELECT c."Id", c."Budget", c."Description", c."Name", c."RegistrationDate", c."PictureLink",
                            e."UserId", e."CompanyId", e."PositionInCompanyId", e."Salary",
                            u."Id" AS "UserId", u."Email", u."PasswordHash", u."PasswordResetToken", u."PasswordSalt",
                            u."PhoneNumber", u."RegistrationDate" AS "UserRegistrationDate", u."ResetTokenExpires",
                            u."Username", u."VerificationToken", u."VerifiedAt",
                            p."Name" AS "PositionName", p."Priority" AS "PositionPriority", p."Permissions" AS "PositionPermissions"
                        FROM "Companies" AS c
                        LEFT JOIN "Employees" AS e ON c."Id" = e."CompanyId"
                        LEFT JOIN "Users" AS u ON e."UserId" = u."Id"
                        LEFT JOIN "PositionInCompanies" AS p ON c."Id" = p."CompanyId"
                        WHERE c."Name" = @name
                        ORDER BY e."UserId" ASC 
                        
            """,
            connection);
        command.Parameters.AddWithValue("name", name);

        await using var reader = await command.ExecuteReaderAsync();

        Company? company = null;
        
        while (await reader.ReadAsync())
        {
            company ??= new Company
            {
                Id = reader.GetInt64(reader.GetOrdinal("Id")),
                Budget = reader.GetDouble(reader.GetOrdinal("Budget")),
                Description = (reader.IsDBNull(reader.GetOrdinal("Description"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Description")))!,
                Name = reader.GetString(reader.GetOrdinal("Name")),
                RegistrationDate = reader.GetDateTime(reader.GetOrdinal("RegistrationDate")),
                PictureLink = reader.IsDBNull(reader.GetOrdinal("PictureLink")) ? null
                    : reader.GetString(reader.GetOrdinal("PictureLink")),
                Users = new List<User>(),
                PositionInCompanies = new List<PositionInCompany>()
            };
        
            if (reader.IsDBNull(reader.GetOrdinal("UserId"))) continue;
            var email = reader.GetString(reader.GetOrdinal("Email"));
            if (company.Users!.Any(u => u.Email == email)) continue;
            var user = new User
            {
                Id = reader.GetInt64(reader.GetOrdinal("UserId")),
                Email = email,
                PasswordHash = (reader.IsDBNull(reader.GetOrdinal("PasswordHash")) ? null : reader.GetFieldValue<byte[]>(reader.GetOrdinal("PasswordHash")))!,
                PasswordSalt = (reader.IsDBNull(reader.GetOrdinal("PasswordSalt")) ? null : reader.GetFieldValue<byte[]>(reader.GetOrdinal("PasswordSalt")))!,
                PhoneNumber =  reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                RegistrationDate = reader.GetDateTime(reader.GetOrdinal("UserRegistrationDate")),
                Username = reader.GetString(reader.GetOrdinal("Username")),
                VerifiedAt = reader.GetDateTime(reader.GetOrdinal("VerifiedAt")),
            };
            company.Users!.Add(user);
        }

        while (await reader.ReadAsync())
        {
            if (reader.IsDBNull(reader.GetOrdinal("PositionInCompanyId"))) continue;
            var position = new PositionInCompany
            {
                CompanyId = company!.Id,
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Priority = reader.GetInt64(reader.GetOrdinal("Priority")),
                Permissions = (PositionPermissions) reader.GetInt64(reader.GetOrdinal("Permissions"))
            };
            
            company.PositionInCompanies!.Add(position);
        }
        return company ?? throw new EntityNotFoundException(nameof(Company));
    }

    public async Task<List<Company>> GetAllByUserAsync(User user, string sortOrder)
    {
        await using var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            """
            
                SELECT *
                FROM "Companies"
                WHERE "Id" IN (
                    SELECT DISTINCT "CompanyId"
                    FROM "Employees"
                    WHERE "UserId" = @userId
                )
                ORDER BY "RegistrationDate" 
            """ + sortOrder + ";",
            connection);
        command.Parameters.AddWithValue("userId", user.Id);

        await using var reader = await command.ExecuteReaderAsync();

        var companies = new List<Company>();
        
         while (await reader.ReadAsync())
         {
            var company = new Company
            {
                Id = reader.GetInt64(reader.GetOrdinal("Id")),
                Budget = reader.GetDouble(reader.GetOrdinal("Budget")),
                Description = (reader.IsDBNull(reader.GetOrdinal("Description"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Description")))!,
                Name = reader.GetString(reader.GetOrdinal("Name")),
                RegistrationDate = reader.GetDateTime(reader.GetOrdinal("RegistrationDate")),
                PictureLink = reader.IsDBNull(reader.GetOrdinal("PictureLink")) ? null
                    : reader.GetString(reader.GetOrdinal("PictureLink")),
                Users = new List<User>(),
                PositionInCompanies = new List<PositionInCompany>()
            };
            companies.Add(company);
        }
        return companies ?? throw new EntityNotFoundException(nameof(Company));
    }

    public async Task<Employee> GetEmployeeByUserAndCompanyAsync(User user, Company company)
    {
        return await dbContext.Employees
            .Include(e => e.PositionInCompany)
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.CompanyId == company.Id && e.User.Id == user.Id);
    }
    
    public async Task<Employee> GetEmployeeById(long employeeId)
    {
        await using var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            """
            
                    SELECT e.*,
                    u."Id" AS "UserId", u."Username", u."Email", u."PasswordHash", 
                    u."PasswordResetToken", u."PasswordSalt", u."PhoneNumber", u."RegistrationDate",
                    p."Id" as "PositionId", p."Name", p."Priority", p."Permissions",
                    ph."Id" as "PhotoId", ph."PictureLink"
                    FROM "Employees" e
                    JOIN "Users" u ON e."UserId" = u."Id"
                    JOIN "PositionInCompanies" p ON e."PositionInCompanyId" = p."Id"
                    LEFT JOIN "ProfilePhotos" ph ON ph."UserId" = u."Id"
                    WHERE e."Id" = @id;
                
            """,
            connection);
        command.Parameters.AddWithValue("id", employeeId);
        
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync()) 
        {
            var user = new User
            {
                Id = reader.GetInt64(reader.GetOrdinal("UserId")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                PasswordHash = (reader.IsDBNull(reader.GetOrdinal("PasswordHash"))
                    ? null
                    : reader.GetFieldValue<byte[]>(reader.GetOrdinal("PasswordHash")))!,
                PasswordSalt = (reader.IsDBNull(reader.GetOrdinal("PasswordSalt"))
                    ? null
                    : reader.GetFieldValue<byte[]>(reader.GetOrdinal("PasswordSalt")))!,
                PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                RegistrationDate = reader.GetDateTime(reader.GetOrdinal("RegistrationDate")),
                Username = reader.GetString(reader.GetOrdinal("Username")),
            };

            if (!reader.IsDBNull(reader.GetOrdinal("PictureLink")))
            {
                var photo = new ProfilePhoto
                {
                    Id = reader.GetInt64(reader.GetOrdinal("PhotoId")),
                    PictureLink = reader.GetString(reader.GetOrdinal("PictureLink")),
                };
                user.ProfilePhoto = photo;
            }
            
            var positionInCompany = new PositionInCompany
            {
                CompanyId = reader.GetInt64(reader.GetOrdinal("CompanyId")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Priority = reader.GetInt64(reader.GetOrdinal("Priority"))
            };

            var employee = new Employee
            {
                Id = reader.GetInt64(reader.GetOrdinal("Id")),
                UserId = user.Id,
                Salary = reader.GetDouble(reader.GetOrdinal("Salary")),
                CompanyId = positionInCompany.CompanyId,
                PositionInCompanyId = reader.GetInt64(reader.GetOrdinal("PositionInCompanyId")),
                PositionInCompany = positionInCompany,
                User = user
            };
            return employee;
        }

        throw new EntityNotFoundException(nameof(Employee));
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

    public async Task<List<PositionInCompany>> GetPositionsByCompanyIdAsync(long id)
    {
        await using var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync();
    
        await using var command = new NpgsqlCommand(
            """
            
                    SELECT DISTINCT "Name", "Priority", "Permissions"
                    FROM "PositionInCompanies"
                    WHERE "CompanyId" = @id
                    
            """,
            connection);
        command.Parameters.AddWithValue("id", id);
    
        await using var reader = await command.ExecuteReaderAsync();

        var positions = new List<PositionInCompany>();
    
        while (await reader.ReadAsync())
        {
            var position = new PositionInCompany
            {
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Priority = reader.GetInt32(reader.GetOrdinal("Priority")),
                Permissions = (PositionPermissions)reader.GetInt32(reader.GetOrdinal("Permissions")),
                CompanyId = id
            };
            positions.Add(position);
        }
    
        return positions;
    }

    public async Task<double> GetAverageUserSalary(User user)
    {
        await using var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            """
            
                    SELECT u."Id", AVG(e."Salary") as "AverageSalary"
                    FROM "Users" as u
                    LEFT JOIN "Employees" as e ON e."UserId" = u."Id"
                    WHERE u."Id" = @id
                    GROUP BY u."Id"
                
            """,
            connection);
        command.Parameters.AddWithValue("id", user.Id);
        
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            return reader.IsDBNull(reader.GetOrdinal("AverageSalary")) 
                ? 0 
                : reader.GetDouble(reader.GetOrdinal("AverageSalary"));
        }

        return 0;
    }

    public async Task<double> GetAverageSalaryInCompany(Company company)
    {
        await using var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            """
            
                    SELECT c."Id", avg(e."Salary")
                    FROM "Companies" AS c
                    LEFT JOIN "Employees" AS e on c."Id" = e."CompanyId"
                    WHERE c."Id" = @id
                    GROUP BY c."Id"
                
            """,
            connection);
        command.Parameters.AddWithValue("id", company.Id);
        
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            return reader.GetDouble(reader.GetOrdinal("avg"));
        }

        return 0;
    }

    public async Task<double> GetMinSalaryInCompany(Company company)
    {
        await using var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            """
            
                    SELECT c."Id", min(e."Salary")
                    FROM "Companies" AS c
                    LEFT JOIN "Employees" AS e on c."Id" = e."CompanyId"
                    WHERE c."Id" = @id
                    GROUP BY c."Id"
                
            """,
            connection);
        command.Parameters.AddWithValue("id", company.Id);
        
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            return reader.GetDouble(reader.GetOrdinal("min"));
        }

        return 0;
    }

    public async Task<double> GetMaxSalaryInCompany(Company company)
    {
        await using var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            """
            
                    SELECT c."Id", max(e."Salary")
                    FROM "Companies" AS c
                    LEFT JOIN "Employees" AS e on c."Id" = e."CompanyId"
                    WHERE c."Id" = @id
                    GROUP BY c."Id"
                
            """,
            connection);
        command.Parameters.AddWithValue("id", company.Id);
        
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            return reader.GetDouble(reader.GetOrdinal("max"));
        }

        return 0;
    }
    
    public async Task<double> GetAllCostsInCompany(Company company)
    {
        await using var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            """
            
                    SELECT c."Id", 
                    e."EmployeesSalaries",
                    p."ProjectBudgets",
                    e."EmployeesSalaries" + p."ProjectBudgets" AS "Total"
                    FROM (
                        SELECT "CompanyId", 
                               SUM("Salary") AS "EmployeesSalaries"
                        FROM "Employees"
                        GROUP BY "CompanyId"
                    ) AS e
                    JOIN (
                        SELECT "CompanyId", 
                               SUM("Budget") AS "ProjectBudgets"
                        FROM "Projects"
                        GROUP BY "CompanyId"
                    ) AS p ON e."CompanyId" = p."CompanyId"
                    JOIN "Companies" AS c ON e."CompanyId" = c."Id"
                    WHERE c."Id" = @id
                
            """,
            connection);
        command.Parameters.AddWithValue("id", company.Id);
        
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var totalCosts = reader.GetDouble(reader.GetOrdinal("Total"));
            return totalCosts;
        }

        return 0;
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await dbContext.Companies
            .AnyAsync(e => e.Name == name);
    }

    public async Task<bool> ExistsByUserAndCompanyAsync(User userToAdd, Company company)
    {
        await using var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync();
        
        await using var command = new NpgsqlCommand(
            """
            
                    SELECT EXISTS (
                        SELECT 1
                        FROM "Employees"
                        WHERE "UserId" = @id AND "CompanyId" = @companyId
                    )
                    
            """,
            connection);
        command.Parameters.AddWithValue("id", userToAdd.Id);
        command.Parameters.AddWithValue("companyId", company.Id);
        
        return (bool) (await command.ExecuteScalarAsync())!;
    }

    public async Task<bool> PositionExistsByNameAndCompanyIdAsync(string name, long companyId)
    {
        await using var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync();
    
        await using var command = new NpgsqlCommand(
            """
            
                    SELECT EXISTS (
                        SELECT 1
                        FROM "PositionInCompanies"
                        WHERE "Name" = @name AND "CompanyId" = @companyId
                    )
                    
            """,
            connection);
        command.Parameters.AddWithValue("name", name);
        command.Parameters.AddWithValue("companyId", companyId);
    
        return (bool) (await command.ExecuteScalarAsync())!;
    }
}