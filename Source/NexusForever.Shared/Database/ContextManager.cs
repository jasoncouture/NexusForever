using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NexusForever.Shared.Configuration;
using NLog;

namespace NexusForever.Shared.Database
{
    public static class ContextManager
    {
        private static ILogger Logger { get; } = LogManager.GetCurrentClassLogger();
        public static void Initialise<TContext>(DatabaseConfig config) where TContext : DbContext, new()
        {
            using (var context = new TContext())
                Initialise(context, config);
        }
        public static void Initialise<TContext>(TContext context, DatabaseConfig config) where TContext : DbContext
        {

            var pendingMigrations = context.Database.GetPendingMigrations().ToList();
            if (pendingMigrations.Count != 0) HandleMigration(context, config);
            if (config.SeedDatabase) HandleSeeding(context, config);
        }

        private static void HandleSeeding<TContext>(TContext context, DatabaseConfig config) where TContext : DbContext
        {
            Logger.Info($"Seeding database: {context.Database.GetDbConnection().Database}");
            // This is here because I see seeded data in the existing scripts.
            // and may need to manually seed data per-context. Maybe it isn't necessary, but I want to be ready.
            if (context is ISeed<TContext> seeder)
                seeder.Seed(context);
        }


        private static void HandleMigration<TContext>(TContext context, DatabaseConfig config) where TContext : DbContext
        {
            if (!config.MigrateToLatestVersion)
            {

                Logger.Fatal("Database is out of data, and database migrations are disabled.");
                throw new InvalidOperationException("The database version does not match the expected version, and database upgrades are disabled. Cannot continue.");
            }
            Logger.Info($"Upgrading database: {context.Database.GetDbConnection().Database}");
            context.Database.Migrate();
        }
    }
}
