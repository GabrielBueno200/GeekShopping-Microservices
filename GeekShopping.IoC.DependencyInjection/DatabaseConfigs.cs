using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GeekShopping.IoC.DependencyInjection;

public static class DatabaseConfigs
{
    public static DbContextOptionsBuilder<TContext> AddDatabaseConfigs<TContext>(this IServiceCollection services, IConfiguration configuration, bool useDbContextOptionsBuilder = false)
        where TContext : DbContext
    {
        var connectionString = configuration["DatabaseConnection:MySQLConnectionString"];
        var serverVersion = ServerVersion.AutoDetect(connectionString);

        services.AddDbContext<TContext>(options =>
            options.UseMySql(connectionString, serverVersion)
        );

        if (useDbContextOptionsBuilder)
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<TContext>();
            dbContextOptionsBuilder.UseMySql(connectionString, serverVersion);
            return dbContextOptionsBuilder;
        }

        return null;
    }
}
