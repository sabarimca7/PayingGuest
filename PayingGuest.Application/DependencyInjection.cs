using FluentValidation;
using MediatR;
using MediatR.NotificationPublishers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PayingGuest.Application.Behaviors;
using PayingGuest.Application.Commands;
using PayingGuest.Application.Mappings;
using PayingGuest.Application.Validators;
using System.Reflection;

namespace PayingGuest.Application
{
    /// <summary>
    /// Extension methods for setting up application layer services in DI container
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds application layer services to the dependency injection container
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Get the executing assembly for registration
            var assembly = Assembly.GetExecutingAssembly();

            // Add AutoMapper with all profiles from the assembly
            services.AddAutoMapper(config =>
            {
                config.AddMaps(assembly);
                config.AddProfile<MappingProfile>();
            });

            // Add MediatR for CQRS pattern
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(assembly);

                // Configure MediatR behaviors
                config.NotificationPublisher = new TaskWhenAllPublisher();
                config.Lifetime = ServiceLifetime.Scoped;
            });

            // Add Pipeline Behaviors - Order matters!
            // Behaviors execute in the order they are registered
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));

            // Add FluentValidation
            services.AddValidatorsFromAssembly(assembly, ServiceLifetime.Scoped);
            // Register all validators in assembly
         //   services.AddValidatorsFromAssemblyContaining<RegisterUserCommandValidator>();
            // Or manually
            //services.AddScoped<IValidator<RegisterUserCommand>, RegisterUserCommandValidator>();

            return services;
        }
    }  

    /// <summary>
    /// Logs all MediatR requests
    /// </summary>
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var requestGuid = Guid.NewGuid().ToString();

            _logger.LogInformation(
                "Handling {RequestName} [{RequestGuid}] {@Request}",
                requestName, requestGuid, request);

            var response = await next();

            _logger.LogInformation(
                "Handled {RequestName} [{RequestGuid}]",
                requestName, requestGuid);

            return response;
        }
    }

    /// <summary>
    /// Monitors performance of MediatR requests
    /// </summary>
    public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
        private readonly System.Diagnostics.Stopwatch _timer;

        public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
            _timer = new System.Diagnostics.Stopwatch();
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            _timer.Start();

            var response = await next();

            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            if (elapsedMilliseconds > 500)
            {
                var requestName = typeof(TRequest).Name;

                _logger.LogWarning(
                    "Long Running Request: {RequestName} ({ElapsedMilliseconds} ms) {@Request}",
                    requestName, elapsedMilliseconds, request);
            }

            return response;
        }
    }


    
}