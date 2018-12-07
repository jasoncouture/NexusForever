using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NexusForever.Shared.Configuration;

namespace NexusForever.Shared.Database.Auth.Model
{
    public class AuthContextFactory : IDbContextFactory<AuthContext>
    {
        public AuthContext Create(DbContextFactoryOptions options)
        {
            return null;
        }
    }
}
