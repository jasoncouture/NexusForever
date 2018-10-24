using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NexusForever.Shared.Configuration;

namespace NexusForever.Shared.Database
{
    public static class ContextManager
    {
        public static void Initialise<TContext>(DatabaseConfig config) where TContext : DbContext, new()
        {
            using (var context = new TContext())
                Initialise(context, config);
        }
        public static void Initialise<TContext>(TContext context, DatabaseConfig config) where TContext : DbContext
        {
            var pendingMigrations = context.Database.GetPendingMigrations().ToList();
            if(pendingMigrations.Count == 0) return;
            if(!config.MigrateToLatestVersion) throw new InvalidOperationException("The database version does not match the expected version, and database upgrades are disabled. Cannot continue.");
            context.Database.Migrate();
            if(!config.SeedDatabase) return;
            // This is here because I see seeded data in the existing scripts.
            // and may need to manually seed data per-context. Maybe it isn't necessary, but I want to be ready.
            if (context is ISeed<TContext> seeder)
                seeder.Seed(context);
        }
    }
}
