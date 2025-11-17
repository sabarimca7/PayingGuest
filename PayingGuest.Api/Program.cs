using Microsoft.OpenApi.Models;
using PayingGuest.Api.Middleware;
using PayingGuest.Application;
using PayingGuest.Infrastructure;
using Serilog;
using System.Text;
using System.Text.Json;

// Configure Serilog before building the host
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/bootstrap-.txt", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting PayingGuest API application");

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container
    ConfigureServices(builder.Services, builder.Configuration);

    var app = builder.Build();

    // Configure the HTTP request pipeline
    ConfigureMiddleware(app, app.Environment);

    
    Log.Information("PayingGuest API started successfully");
    Log.Information("Swagger UI available at: {SwaggerUrl}",
        app.Environment.IsDevelopment() ? "https://localhost:5001/swagger" : "{BaseUrl}/swagger");

    await app.RunAsync();
}
catch (Exception ex) when (ex.GetType().Name != "StopTheHostException")
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.Information("Shutting down PayingGuest API");
    await Log.CloseAndFlushAsync();
}

// Configure Services
static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Add basic services
    services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
            options.JsonSerializerOptions.WriteIndented = true;
            options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        });

    // Add API Explorer for Swagger
    services.AddEndpointsApiExplorer();

    // Add HttpContextAccessor
    services.AddHttpContextAccessor();

    
    // Configure Swagger/OpenAPI
    services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "PayingGuest API",
            Version = "v1",
            Description = "API for PayingGuest Management System with IdentityServer Integration",
            TermsOfService = new Uri("https://thoughtwalks.net/"),
            Contact = new OpenApiContact
            {
                Name = "PayingGuest Support",
                Email = "support@payingguest.com",
                Url = new Uri("https://thoughtwalks.net/")
            },
            License = new OpenApiLicense
            {
                Name = "ThoughtWalk",
                Url = new Uri("https://thoughtwalks.net/")
            }
        });

        // Add Bearer token authentication to Swagger
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Description = "JWT Authorization header using the Bearer scheme. Enter your token below (without 'Bearer' prefix)."
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "bearer",
                    Name = "Bearer",
                    In = ParameterLocation.Header
                },
                new List<string>()
            }
        });

        // Include XML comments if available
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }

        // Custom schema IDs to avoid conflicts
        options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));

        // Enable annotations
      //  options.EnableAnnotations();
    });

    // Add layer services using DI configuration
    services.AddApplication();
    services.AddInfrastructure(configuration);

    // Add Authentication & Authorization (if not already added in AddApiServices)
    services.AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured"))),
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(Microsoft.IdentityModel.Tokens.SecurityTokenExpiredException))
                    {
                        context.Response.Headers.Add("Token-Expired", "true");
                    }
                    Log.Error("Authentication failed: {Message}", context.Exception.Message);
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    Log.Debug("Token validated for user: {User}", context.Principal?.Identity?.Name);
                    return Task.CompletedTask;
                }
            };
        });

    services.AddAuthorization();

    // Add CORS
    services.AddCors(options =>
    {
        options.AddPolicy("PayingGuestCorsPolicy",
            policy =>
            {
                policy.WithOrigins("http://localhost:4200", "https://localhost:4200", "http://localhost:5000")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
    });
   
    // Add Response Compression
    services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
    });
}

// Configure Middleware Pipeline
static void ConfigureMiddleware(WebApplication app, IWebHostEnvironment env)
{
    // Enable response compression
    app.UseResponseCompression();

    // Development-specific middleware
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();

        // Enable Swagger in Development
        app.UseSwagger(options =>
        {
            options.SerializeAsV2 = false;
        });

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "PayingGuest API V1");
            options.RoutePrefix = "swagger"; // Swagger UI at /swagger

            // UI Customizations
            options.DocumentTitle = "PayingGuest API Documentation";
            options.DisplayRequestDuration();
            options.EnableDeepLinking();
            options.EnableFilter();
            options.ShowExtensions();
            options.ShowCommonExtensions();
            options.EnableValidator();
            options.EnableTryItOutByDefault();

            // Add custom CSS if needed
            options.InjectStylesheet("/swagger-ui/custom.css");

            // Collapse models by default
            options.DefaultModelsExpandDepth(-1);

            // Expand operations by default
            options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);

            // Enable syntax highlighting
            options.ConfigObject.AdditionalItems.Add("syntaxHighlight", true);
        });
    }
    else
    {
        // Production error handling
        app.UseExceptionHandler("/error");
        app.UseHsts();
    }

    // Security headers middleware
    app.Use(async (context, next) =>
    {
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Add("X-Frame-Options", "DENY");
        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
        context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; style-src 'self' 'unsafe-inline'; script-src 'self' 'unsafe-inline' 'unsafe-eval';");

        await next();
    });

    // Standard middleware
    app.UseHttpsRedirection();

    // Use CORS
    app.UseCors("PayingGuestCorsPolicy");

  
    // Serve static files (for custom Swagger CSS if needed)
    //app.UseStaticFiles();

    // Custom middleware
    //app.UseMiddleware<TokenValidationMiddleware>();
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    // Authentication & Authorization
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
}

// Make Program class public for integration tests
public partial class Program { }