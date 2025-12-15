using FitHubBackendAPI.Data;
using AutoMapper;
using FitHubBackendAPI.Repository.Implementation;
using FitHubBackendAPI.Repository.Interfaces;
using FitHubBackendAPI.Services.Implementation.AdminServices;
using FitHubBackendAPI.Services.Implementation.AuthServices;
using FitHubBackendAPI.Services.Implementation.GymServices;
using FitHubBackendAPI.Services.Interfaces;
using FitHubBackendAPI.Services.Interfaces.AuthServices;
using FitHubBackendAPI.Services.Interfaces.GymBranch;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using FitHubBackendAPI.Profiles;

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

            //builder.Services.AddAutoMapper(typeof(GymPlanProfile));
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<GymPlanProfile>();
            });

            IMapper mapper = mapperConfig.CreateMapper();
            builder.Services.AddSingleton(mapper);
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IGymBranchService, GymBranchService>();
            builder.Services.AddScoped<IPlanService, PlanService>();
            builder.Services.AddScoped<IOwnerToStaffService, OwnerToStaffService>();
            //register automapper

            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IEmailService, EmailService>();

            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddScoped<IAdminOwnerService, AdminOwnerService>();
            builder.Services.AddScoped<IAdminUserService, AdminUserService>();


            // ===============================
            // 3) Add AutoMapper
            // ===============================
            //builder.Services.AddAutoMapper(typeof(Program));

            // disable automatic 400 ProblemDetails so we can return our custom ResponseViewModel
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            // register controllers and add the ModelStateValidationFilter globally
            builder.Services.AddControllers(options =>
            {
                // register the filter globally so it applies to all controllers/actions
                options.Filters.Add<ModelStateValidationFilter>();
            });

            // ===============================
            // 4) Add FluentValidation
            // ===============================
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

            // ===============================
            // 5) Add Controllers
            // ===============================
            builder.Services.AddControllers();

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
                        ValidateLifetime = false,
                        ValidateIssuerSigningKey = true,   
                        IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            )
                    };
                });

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

            //add polcys 
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
                options.AddPolicy("Owner", policy => policy.RequireRole("Owner"));
            });



            // ===============================
            // 8) Add Swagger
            // ===============================
            builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerGen(swagger =>
            {
                //This�is�to�generate�the�Default�UI�of�Swagger�Documentation����
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ASP.NET�8�Web�API",
                    Description = " ITI Projrcy"
                });
                //�To�Enable�authorization�using�Swagger�(JWT)����
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter�'Bearer'�[space]�and�then�your�valid�token�in�the�text�input�below.\r\n\r\nExample:�\"Bearer�eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                    new OpenApiSecurityScheme
                    {
                    Reference = new OpenApiReference
                    {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                    }
                    },
                    new string[] {}
                    }
                    });
            });

            var app = builder.Build();
            

            // ===============================
            // ENABLE Swagger
            // ===============================
            app.UseSwagger();
            app.UseSwaggerUI();

            // ===============================
            // Middlewares
            // ===============================
            app.UseMiddleware<GlobalExceptionMiddleware>();

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
