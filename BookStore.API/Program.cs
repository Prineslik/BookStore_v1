using AutoMapper;
using BookStore.API.Extensions;
using BookStore.API.Middleware;
using BookStore.API.Providers;
using BookStore.Application.Authorization.Handlers;
using BookStore.Application.Interfaces;
using BookStore.Application.Interfaces.Books;
using BookStore.Application.Interfaces.Permissions;
using BookStore.Application.Interfaces.Roles;
using BookStore.Application.Interfaces.Users;
using BookStore.Application.Services;
using BookStore.Application.Validators.Permissions;
using BookStore.Application.Validators.Roles;
using BookStore.Application.Validators.Users;
using BookStore.Infrastructure;
using BookStore.Infrastructure.Mappings;
using BookStore.Infrastructure.Models;
using BookStore.Infrastructure.Repositories;
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

builder.Services.AddValidatorsFromAssemblyContaining<LoginUserRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UsersRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RolesRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PermissionsRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddControllers();

//builder.Services.Configure<JwtOptions>(jwtOptions);
builder.Services.AddApiAuthentication(builder.Configuration);

//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IBooksRepository, BooksRepository>();
builder.Services.AddScoped<IBooksService, BooksService>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IRolesRepository, RolesRepository>();
builder.Services.AddScoped<IRolesService, RolesService>();
builder.Services.AddScoped<IPermissionsRepository, PermissionsRepository>();
builder.Services.AddScoped<IPermissionsService, PermissionsService>();

builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();

//builder.Services.AddValidatorsFromAssemblyContaining<UsersRequestValidator>();
//builder.Services.AddValidatorsFromAssemblyContaining<LoginUserRequestValidator>();

builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

builder.Services.AddDbContext<BookStoreDbContext>(
    options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(BookStoreDbContext)));
    });

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<InfrastructureMappingProfile>();
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
