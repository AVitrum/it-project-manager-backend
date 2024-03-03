using System.Text;
using EmailSystem;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Server.Repositories.Implementations;
using Server.Repositories.Interfaces;
using Server.Services.Implementations;
using Server.Services.Interfaces;
using Swashbuckle.AspNetCore.Filters;

namespace Server.Config;

public static class AppConfiguration
{
    public static void AddCustomServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>().AddProblemDetails().AddExceptionHandler<GlobalExceptionHandler>();
        services.AddScoped<IUserService, UserService>().AddProblemDetails().AddExceptionHandler<GlobalExceptionHandler>();
        services.AddScoped<ITeamService, TeamService>().AddProblemDetails().AddExceptionHandler<GlobalExceptionHandler>();
        services.AddScoped<IEmailSender, EmailSender>().AddProblemDetails()
            .AddExceptionHandler<GlobalExceptionHandler>();
    }
    
    public static void AddCustomRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>().AddProblemDetails().AddExceptionHandler<GlobalExceptionHandler>();
        services.AddScoped<ITeamRepository, TeamRepository>().AddProblemDetails().AddExceptionHandler<GlobalExceptionHandler>();
    }
    
    public static void AddCustomSwaggerGen(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });

            options.OperationFilter<SecurityRequirementsOperationFilter>();
        });
    }
    
    public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication().AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    configuration.GetSection("AppSettings:Token").Value!))
            };
        });
    }
}