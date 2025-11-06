using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PayingGuest.Application.Interfaces;

namespace PayingGuest.Api.Middleware
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TokenValidationMiddleware> _logger;
        private readonly IServiceProvider _serviceProvider;

        public TokenValidationMiddleware(
            RequestDelegate next,
            ILogger<TokenValidationMiddleware> logger,
            IServiceProvider serviceProvider)
        {
            _next = next;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip token validation for Swagger endpoints
            if (context.Request.Path.StartsWithSegments("/swagger") ||
                context.Request.Path.Value == "/" ||
                context.Request.Path.StartsWithSegments("/api/Token/token") || context.Request.Path.StartsWithSegments("/api/Auth/authtoken"))
            {
                await _next(context);
                return;
            }

            var token = ExtractTokenFromHeader(context);

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("No token found in request");
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Authorization token is required");
                return;
            }

            using (var scope = _serviceProvider.CreateScope())
            {
                var identityService = scope.ServiceProvider.GetRequiredService<IIdentityService>();

                var validationResult = await identityService.ValidateTokenAsync(token);

                if (!validationResult.Valid)
                {
                    _logger.LogWarning("Invalid token: {Error}", validationResult.Error);
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync($"Invalid token: {validationResult.Error}");
                    return;
                }

                // Add claims to context
                if (validationResult.ClientId != null)
                {
                    context.Items["ClientId"] = validationResult.ClientId;
                }

                if (validationResult.Subject != null)
                {
                    context.Items["Subject"] = validationResult.Subject;
                }

                if (validationResult.Scopes != null)
                {
                    context.Items["Scopes"] = validationResult.Scopes;
                }
            }

            await _next(context);
        }

        private string? ExtractTokenFromHeader(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authHeader))
            {
                return null;
            }

            const string bearerPrefix = "Bearer ";
            if (authHeader.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return authHeader.Substring(bearerPrefix.Length).Trim();
            }

            return authHeader;
        }
    }
}