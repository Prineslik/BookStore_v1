using BookStore.Application.Authorization.Attributes;
using BookStore.Core.Enums;
using BookStore.Infrastructure.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic.FileIO;
using System.Text;

namespace BookStore.API.Extensions
{
    public static class ApiExtensions
    {

        public static void AddApiAuthentication(this IServiceCollection services,
    IConfiguration configuration)
        {
            var jwtSection = configuration.GetSection("JwtOptions");
            services.Configure<JwtOptions>(jwtSection);

            var jwtOptions = jwtSection.Get<JwtOptions>();

            if (jwtOptions == null || string.IsNullOrEmpty(jwtOptions.SecretKey))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[CRITICAL] AuthRegistration: JwtOptions или SecretKey не найдены в конфигурации!");
                Console.ResetColor();

                throw new InvalidOperationException("Невозможно запустить приложение без секретного ключа JWT.");
            }

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    var secretKey = configuration["JwtOptions:SecretKey"];

                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey( 
                            Encoding.UTF8.GetBytes(secretKey)
                        ?? throw new InvalidOperationException("SecretKey is missing")),
                        RoleClaimType = "Roles"
                    };

                    options.Events = new JwtBearerEvents 
                    {
                        OnMessageReceived = context =>
                        {
                            context.Token = context.Request.Cookies["RefreshToken"];

                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("JwtAuth");

                            logger.LogWarning(context.Exception, "Ошибка аутентификации JWT для IP: {RemoteIpAddress}",
                                context.HttpContext.Connection.RemoteIpAddress);

                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("JwtAuth");

                            // Извлекаем NameIdentifier (или любой другой claim) для красивого лога
                            var userId = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                            // Используем Debug, чтобы не засорять прод-логи каждого запроса уровнем Information
                            logger.LogDebug("Пользователь {UserId} успешно прошел JWT-аутентификацию", userId);

                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                {
                    policy.RequireRole("Admin");
                    //policy.RequireClaim("Admin", "true");
                    //policy.AddRequirements();
                });

                options.AddPolicy("UserOrAdmin", policy =>
                {
                    policy.RequireRole("Admin", "User");
                    //policy.RequireClaim("Admin", "true");
                    //policy.AddRequirements();
                });

                options.AddPolicy("CrudRole", policy =>
                {
                    policy.RequireRole("crud role");
                    
                    //policy.RequireClaim("Admin", "true");
                    //policy.AddRequirements();
                });

                //options.AddPolicy("PermissionBased", policy =>
                //{
                //    //policy.Requirements.Add(new RequirePermissionAttribute(""));
                //});
            });

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[Success] AuthRegistration: Аутентификация JWT успешно добавлена.");
            Console.ResetColor();
        }
    }
}
