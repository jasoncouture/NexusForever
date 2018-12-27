namespace NexusForever.Shared.Configuration
{
    public class DatabaseConnectionString : IConnectionString
    {
        public bool EnableMigrations { get; set; }
        public DatabaseProvider Provider { get; set; }
        public string ConnectionString { get; set; }
        public bool Log { get; set; } = true;
    }
}
