using Microsoft.EntityFrameworkCore;
using Npgsql;
using UrlShorter.src.Data;
using UrlShorter.src.Modules.Users.Infrastructure.Enums;

namespace UrlShorter.src.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")!;

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

        dataSourceBuilder.MapEnum<UserRole>();

        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(dataSource));

        return services;
    }
}