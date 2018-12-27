using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using NexusForever.Shared.Configuration;

namespace NexusForever.Shared.Database.Auth.Model
{
    public abstract class DesignTimeDbFactory<T> : IDesignTimeDbContextFactory<T> where T : DbContext
    {

        public T CreateDbContext(string[] args)
        {
            SharedConfiguration.Initialize("global.json");
            DatabaseConfig databaseConfiguration = SharedConfiguration.Configuration.GetSection("Database").Get<DatabaseConfig>();
            DatabaseManager.Initialise(databaseConfiguration);
            return GetInstance(args);
        }

        protected virtual T GetInstance(string[] args)
        {
            return Activator.CreateInstance<T>();
        }
    }
}