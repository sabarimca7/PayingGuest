
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using PayingGuest.Api.Middleware;
using PayingGuest.Application;
using PayingGuest.Application.Commands;
using PayingGuest.Application.Handlers;
using PayingGuest.Application.Interfaces;
using PayingGuest.Application.Queries;
using PayingGuest.Domain.Interfaces;
using PayingGuest.Infrastructure;
using PayingGuest.Infrastructure.Repositories;
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

    // ADD YOUR CONTACT SERVICE
    // -----------------------------
    services.AddScoped<IContactRepository, ContactRepository>();

    services.AddScoped<IBedRepository, BedRepository>();

    services.AddScoped<IRoomRepository, RoomRepository>();

    services.AddScoped<IFilterRepository, FilterRepository>();

    services.AddScoped<IDashboardRepository, DashboardRepository>();

    services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(typeof(GetDashboardHandler).Assembly));

    services.AddScoped<IRecentBookingRepository, RecentBookingRepository>();

    services.AddScoped<IRecentPaymentRepository, RecentPaymentRepository>();
    services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(GetRecentPaymentsQuery).Assembly)); ;

    services.AddScoped<ISystemOverviewRepository, SystemOverviewRepository>();

    services.AddScoped<IUserDashboardRepository, UserDashboardRepository>();
    services.AddScoped<IRoomAmenitiesRepository, RoomAmenitiesRepository>();

    services.AddScoped<IUpcomingPaymentRepository, UpcomingPaymentRepository>();

    services.AddScoped<IMaintenanceRepository, MaintenanceRepository>();

    services.AddScoped<IPaymentRepository, PaymentRepository>();

    services.AddHttpContextAccessor();

    services.AddScoped<IProfileRepository, ProfileRepository>();
    services.AddScoped<IBookingRepository, BookingRepository>();

    services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreatePaymentCommand).Assembly)
);



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
    app.UsePathBase("/PayingGuest"); // must match IIS alias

    app.UseStaticFiles();             // required for Swagger UI HTML, JS, CSS
    app.UseResponseCompression();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("./v1/swagger.json", "PayingGuest API v1");
        c.RoutePrefix = "swagger"; // HTML page at /PayingGuest/swagger
    });

    // Security headers, CORS, etc.
    app.UseHttpsRedirection();
    app.UseCors("PayingGuestCorsPolicy");

    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseRouting();
    app.MapControllers();

    // Test endpoint
    app.MapGet("/", () => "PayingGuest API is running!");
}



// Make Program class public for integration tests
public partial class Program { }