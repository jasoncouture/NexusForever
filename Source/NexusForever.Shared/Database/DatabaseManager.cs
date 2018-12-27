using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using NexusForever.Shared.Configuration;

namespace NexusForever.Shared.Database
{
    public static class DatabaseManager
    {
        public static DatabaseConfig Config { get; private set; }

        public static void Initialise(DatabaseConfig config)
        {
            Config = config;
        }

        public static void Migrate<T>() where T : DbContext, new()
        {
            DatabaseType databaseType = GetDatabaseTypeEnumFromType(typeof(T));
            IConnectionString connectionString = ((IDatabaseConfiguration) Config).GetConnectionString(databaseType);
            if(!connectionString.EnableMigrations)
                return;
            T context = new T();
            context.Database.Migrate();
            MethodInfo seedMethod = typeof(T).GetMethod("Seed", BindingFlags.Instance | BindingFlags.Public, Type.DefaultBinder, Type.EmptyTypes, null);
            if (seedMethod != null)
                seedMethod.Invoke(context, new object[0]);
        }

        private static DatabaseType GetDatabaseTypeEnumFromType(Type type)
        {
            switch (type.Name)
            {
                case "WorldContext":
                    return DatabaseType.World;
                case "AuthContext":
                    return DatabaseType.Auth;
                case "CharacterContext":
                    return DatabaseType.Character;
            }

            throw new InvalidOperationException($"Unsupported database type: {type.FullName}");
        }


    }
}
