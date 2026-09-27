using BookStore.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using System;
using System.Collections.Generic;
using System.Text;
using Testcontainers.PostgreSql;
using Xunit;

namespace BookStore.Tests.Integration
{
    public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        //private static readonly string scriptPath = Path.Combine(AppContext.BaseDirectory, "Integration", "Scripts", "init-test-data.sql");

        private readonly PostgreSqlContainer _dbContainer =
        new PostgreSqlBuilder("postgres:17-alpine")
            .WithDatabase("bookstore_test")
            .WithUsername("postgres")
            .WithPassword("test123")
            //.WithBindMount(scriptPath, "/docker-entrypoint-initdb.d/init.sql")
            .Build();
        private Respawner _respawner = default!;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                // Удаляем реальную регистрацию DbContext
                var descriptor = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(DbContextOptions<BookStoreDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Добавляем DbContext с подключением к контейнеру
                services.AddDbContext<BookStoreDbContext>(options =>
                    options.UseNpgsql(_dbContainer.GetConnectionString()));
            });
        }

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();

            using var scope = Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BookStoreDbContext>();

            Console.WriteLine($"[DEBUG] Init connection: {context.Database.GetConnectionString()}");
            
            await context.Database.MigrateAsync();

            var scriptPath = Path.Combine(
                AppContext.BaseDirectory, "Integration", "Scripts", "init-test-data.sql");

            var scriptContent = await File.ReadAllTextAsync(scriptPath);
            await context.Database.ExecuteSqlRawAsync(scriptContent);

            await using var connection = new NpgsqlConnection(_dbContainer.GetConnectionString());
            await connection.OpenAsync();

            var count = await context.Users.CountAsync();
            Console.WriteLine($"[DEBUG] Users count after script: {count}");

            _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = new[] { "public" },
                TablesToIgnore = new Respawn.Graph.Table[] { "__EFMigrationsHistory" }
            });
        }

        public async Task ResetDatabaseAsync()
        {
            using var connection = new NpgsqlConnection(_dbContainer.GetConnectionString());
            await connection.OpenAsync();
            await _respawner.ResetAsync(connection);

            var scriptPath = Path.Combine(
                AppContext.BaseDirectory, "Integration", "Scripts", "init-test-data.sql");
            using var scope = Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BookStoreDbContext>();
            var scriptContent = await File.ReadAllTextAsync(scriptPath);
            await context.Database.ExecuteSqlRawAsync(scriptContent);
        }

        public new async Task DisposeAsync()
        {
            await _dbContainer.DisposeAsync();
        }

       
    }
}
