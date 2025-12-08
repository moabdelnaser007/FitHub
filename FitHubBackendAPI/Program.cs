using FitHubBackendAPI.Data;
using FitHubBackendAPI.Repository.Implementation;
using FitHubBackendAPI.Repository.Interfaces;
using FitHubBackendAPI.Services.Implementation;
using FitHubBackendAPI.Services.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FitHubBackendAPI
{
    public class Program
    {
        public static void Main(string[] args)
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
            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = false,
                        ValidateLifetime = false
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

            app.Run();
        }
    }
}
