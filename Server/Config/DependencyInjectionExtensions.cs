using System.Text;
using DatabaseService.Repositories.Implementations;
using DatabaseService.Repositories.Interfaces;
using EmailService;
using FileService;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Server.Services.Implementations;
using Server.Services.Interfaces;
using Swashbuckle.AspNetCore.Filters;

namespace Server.Config;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddFeatureAddServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IAuthService, AuthService>().AddProblemDetails()
            .AddExceptionHandler<GlobalExceptionHandler>();
        serviceCollection.AddScoped<IUserService, UserService>().AddProblemDetails()
            .AddExceptionHandler<GlobalExceptionHandler>();
        serviceCollection.AddScoped<ICompanyService, CompanyService>().AddProblemDetails()
            .AddExceptionHandler<GlobalExceptionHandler>();
        serviceCollection.AddScoped<IEmailSender, EmailSender>().AddProblemDetails()
            .AddExceptionHandler<GlobalExceptionHandler>();
        serviceCollection.AddScoped<IFileService, FileService.FileService>().AddProblemDetails()
            .AddExceptionHandler<GlobalExceptionHandler>();
        serviceCollection.AddScoped<IEmployeeService, EmployeeService>().AddProblemDetails()
            .AddExceptionHandler<GlobalExceptionHandler>();

        return serviceCollection;
    }
    
    public static IServiceCollection AddFeatureAddCustomRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IUserRepository, UserRepository>().AddProblemDetails().AddExceptionHandler<GlobalExceptionHandler>();
        serviceCollection.AddScoped<ICompanyRepository, CompanyRepository>().AddProblemDetails().AddExceptionHandler<GlobalExceptionHandler>();

        return serviceCollection;
    }
    
    public static IServiceCollection AddCustomSwaggerGen(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });

            options.OperationFilter<SecurityRequirementsOperationFilter>();
        });

        return serviceCollection;
    }
    
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddAuthentication().AddJwtBearer(options =>
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

        return serviceCollection;
    }
}