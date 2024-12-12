using Course_platform;
using Course_platform.Services;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000");
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
    });

});

builder.Services.AddControllers();

DotNetEnv.Env.Load();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwtOptions =>
{
    jwtOptions.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
        ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY"))),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("admin", p =>
        p.RequireClaim(ClaimTypes.Role, "admin"));
    options.AddPolicy("regularUser", p =>
        p.RequireClaim(ClaimTypes.Role, "regularUser"));
});

builder.Services.AddDbContext<CoursePlatformDbContext>(options =>
    options.UseNpgsql($"Host={Environment.GetEnvironmentVariable("DATABASE_HOST")};Port={Environment.GetEnvironmentVariable("DATABASE_PORT")};Database={Environment.GetEnvironmentVariable("DATABASE_NAME")};Username={Environment.GetEnvironmentVariable("DATABASE_USERNAME")};Password={Environment.GetEnvironmentVariable("DATABASE_PASSWORD")}"));

builder.Services.AddSingleton<IHostedService, SchedulerService>();

var app = builder.Build();


app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseCors();
app.MapControllers();

app.Run();
