using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Persistence.DatabaseContext;
using Persistence.Repositories;

namespace Persistence
{
    public static class PersistenceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
        {
            const string sqlDbConnection = "Server=localhost;Database=master;Trusted_Connection=True;Encrypt=False;";
            const string redisConnectionString = "localhost:6379";

            services.AddDbContext<TableContext>(opt => opt.UseSqlServer(sqlDbConnection));
            services.AddScoped<ITableSpecificationRepository, TableSpecificationRepository>();

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configurationOptions = ConfigurationOptions.Parse(redisConnectionString, true);
                return ConnectionMultiplexer.Connect(configurationOptions);
            });
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "YourAppName:";
            });

            return services;
        }
    }
}
