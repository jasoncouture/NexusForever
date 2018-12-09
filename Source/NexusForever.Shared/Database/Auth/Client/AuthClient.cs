using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.Database.Auth.Model;

namespace NexusForever.Shared.Database.Auth.Client
{
    public class AccountAlreadyExistsException : Exception
    {
    }

    public enum AccountType
    {
        /// <summary>
        /// User cannot log in
        /// </summary>
        Banned,
        /// <summary>
        /// Normal user, can play the game, and has no access to commands of any type
        /// </summary>
        User,
        /// <summary>
        /// Can access most commands, change other users to banned, or vice versa. Cannot modify GM's or higher.
        /// </summary>
        GameMaster,
        /// <summary>
        /// Same as <seealso cref="GameMaster">GameMaster</seealso> , but with additional access to some debug commands.
        /// </summary>
        Developer,
        /// <summary>
        /// Can access all commands and functions.
        /// </summary>
        Administrator
    }
    public class RegisterModel
    {
        public string EMailAddress { get; set; }
        public string Password { get; set; }
        public AccountType AccountType { get; set; }
    }
    public class AuthClient
    {
        private AuthClient()
        {

        }

        private HttpClient client = new HttpClient();

        private Uri GetUri(string path)
        {
            var baseUri =
                SharedConfiguration.Configuration.GetValue<Uri>("AuthApiUrl", new Uri("http://localhost:5001/"));
            return new Uri(baseUri, path);
        }

        public async Task<bool> DeleteAccountAsync(string email)
        {
            try
            {
                uint id;
                if(uint.TryParse(email, out var emailId))
                    id = emailId;
                else
                    id = await GetAccountIdByEmail(email);
                HttpResponseMessage response = await client.DeleteAsync(GetUri("api/account/" + id));
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public static AuthClient Client { get; } = new AuthClient();

        private HttpContent ToJsonContent<T>(T data)
        {
            return new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        }

        public async Task<uint> CreateAccountAsync(string email, string password, AccountType accountType = AccountType.User)
        {
            var model = new RegisterModel()
            {
                EMailAddress = email,
                Password = password,
                AccountType = accountType
            };

            HttpContent content = ToJsonContent(model);
            HttpResponseMessage response = await client.PostAsync(GetUri("api/account/create"), content).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                throw new AccountAlreadyExistsException();
            response.EnsureSuccessStatusCode();
            uint? id = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync().ConfigureAwait(false)).GetValue("Id", StringComparison.OrdinalIgnoreCase)?.Value<uint>();
            if (id == null) throw new InvalidOperationException("Could not read ID from response!");
            return id.Value;
        }

        public async Task<uint> GetAccountIdByEmail(string email)
        {
            HttpResponseMessage response = await client.GetAsync(GetUri("api/account/find?email=" + Uri.EscapeDataString(email))).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            uint? id = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync().ConfigureAwait(false)).GetValue("Id", StringComparison.OrdinalIgnoreCase)?.Value<uint>();
            if (id == null) throw new InvalidOperationException("Could not read ID from response!");
            return id.Value;
        }

        public async Task<string> GetGameToken(uint id)
        {
            HttpResponseMessage response = await client.GetAsync(GetUri("api/account/token/game/" + id)).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            string gameToken = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync().ConfigureAwait(false)).GetValue("GameToken", StringComparison.OrdinalIgnoreCase)?.Value<string>();
            if (string.IsNullOrEmpty(gameToken)) throw new InvalidOperationException("Could not read game token from response!");
            return gameToken;
        }

        public async Task<string> GenerateSessionKeyAsync(uint id)
        {
            HttpResponseMessage response = await client.PostAsync(GetUri("api/account/token/session/" + id), new ByteArrayContent(new byte[0])).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            string sessionKey = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync().ConfigureAwait(false)).GetValue("SessionKey", StringComparison.OrdinalIgnoreCase)?.Value<string>();
            if (string.IsNullOrEmpty(sessionKey)) throw new InvalidOperationException("Could not read session key from response!");
            return sessionKey;
        }

        public async Task<IEnumerable<Server>> GetServersAsync()
        {
            HttpResponseMessage response = await client.GetAsync(GetUri("api/realm")).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var jobj = JsonConvert.DeserializeObject<JObject>(json);
            var realmsPropertyValue = jobj.GetValue("realms", StringComparison.OrdinalIgnoreCase);
            return realmsPropertyValue.ToObject<Server[]>();
        }

        public async Task<string> GetSessionKeyAsync(uint id)
        {
            HttpResponseMessage response = await client.GetAsync(GetUri("api/account/token/session/" + id)).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            string sessionKey = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync().ConfigureAwait(false)).GetValue("SessionKey", StringComparison.OrdinalIgnoreCase)?.Value<string>();
            if (string.IsNullOrEmpty(sessionKey)) throw new InvalidOperationException("Could not read session key from response!");
            return sessionKey;
        }
    }
}
