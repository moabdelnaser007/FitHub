using FitHubBackendAPI.Data;
using FitHubBackendAPI.Data.DataSeeder;
using FitHubBackendAPI.Repository.Implementation;
using FitHubBackendAPI.Repository.Interfaces;
using FitHubBackendAPI.Services.Implementation.AdminServices;
using FitHubBackendAPI.Services.Implementation.AuthServices;
using FitHubBackendAPI.Services.Implementation.UserServices;
using FitHubBackendAPI.Services.Interfaces.AdminServices;
using FitHubBackendAPI.Services.Interfaces.AuthServices;
using FitHubBackendAPI.Services.Interfaces.UserServices;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

namespace FitHubBackendAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ===============================
            // 1) Serilog Logging
            // ===============================
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Host.UseSerilog();

            // ===============================
            // 2) Add DbContext (EF Core 8)
            // ===============================
            builder.Services.AddDbContext<FitHubDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                // options.UseLazyLoadingProxies(); // enable if needed
            });


            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IEmailService, EmailService>();

            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddScoped<IAdminOwnerService, AdminOwnerService>();


            // ===============================
            // 3) Add AutoMapper
            // ===============================
            //builder.Services.AddAutoMapper(typeof(Program));

            // ===============================
            // 4) Add FluentValidation
            // ===============================
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

            // ===============================
            // 5) Add Controllers
            // ===============================
            builder.Services.AddControllers();
            //    .AddNewtonsoftJson(); // Optional - if you want Newtonsoft

            // ===============================
            // 6) Add CORS
            // ===============================
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowAnyOrigin();
                });
            });

            // ===============================
            // 7) Add JWT Authentication (Optional)
            // ===============================
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Bearer";
                options.DefaultChallengeScheme = "Bearer";
            })
            .AddJwtBearer("Bearer", options =>
            {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;

            options.TokenValidationParameters = new TokenValidationParameters
            {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
        ),
        ClockSkew = TimeSpan.Zero
    };
});
            builder.Services.AddAuthorization();


            // ===============================
            // 8) Add Swagger
            // ===============================
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // ===============================
            // ENABLE Swagger
            // ===============================
            app.UseSwagger();
            app.UseSwaggerUI();

            // ===============================
            // Middlewares
            // ===============================
            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<FitHubDbContext>();
                await DataSeeder.SeedAdminAsync(context);
            }

            app.Run();
        }
    }
}
