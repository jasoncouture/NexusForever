using Microsoft.AspNetCore.Mvc;
using NexusForever.Shared;
using NexusForever.Shared.Database.Auth.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NexusForever.Shared.Cryptography;
using NexusForever.Shared.Database.Auth;
using NexusForever.Shared.Database.Auth.Client;

namespace NexusForever.StsServer.Api.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private static ILogger Logger { get; } = LogManager.GetCurrentClassLogger();
        public AuthContext Database { get; }

        public AccountController(AuthContext database)
        {
            Database = database;
        }

        [HttpGet("find")]
        public async Task<IActionResult> Find(string email)
        {
            Account account = await Database.Account.FirstOrDefaultAsync(i => i.Email == email).ConfigureAwait(false);
            if (account == null) return NotFound();
            return Ok(new
            {
                account.Id
            });
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody]RegisterModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            using (var transaction = Database.Database.BeginTransaction(IsolationLevel.RepeatableRead))
            {
                if (await Database.Account.Where(i => i.Email.ToLower() == model.EMailAddress.ToLower()).AnyAsync())
                {
                    return Conflict();
                }

                var s = RandomProvider.GetBytes(16u);
                var v = Srp6Provider.GenerateVerifier(s, model.EMailAddress, model.Password);
                var accountEntry = Database.Account.Add(new Account()
                {
                    Email = model.EMailAddress,
                    S = s.ToHexString(),
                    V = v.ToHexString(),
                });
                await Database.SaveChangesAsync().ConfigureAwait(false);
                transaction.Commit();
                await accountEntry.ReloadAsync();
                return Ok(new
                {
                    accountEntry.Entity.Id
                });
            }
        }

        [HttpGet("token/game/{id}")]
        public async Task<IActionResult> GetGameTokenAsync(uint id)
        {
            var account = await Database.Account.FindAsync(id).ConfigureAwait(false);
            if (account == null) return NotFound();
            return Ok(new
            {
                account.GameToken
            });
        }

        [HttpGet("token/session/{id}")]
        public async Task<IActionResult> GetSessionTokenAsync(uint id)
        {
            var account = await Database.Account.FindAsync(id).ConfigureAwait(false);
            if (account == null) return NotFound();
            return Ok(new
            {
                account.SessionKey
            });
        }

        [HttpPost("token/game/{id}")]
        public async Task<IActionResult> GenerateGameToken(uint id)
        {
            var account = await Database.Account.FindAsync(id).ConfigureAwait(false);
            if (account == null) return NotFound();
            account.GameToken = Guid.NewGuid().ToByteArray().ToHexString();
            await Database.SaveChangesAsync().ConfigureAwait(false);
            return Ok(new
            {
                account.GameToken
            });
        }

        [HttpPost("token/session/{id}")]
        public async Task<IActionResult> GenerateSessionToken(uint id)
        {
            var account = await Database.Account.FindAsync(id).ConfigureAwait(false);
            if (account == null) return NotFound();
            account.SessionKey = RandomProvider.GetBytes(16u).ToHexString();
            await Database.SaveChangesAsync().ConfigureAwait(false);
            return Ok(new
            {
                account.SessionKey
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(uint id)
        {
            using (IDbContextTransaction transaction = Database.Database.BeginTransaction(IsolationLevel.RepeatableRead))
            {
                Account account = await Database.Account.FindAsync(id).ConfigureAwait(false);
                if (account == null) return NotFound();
                Database.Account.Remove(account);
                await Database.SaveChangesAsync();
                transaction.Commit();
                return NoContent();
            }
        }

    }
}
