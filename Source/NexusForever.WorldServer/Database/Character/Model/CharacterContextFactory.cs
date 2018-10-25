using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public class CharacterContextFactory : IDesignTimeDbContextFactory<CharacterContext>
    {
        public CharacterContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CharacterContext>();
            optionsBuilder.UseSqlServer(
                "Server=(localdb)\\mssqllocaldb;Database=NexusForeverCharacter;Trusted_Connection=True;MultipleActiveResultSets=true");
            return new CharacterContext(optionsBuilder.Options);
        }
    }
}