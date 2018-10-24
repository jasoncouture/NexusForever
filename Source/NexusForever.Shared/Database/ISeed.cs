using Microsoft.EntityFrameworkCore;

namespace NexusForever.Shared.Database
{
    public interface ISeed<in TContext> where TContext : DbContext
    {
        void Seed(TContext context);
    }
}
