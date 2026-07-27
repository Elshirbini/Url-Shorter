using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using UrlShorter.Data;
using StackExchange.Redis;
using UrlShorter.Common.Redis;
using UrlShorter.Common.Middlewares;
using UrlShorter.Common.Emails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using UrlShorter.Common.Security;
using Serilog;
using System.Threading.RateLimiting;
using UrlShorter.Modules.Links.Application.UseCases;
using UrlShorter.Modules.Links.Infrastructure.Repositories;
using UrlShorter.Modules.Links.Application.Interfaces;
using UrlShorter.Modules.Categories.Application.Interfaces;
using UrlShorter.Modules.Categories.Infrastructure.Repositories;
using UrlShorter.Common.Formatters;
using UrlShorter.Modules.Categories.Application.UseCases;
using UrlShorter.Modules.Users.Application.Interfaces;
using UrlShorter.Modules.Users.Infrastructure.Repositories;
using UrlShorter.Modules.Users.Application.UseCases;
using UrlShorter.Modules.Auth.Infrastructure.Repositories;
using UrlShorter.Modules.Auth.Application.Interfaces;
using UrlShorter.Modules.Auth.Application.UseCases;
using UrlShorter.Extensions;
using UrlShorter.Common.Storage;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Features;
using Hangfire;
using Hangfire.PostgreSql;
using UrlShorter.Modules.Auth.Jobs;
using UrlShorter.Common.Messaging.Extensions;
using Asp.Versioning;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;



Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(new PrettyJsonFormatter()).WriteTo.File(
        new PrettyJsonFormatter(),
        "logs/app-.log",
        rollingInterval: RollingInterval.Day,
        rollOnFileSizeLimit: true,
        fileSizeLimitBytes: 20 * 1024 * 1024, //20MB
        retainedFileCountLimit: 30)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5000") // allowed origin
              .AllowCredentials()                   // allow cookies
              .AllowAnyMethod()                     // GET, POST, etc.
              .AllowAnyHeader();                    // headers
    });
});

builder.Services.AddRateLimiter(options =>
{
    // Default API Policy
    options.GlobalLimiter =
        PartitionedRateLimiter.Create<HttpContext, string>(
            httpContext =>
            {
                var ip =
                    httpContext.Connection.RemoteIpAddress?.ToString()
                    ?? "unknown";

                return RateLimitPartition.GetSlidingWindowLimiter(
                    partitionKey: ip,
                    factory: _ => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1),
                        SegmentsPerWindow = 6,
                        QueueLimit = 0
                    });
            });

    // Auth Policy
    options.AddPolicy("auth", httpContext =>
    {
        var ip =
            httpContext.Connection.RemoteIpAddress?.ToString()
            ?? "unknown";

        return RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: ip,
            factory: _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 6,

                QueueLimit = 0,
                AutoReplenishment = true
            });
    });
});

// ✅ Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetSection("Redis:ConnectionString").Value;

    return ConnectionMultiplexer.Connect(connectionString!);
});

// ✅ Database
builder.Services.AddDatabase(builder.Configuration);


// MQ
builder.Services.AddMessaging(builder.Configuration);

// Hangfire
builder.Services.AddHangfire(config =>
{
    config.UsePostgreSqlStorage(options =>
    {
        options.UseNpgsqlConnection(
            builder.Configuration.GetConnectionString("DefaultConnection"));
    });
});

builder.Services.AddHangfireServer();

builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

// ✅ Controllers
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });


builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.Configure<FormOptions>(options =>
{
    options.MemoryBufferThreshold = 5 * 1024 * 1024;
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .SelectMany(x => x.Value!.Errors.Select(e => new
            {
                field = x.Key,
                message = e.ErrorMessage
            }))
            .ToList();

        return new BadRequestObjectResult(new
        {
            success = false,
            message = "Validation failed",
            errors
        });
    };
});


// Services
builder.Services.AddScoped<IRedisClient, RedisClient>();
builder.Services.AddHttpClient<IEmailService, EmailService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddStorage(builder.Configuration);
// repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IClickRepository, ClickRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ILinkRepository, LinkRepository>();
// Link use cases
builder.Services.AddScoped<RedirectLinkUseCase>();
builder.Services.AddScoped<CreateLinkUseCase>();
builder.Services.AddScoped<UpdateLinkUseCase>();
builder.Services.AddScoped<DeleteLinkUseCase>();
builder.Services.AddScoped<GetLinkByIdUseCase>();
builder.Services.AddScoped<GetAllLinksUseCase>();
// category use cases
builder.Services.AddScoped<CreateCategoryUseCase>();
builder.Services.AddScoped<UpdateCategoryUseCase>();
builder.Services.AddScoped<DeleteCategoryUseCase>();
builder.Services.AddScoped<GetAllCategoriesUseCase>();
// user use cases 
builder.Services.AddScoped<GetUserUseCase>();
builder.Services.AddScoped<UpdateUserProfileUseCase>();
builder.Services.AddScoped<ResetPasswordUseCase>();
// auth use cases
builder.Services.AddScoped<LoginUseCase>();
builder.Services.AddScoped<SignupUseCase>();
builder.Services.AddScoped<LogoutUseCase>();
builder.Services.AddScoped<VerifyEmailUseCase>();
builder.Services.AddScoped<ForgetPasswordUseCase>();
builder.Services.AddScoped<VerifyCodeUseCase>();
builder.Services.AddScoped<NewPasswordUseCase>();
builder.Services.AddScoped<RefreshTokenUseCase>();

// ✅ Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// 🔐 JWT CONFIG
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        RoleClaimType = "role"
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["accessToken"];
            return Task.CompletedTask;
        },

        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("❌ Auth Failed: " + context.Exception.Message);
            return Task.CompletedTask;
        },

        OnForbidden = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        context.Response.ContentType = "application/json";

        return context.Response.WriteAsJsonAsync(new
        {
            success = false,
            message = "Forbidden. You don't have permission to access this resource."
        });
    },

        OnChallenge = context =>
        {
            context.HandleResponse();

            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";

            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = "Unauthorized"
            });

            return context.Response.WriteAsync(result);
        }
    };
});

// Compression providers
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;

    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});


var app = builder.Build();

// Use CORS middleware
app.UseCors("MyPolicy");

using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var redis = scope.ServiceProvider.GetRequiredService<IConnectionMultiplexer>();

        // DB check
        var dbOk = db.Database.CanConnect();

        // Redis check
        var ping = redis.GetDatabase().Ping();

        Console.WriteLine($"[HEALTH] DB Connected: {dbOk}");
        Console.WriteLine($"[HEALTH] Redis Ping: {ping.TotalMilliseconds} ms");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[HEALTH ERROR] {ex.Message}");
    }
}

// ✅ Swagger في dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ Middlewares
app.UseMiddleware<RequestLoggingMiddleware>();
// app.UseHttpsRedirection();

// ✅ Global Exception Handling
app.UseMiddleware<ExceptionMiddleware>();


// Response Compression
app.UseResponseCompression();

// Auth
app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

// Hangfire dashboard
app.UseHangfireDashboard("/hangfire");

// Register Recurring Jobs
RecurringJob.AddOrUpdate<CleanupRefreshTokensJob>(
    "cleanup-refresh-tokens",
    job => job.ExecuteAsync(CancellationToken.None),
    Cron.Daily());

// ✅ Routing
app.MapControllers();



app.Run();