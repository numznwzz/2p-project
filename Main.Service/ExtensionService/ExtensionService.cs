using Domain.Repository;
using Infrastructure.Evironments;
using Infrastructure.Repository;
using Mongo.Migration.Startup;
using Mongo.Migration.Startup.DotNetCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Serilog;

namespace Main.Service.ExtensionService
{
    internal static class ExtensionService
    {
        public static void AddMongoDb(this IServiceCollection services, IConfiguration configuration)
        {
            // convert property to camel case
            var conventionPack = new  ConventionPack {new CamelCaseElementNameConvention()};
            ConventionRegistry.Register("camelCase", conventionPack, t => true);

            // convert enum value to string
            var enumConventionPick = new ConventionPack{new EnumRepresentationConvention(BsonType.String)};
            ConventionRegistry.Register("EnumStringConvention", enumConventionPick, t => true);

            var environmentsConfig = new EnvironmentsConfig(configuration);
            var connectionString = environmentsConfig.GetConnectionString("MongoDb");

            if (string.IsNullOrEmpty(connectionString))
            {
                Log.Error($"MongoDb Conn:{connectionString}");
                Environment.Exit(0);
            }
            services.AddMigration(new MongoMigrationSettings()
            {
                ConnectionString =connectionString,
                Database = "2p-project"
            });
            
            services.AddSingleton<IMongoClient, MongoClient>(sp => new MongoClient(connectionString));
            services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));
        }
        
        internal static void AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            EnvironmentsConfig env = new EnvironmentsConfig(configuration);
            string conn = env.GetValue<string>("RedisConfig");
            string insName = env.GetValue<string>("RedisService");

            services.AddDistributedRedisCache(option =>
            {
                option.Configuration = conn;
                option.InstanceName = $"{insName}:";
            });
        }

    }
}