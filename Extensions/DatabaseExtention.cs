using Microsoft.EntityFrameworkCore;
using Npgsql;
using UrlShorter.Data;
using UrlShorter.Modules.Users.Infrastructure.Enums;

namespace UrlShorter.Extensions;

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