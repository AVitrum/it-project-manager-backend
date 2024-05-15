using System.Security.Claims;
using DatabaseService.Data.Models;
using DatabaseService.Exceptions;
using DatabaseService.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace DatabaseService.Repositories.Implementations;

public class UserRepository(
    IHttpContextAccessor httpContextAccessor, 
    IConfiguration configuration,
    AppDbContext dbContext) 
    : IUserRepository
{
    public async Task CreateAsync(User user)
    {
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync();
    }

    public async Task AddProfilePhoto(ProfilePhoto profilePhoto)
    {
        await dbContext.ProfilePhotos.AddAsync(profilePhoto);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
    {
        if (await dbContext.RefreshTokens.AnyAsync(e => e.Token == refreshToken.Token))
            dbContext.RefreshTokens.Update(refreshToken);
        else
            await dbContext.RefreshTokens.AddAsync(refreshToken);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(User user)
    {
        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteProfilePhotoAsync(ProfilePhoto profilePhoto)
    {
        dbContext.ProfilePhotos.Remove(profilePhoto);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<User> GetByJwtAsync()
    {
        const string email = ClaimTypes.Email;

        return httpContextAccessor.HttpContext is not null
            ? await GetByEmailAsync(httpContextAccessor.HttpContext.User.FindFirstValue(email)!)
            : throw new UserNotFoundException(email);
    }

    public async Task<(User, ProfilePhoto?)> GetByJwtWithPhotoAsync()
    {
        var user = await GetByJwtAsync();
        
        var query = 
            from profilePhoto in dbContext.ProfilePhotos 
            where profilePhoto.UserId == user.Id
            select new { profilePhoto };

        var result = await query.FirstOrDefaultAsync();
        return (user, result?.profilePhoto);
    }

    public async Task<(User, RefreshToken)> GetByRefreshToken(string refreshToken)
    {
        await using var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            """
            
                    SELECT u."Id", u."Email", u."PasswordHash", u."PasswordResetToken", u."PasswordSalt", u."PhoneNumber",
                           u."RegistrationDate", u."ResetTokenExpires", u."Username", u."VerificationToken", u."VerifiedAt",
                           r."Id", r."Created", r."Expired", r."Expires", r."Token", r."UserId"
                    FROM "Users" AS u
                    INNER JOIN "RefreshTokens" AS r ON u."Id" = r."UserId"
                    WHERE r."Token" = @refreshToken
                    LIMIT 1
            """,
            connection);

        command.Parameters.AddWithValue("refreshToken", refreshToken);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            throw new EntityNotFoundException(nameof(User));
        }

        var user = new User
        {
            Id = reader.GetInt64(0),
            Email = reader.GetString(1),
            PasswordHash = (reader.IsDBNull(2) ? null : reader.GetFieldValue<byte[]>(2))!,
            PasswordResetToken = reader.IsDBNull(3) ? null : reader.GetString(3),
            PasswordSalt = (reader.IsDBNull(4) ? null : reader.GetFieldValue<byte[]>(4))!,
            PhoneNumber = reader.IsDBNull(5) ? null : reader.GetString(5),
            RegistrationDate = reader.GetDateTime(6),
            Username = reader.GetString(8),
            VerificationToken = reader.IsDBNull(9) ? null : reader.GetString(9),
            VerifiedAt = reader.IsDBNull(10) ? null : reader.GetDateTime(10)
        };

        var refreshTokenEntity = new RefreshToken
        {
            Id = reader.GetInt64(11),
            Created = reader.GetDateTime(12),
            Expired = reader.GetBoolean(13),
            Expires = reader.GetDateTime(14),
            Token = reader.GetString(15),
            UserId = reader.GetInt64(16)
        };

        return (user, refreshTokenEntity);
    }

    public async Task<User> GetByIdAsync(long id)
    {
        return await dbContext.Users
                   .FirstOrDefaultAsync(e => e.Id.Equals(id))
               ?? throw new UserNotFoundException(id);
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await dbContext.Users
            .Include(e => e.ProfilePhoto)
            .FirstOrDefaultAsync(e => e.Email == email) ?? throw new UserNotFoundException(email);
    }

    public async Task<User> GetByTokenAsync(string token)
    {
        await using var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            """
            
                    SELECT u.*,
                    ph."Id" as "PhotoId", ph."PictureLink", ph."PictureName"
                    FROM "Users" u
                    LEFT JOIN "ProfilePhotos" ph ON ph."UserId" = u."Id"
                    WHERE u."VerificationToken" = @token;
                
            """,
            connection);
        command.Parameters.AddWithValue("token", token);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var user = new User
            {
                Id = reader.GetInt64(reader.GetOrdinal("Id")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                PasswordHash = (reader.IsDBNull(reader.GetOrdinal("PasswordHash"))
                    ? null
                    : reader.GetFieldValue<byte[]>(reader.GetOrdinal("PasswordHash")))!,
                PasswordSalt = (reader.IsDBNull(reader.GetOrdinal("PasswordSalt"))
                    ? null
                    : reader.GetFieldValue<byte[]>(reader.GetOrdinal("PasswordSalt")))!,
                PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                RegistrationDate = reader.GetDateTime(reader.GetOrdinal("RegistrationDate")),
                Username = reader.GetString(reader.GetOrdinal("Username")),
                VerificationToken = reader.IsDBNull(reader.GetOrdinal("VerificationToken"))
                ? null
                : reader.GetString(reader.GetOrdinal("VerificationToken")),
                VerifiedAt = reader.IsDBNull(reader.GetOrdinal("VerifiedAt"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("VerifiedAt"))
            };

            if (reader.IsDBNull(reader.GetOrdinal("PictureLink"))) return user;
            var photo = new ProfilePhoto
            {
                Id = reader.GetInt64(reader.GetOrdinal("PhotoId")),
                PictureName = reader.GetString(reader.GetOrdinal("PictureName")),
                PictureLink = reader.GetString(reader.GetOrdinal("PictureLink")),
            };
            user.ProfilePhoto = photo;
            return user;
        }

        throw new UserNotFoundException(token);
    }

    public async Task<User> GetByPasswordResetTokenAsync(string token)
    {
        return await dbContext.Users.FirstOrDefaultAsync(u => u.PasswordResetToken!.Equals(token))
               ?? throw new UserException("Wrong Password Reset Token!");
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await dbContext.Users.AnyAsync(u => u.Email.Equals(email));
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        await using var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync();
    
        await using var command = new NpgsqlCommand(
            """
            
                    SELECT EXISTS (
                        SELECT 1
                        FROM "Users"
                        WHERE "Username" = @name
                    )
                    
            """,
            connection);
        command.Parameters.AddWithValue("name", username);
        return (bool) (await command.ExecuteScalarAsync())!;
    }
}