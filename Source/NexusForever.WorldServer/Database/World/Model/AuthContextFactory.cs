using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NexusForever.WorldServer.Database.World.Model
{
    public class AuthContextFactory : IDesignTimeDbContextFactory<WorldContext>
    {
        public WorldContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<WorldContext>();
            optionsBuilder.UseSqlServer(
                "Server=(localdb)\\mssqllocaldb;Database=NexusForeverWorld;Trusted_Connection=True;MultipleActiveResultSets=true");
            return new WorldContext(optionsBuilder.Options);
        }
    }
}