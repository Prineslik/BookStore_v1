using BookStore.Application.Authorization.Attributes;
using BookStore.Infrastructure.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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

        public static void AddConfiguredCors(this IServiceCollection services, CorsSettings corsSettings)
        {
            
            services.AddCors(options =>
            {
                options.AddPolicy("Production", policy =>
                {
                    policy.WithOrigins(corsSettings.AllowedOrigins)
                        .WithMethods(corsSettings.AllowedMethods)
                        .WithHeaders(corsSettings.AllowedHeaders)
                        .AllowCredentials();
                });

                options.AddPolicy("Development", policy =>
                {
                    policy.WithOrigins("http://localhost:5173")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });
        }

        public static void AddConfiguredSecurityHeaders(this IServiceCollection services, bool isDevelopment)
        {
            services.AddSecurityHeaderPolicies(options =>
            {
                options.AddPolicy("Development", policy =>
                {
                    policy.AddDefaultApiSecurityHeaders();

                    policy.AddContentSecurityPolicy(builder =>
                    {
                        builder.AddDefaultSrc().Self();

                        builder.AddScriptSrc()
                            .Self()
                            .UnsafeInline()
                            .From("https://cdn.jsdelivr.net");

                        builder.AddStyleSrc()
                            .Self()
                            .UnsafeInline()
                            .From("https://cdn.jsdelivr.net");

                        builder.AddImgSrc()
                            .Self()
                            .From("data:")
                            .From("https:");

                        builder.AddConnectSrc()
                            .Self()
                            .From("https://localhost:*")
                            /*.From("https://127.0.0.1:*")*/;

                        builder.AddFontSrc()
                            .Self()
                            .From("data:")
                            .From("https://cdn.jsdelivr.net");

                        builder.AddFormAction().Self();
                        builder.AddFrameAncestors().None();
                        builder.AddObjectSrc().None();
                        builder.AddBaseUri().None();
                    });

                    policy.AddCrossOriginEmbedderPolicy(builder => builder
                        .Credentialless());
                });

                options.AddPolicy("Production", policy =>
                 {
                     policy.AddDefaultApiSecurityHeaders();

                     policy.AddContentSecurityPolicy(builder =>
                     {
                         builder.AddDefaultSrc().None();
                         builder.AddFormAction().Self();
                         builder.AddFrameAncestors().None();
                         builder.AddObjectSrc().None();
                         builder.AddScriptSrc().None();
                         builder.AddStyleSrc().None();
                         builder.AddImgSrc().None();
                         builder.AddFontSrc().None();
                         builder.AddConnectSrc().Self();
                         builder.AddBaseUri().None();
                     });

                     policy.AddCrossOriginEmbedderPolicy(builder => builder
                         .RequireCorp());

                     policy.AddStrictTransportSecurity((int)TimeSpan.FromDays(365).TotalSeconds, true, false, String.Empty);
                 });
            });
        }
    }
}
