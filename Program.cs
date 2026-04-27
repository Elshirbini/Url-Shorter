using Microsoft.EntityFrameworkCore;
using UrlShorter.Data;
using StackExchange.Redis;
using UrlShorter.Common.Services;
using UrlShorter.Modules.Auth;
using UrlShorter.Common.Middlewares;
using UrlShorter.Common.Emails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using UrlShorter.Common.Security;
using UrlShorter.Modules.Users;
using UrlShorter.Modules.Categories;

var builder = WebApplication.CreateBuilder(args);

// ✅ Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var options = new ConfigurationOptions
    {
        EndPoints = { "redis-14739.c326.us-east-1-3.ec2.cloud.redislabs.com:14739" },
        Password = "I1dFZmeu9DIHLySOueXDXpOoTl04QH60",
        User = "default",
    };

    return ConnectionMultiplexer.Connect(options);
});

// ✅ DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// ✅ Controllers
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
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
builder.Services.AddScoped<RedisService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddHttpClient<IEmailService, EmailService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

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
        IssuerSigningKey = new SymmetricSecurityKey(key)
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


var app = builder.Build();

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

// Auth
app.UseAuthentication();
app.UseAuthorization();


// ✅ Routing
app.MapControllers();


app.MapFallback(async context =>
{
    context.Response.StatusCode = 404;
    context.Response.ContentType = "application/json";

    await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
    {
        success = false,
        message = "Route not found"
    }));
});

app.Run();