using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PayingGuest.Application.Interfaces;
using PayingGuest.Domain.Entities;
using PayingGuest.Domain.Interfaces;
using PayingGuest.Infrastructure.Data;
using PayingGuest.Infrastructure.Repositories;
using PayingGuest.Infrastructure.Services;


namespace PayingGuest.Infrastructure
{
    /// <summary>
    /// Extension methods for setting up infrastructure layer services in DI container
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds infrastructure layer services to the dependency injection container
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The configuration</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
        {
            // Validate required configuration
            ValidateConfiguration(configuration);

            // Add Database Context
            services.AddDbContext<PayingGuestDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("PayingGuestDb");

                options.UseSqlServer(connectionString, sqlServerOptionsAction: sqlOptions =>
                {
                    // Enable retry on failure for transient errors
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);

                    // Set command timeout
                    sqlOptions.CommandTimeout(30);

                    // Enable split queries for better performance with includes
                    sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });

                // Add detailed logging and debugging in development
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                if (environment == "Development")
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                    options.LogTo(Console.WriteLine, LogLevel.Information);
                }
            });

            // Add Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRepository<Property>, Repository<Property>>();
            services.AddScoped<IRepository<AuditLog>, Repository<AuditLog>>();
            services.AddScoped<IRepository<ClientToken>, Repository<ClientToken>>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();

            // Add Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Add Memory Cache with configuration
            services.AddMemoryCache(options =>
            {
            //    options.SizeLimit = 1024; // Maximum number of entries
                options.CompactionPercentage = 0.25; // Compact 25% when size limit is reached
                options.ExpirationScanFrequency = TimeSpan.FromMinutes(5); // How often to scan for expired items
            });

            // Add Distributed Cache (can be replaced with Redis in production)
            services.AddDistributedMemoryCache();

            // Add Token Cache Service
            services.AddSingleton<ITokenCacheService, TokenCacheService>();
           
            services.AddHttpClient<IdentityService>(client =>
            {
                var baseUrl = configuration["IdentityServer:BaseUrl"]
                    ?? throw new InvalidOperationException("IdentityServer:BaseUrl not configured");
                client.BaseAddress = new Uri(baseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            // Register Infrastructure Services
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IDateTimeService, DateTimeService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IUserTokenRepository, UserTokenRepository>();
            services.AddScoped<ITokenService, TokenService>();

            // Add additional services
            services.AddScoped<IAuditService, AuditService>();

            return services;
        }

        /// <summary>
        /// Validates required configuration settings
        /// </summary>
        private static void ValidateConfiguration(IConfiguration configuration)
        {
            var requiredSettings = new[]
            {
                "ConnectionStrings:PayingGuestDb",
                "IdentityServer:BaseUrl",
                "IdentityServer:ClientId",
                "IdentityServer:ClientSecret"
            };

            foreach (var setting in requiredSettings)
            {
                if (string.IsNullOrEmpty(configuration[setting]))
                {
                    throw new InvalidOperationException($"Required configuration '{setting}' is missing.");
                }
            }
        }

    }

   
    
}