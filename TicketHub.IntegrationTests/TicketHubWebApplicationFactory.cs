using TicketHub.API.Models;
using TicketHub.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Data.Common;

namespace TicketHub.IntegrationTests
{
    public class TicketHubWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _dbName = Guid.NewGuid().ToString();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove SQL Server configuration
                services.RemoveAll(typeof(DbContextOptions<ApplicationContext>));
                services.RemoveAll(typeof(DbContextOptions));
                services.RemoveAll(typeof(DbConnection));

                // Override DB options
                services.AddScoped<DbContextOptions<ApplicationContext>>(sp =>
                {
                    return new DbContextOptionsBuilder<ApplicationContext>()
                        .UseInMemoryDatabase(_dbName)
                        .UseApplicationServiceProvider(sp)
                        .Options;
                });

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "TestScheme";
                    options.DefaultChallengeScheme = "TestScheme";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });
            });
        }

        // helper to ensure tests run against a clean in-memory database
        public async Task EnsureCleanDatabaseAsync()
        {
            using var scope = Services.CreateScope();
            var context = scope.ServiceProvider.GetService<ApplicationContext>();

            if (context == null)
                throw new Exception("Something went wrong finding the context...");

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }
    }
}