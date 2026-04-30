using BookStore.Application.Services;
using BookStore.Infrastructure;
using BookStore.Infrastructure.Mappings;
using BookStore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using BookStore.Application.Interfaces.Books;
using BookStore.Application.Interfaces.Users;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IBooksRepository, BooksRepository>();
builder.Services.AddScoped<IBooksService, BooksService>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IUsersService, UsersService>();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
