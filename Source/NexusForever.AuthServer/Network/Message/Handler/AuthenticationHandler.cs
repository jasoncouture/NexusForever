using System.Linq;
using System.Threading.Tasks;
using NexusForever.AuthServer.Network.Message.Model;
using NexusForever.Shared;
using NexusForever.Shared.Cryptography;
using NexusForever.Shared.Database.Auth;
using NexusForever.Shared.Database.Auth.Client;
using NexusForever.Shared.Database.Auth.Model;
using NexusForever.Shared.Game;
using NexusForever.Shared.Game.Events;
using NexusForever.Shared.Network.Message;

namespace NexusForever.AuthServer.Network.Message.Handler
{
    public static class AuthenticationHandler
    {
        private static async Task<Account> GetAccountAsync(ClientHelloAuth auth)
        {
            var account = new Account()
            {
                Email = auth.Email.ToLower()
            };
            try
            {
                account.Id = await AuthClient.Client.GetAccountIdByEmail(auth.Email).ConfigureAwait(false);
                account.GameToken = await AuthClient.Client.GetGameToken(account.Id).ConfigureAwait(false);
                if (auth.GameToken.Guid.ToByteArray().ToHexString() != account.GameToken) return null;
                account.SessionKey = await AuthClient.Client.GenerateSessionKeyAsync(account.Id).ConfigureAwait(false);
            }
            catch
            {
                return null;
            }

            return account;
        }
        [MessageHandler(GameMessageOpcode.ClientHelloAuth)]
        public static void HandleHelloAuth(AuthSession session, ClientHelloAuth helloAuth)
        {
            session.EnqueueEvent(new TaskGenericEvent<Account>(GetAccountAsync(helloAuth),
                account =>
            {

                if (account == null)
                {
                    // TODO: send error
                    return;
                }

                session.EnqueueMessageEncrypted(new ServerAuthAccepted());
                session.EnqueueMessageEncrypted(new ServerRealmMessages
                {
                    MessageGroup =
                    {
                        new ServerRealmMessages.Message
                        {
                            Index = 0,
                            Messages =
                            {
                                "Welcome to this NexusForever server!\nVisit: https://github.com/Rawaho/NexusForever"
                            }
                        }
                    }
                });
                ServerManager.ServerInfo server = ServerManager.Servers.First();
                session.EnqueueMessageEncrypted(new ServerRealmInfo
                {
                    AccountId = account.Id,
                    SessionKey = account.SessionKey.ToByteArray(),
                    Realm = server.Model.Name,
                    Host = server.Address,
                    Port = server.Model.Port,
                    Type = server.Model.Type
                });
            }));
        }
    }
}
