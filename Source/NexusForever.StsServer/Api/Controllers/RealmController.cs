using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NexusForever.Shared.Database.Auth.Model;
using NLog;

namespace NexusForever.StsServer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RealmController : Controller
    {
        public AuthContext Database { get; }
        private static ILogger Logger { get; } = LogManager.GetCurrentClassLogger();

        public RealmController(AuthContext database)
        {
            Database = database;
        }

        [HttpGet]
        public async Task<IActionResult> GetRealms()
        {
            var result = await Database.Server.ToListAsync();
            return Ok(new { realms = result });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRealm(byte id)
        {
            var realm = await Database.Server.FindAsync(id);
            if (realm == null) return NotFound();
            return Ok(realm);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRealm(Server server)
        {
            using (var transaction = Database.Database.BeginTransaction(IsolationLevel.RepeatableRead))
            {
                if (await Database.Server.Where(i => i.Name == server.Name).AnyAsync())
                {
                    return Conflict();
                }

                if (server.Host == null)
                {
                    server.Host = HttpContext.Connection.RemoteIpAddress.ToString();
                }

                Database.Server.Add(server);
                await Database.SaveChangesAsync().ConfigureAwait(false);
                await Database.Entry(server).ReloadAsync().ConfigureAwait(false);
                transaction.Commit();
                return Ok(server);
            }
        }

        [HttpPatch("{id}")]
        [HttpPost("{id}")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRealm(byte id, [FromBody] JObject data)
        {
            using (var transaction = Database.Database.BeginTransaction(IsolationLevel.RepeatableRead))
            {
                // This allows delta patching. Possibly for updating server status/population data in the future.
                var server = await Database.Server.FindAsync(id).ConfigureAwait(false);
                if (server == null) return NotFound();
                JsonConvert.PopulateObject(data.ToString(), server);
                // But we're not allowed to change the ID.
                if (server.Id != id) return Forbid();
                await Database.SaveChangesAsync().ConfigureAwait(false);
                transaction.Commit();
                return Ok(server);
            }
        }
    }
}