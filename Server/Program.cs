using DatabaseService;
using Server.Config;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var inDocker = Environment.GetEnvironmentVariable("IN_DOCKER") == "true";

builder.Services.AddDbContext<AppDbContext>(
    o => o.UseNpgsql(builder.Configuration.GetConnectionString(
        inDocker ? "IN_DOCKER" : "DefaultConnection"))
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policyBuilder =>
    {
        policyBuilder.WithOrigins("http://localhost:5173"); // need to change
        policyBuilder.AllowAnyHeader();
        policyBuilder.AllowAnyMethod();
        policyBuilder.AllowCredentials();
    });
});
builder.Services.AddFeatureAddServices();
builder.Services.AddFeatureAddCustomRepositories();
builder.Services.AddCustomSwaggerGen();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();
app.UseStatusCodePages();
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseCors("frontend");
app.Run();