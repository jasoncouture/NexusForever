﻿using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using NexusForever.Shared.Configuration;

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
            var connectionString = databaseConfiguration.GetConnectionString(databaseType);
            switch (connectionString.Provider)
            {
                case DatabaseProvider.MySql:
                    optionsBuilder.UseMySql(connectionString.ConnectionString);
                    break;
                case DatabaseProvider.MicrosoftSqlServer:
                    optionsBuilder.UseSqlServer(connectionString.ConnectionString);
                    break;
                default:
                    throw new NotSupportedException($"The requested database provider: {connectionString.Provider:G} is not supported.");
            }

            return optionsBuilder;
        }

        private const string MySqlProvider = "Pomelo.EntityFrameworkCore.MySql";
        private const string MicrosoftSqlProvider = "Microsoft.EntityFrameworkCore.SqlServer";
        public static bool IsMySql(this MigrationBuilder migrationBuilder)
        {
            return string.Equals(migrationBuilder.ActiveProvider, MySqlProvider, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsMicrosoftSql(this MigrationBuilder migrationBuilder)
        {
            return string.Equals(migrationBuilder.ActiveProvider, MicrosoftSqlProvider, StringComparison.OrdinalIgnoreCase);
        }
    }


}
