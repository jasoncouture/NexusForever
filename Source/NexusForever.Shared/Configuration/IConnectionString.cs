namespace NexusForever.Shared.Configuration
{
    public interface IConnectionString
    {
        bool EnableMigrations { get; }
        DatabaseProvider Provider { get; }
        string ConnectionString { get; }
        bool Log { get; }
    }
}
