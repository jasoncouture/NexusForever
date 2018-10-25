using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NexusForever.Shared.Database.Auth.Model
{
    public class AuthContextFactory : IDesignTimeDbContextFactory<AuthContext>
    {
        public AuthContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AuthContext>();
            optionsBuilder.UseSqlServer(
                "Server=(localdb)\\mssqllocaldb;Database=NexusForeverAuth;Trusted_Connection=True;MultipleActiveResultSets=true");
            return new AuthContext(optionsBuilder.Options);
        }
    }
}