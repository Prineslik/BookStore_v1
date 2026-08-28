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
using BookStore.Infrastructure;
using BookStore.Infrastructure.Mappings;
using BookStore.Infrastructure.Models;
using BookStore.Infrastructure.Repositories;
using BookStore.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
var jwtOptions = builder.Configuration.GetSection(nameof(JwtOptions));

builder.Services.AddControllers();

builder.Services.Configure<JwtOptions>(jwtOptions);
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

    // Если у вас есть другие профили, добавьте их здесь:
    // cfg.AddProfile<AnotherMappingProfile>();
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

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
