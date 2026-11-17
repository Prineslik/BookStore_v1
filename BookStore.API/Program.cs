using AutoMapper;
using BookStore.API.Extensions;
using BookStore.API.Middleware;
using BookStore.API.Validators.Books;
using BookStore.API.Validators.Permissions;
using BookStore.API.Validators.Roles;
using BookStore.API.Validators.Users;
using BookStore.Application.Authorization;
using BookStore.Application.Interfaces;
using BookStore.Application.Interfaces.Books;
using BookStore.Application.Interfaces.Permissions;
using BookStore.Application.Interfaces.Roles;
using BookStore.Application.Interfaces.Users;
using BookStore.Application.Mappings;
using BookStore.Application.Services;
using BookStore.Infrastructure;
using BookStore.Infrastructure.Mappings;
using BookStore.Infrastructure.Models;
using BookStore.Infrastructure.Repositories;
using BookStore.Infrastructure.Repositories.Book;
using BookStore.Infrastructure.Repositories.Permission;
using BookStore.Infrastructure.Repositories.Role;
using BookStore.Infrastructure.Repositories.User;
using BookStore.Infrastructure.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

bool isDevelopment = builder.Environment.IsDevelopment();

var jwtOptions = builder.Configuration.GetSection(nameof(JwtOptions));
var corsOptions = builder.Configuration.GetSection("CorsSettings").Get<CorsSettings>();

var corsSection = builder.Configuration.GetSection("CorsSettings");
Console.WriteLine($"[DEBUG] CorsSettings exists: {corsSection.Exists()}");
Console.WriteLine($"[DEBUG] corsOptions is null: {corsOptions == null}");   

builder.Services.AddConfiguredCors(corsOptions);
builder.Services.AddConfiguredSecurityHeaders(isDevelopment);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddFluentValidation();

builder.Services.AddControllers();

//builder.Services.Configure<JwtOptions>(jwtOptions);
builder.Services.AddApiAuthentication(builder.Configuration);
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRepositories();
builder.Services.AddServices();

builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddAuthorization();

builder.Services.AddDbContext<BookStoreDbContext>(
    options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(BookStoreDbContext)));
    });

builder.Services.AddAutoMapper(cfg =>
{
    //cfg.AddProfile<InfrastructureMappingProfile>();
    cfg.AddProfile<ApplicationMappingProfile>();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseSecurityHeaders("Development");
else
    app.UseSecurityHeaders("Production");

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
    app.UseCors("Development");
else
    app.UseCors("Production");

app.UseSerilogRequestLogging();

//проверка маппинга
var mapperConfig = app.Services.GetRequiredService<IMapper>();
mapperConfig.ConfigurationProvider.AssertConfigurationIsValid();

if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
