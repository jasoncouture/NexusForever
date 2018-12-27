using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NexusForever.Shared.Configuration;
using NLog.Extensions.Logging;

namespace NexusForever.Shared
{
    public static class Extensions
    {
        public static byte[] ToByteArray(this string value)
        {
            return Enumerable.Range(0, value.Length / 2)
                .Select(x => Convert.ToByte(value.Substring(x * 2, 2), 16))
                .ToArray();
        }

        public static string ToHexString(this byte[] value)
        {
            return BitConverter.ToString(value).Replace("-", "");
        }

        public static DbContextOptionsBuilder UseConfiguration(this DbContextOptionsBuilder optionsBuilder, IDatabaseConfiguration databaseConfiguration, DatabaseType databaseType)
        {
            IConnectionString connectionString = databaseConfiguration.GetConnectionString(databaseType);
            string migrationsAssemblyName = GetMigrationAssemblyName(connectionString, databaseType);
            switch (connectionString.Provider)
            {
                case DatabaseProvider.MySql:
                    if (connectionString.EnableMigrations)
                        optionsBuilder.UseMySql(connectionString.ConnectionString, x => x.MigrationsAssembly(migrationsAssemblyName));
                    else
                        optionsBuilder.UseMySql(connectionString.ConnectionString);
                    break;
                default:
                    throw new NotSupportedException($"The requested database provider: {connectionString.Provider:G} is not supported.");
            }

            if (connectionString.Log)
            {
                optionsBuilder.UseLoggerFactory(new NLogLoggerFactory());
            }

            return optionsBuilder;
        }

        private static string GetMigrationAssemblyName(IConnectionString connectionString, DatabaseType databaseType)
        {
            return $"NexusForever.Migrations.{connectionString.Provider:G}.{databaseType:G}";
        }
    }
}
