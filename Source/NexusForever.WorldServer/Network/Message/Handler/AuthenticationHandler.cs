using System.Linq;
using System.Threading.Tasks;
using NexusForever.Shared;
using NexusForever.Shared.Database.Auth;
using NexusForever.Shared.Database.Auth.Client;
using NexusForever.Shared.Database.Auth.Model;
using NexusForever.Shared.Game.Events;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class AuthenticationHandler
    {
        private static async Task<Account> GetAccountAsync(ClientHelloRealm message)
        {
            var sessionKey = await AuthClient.Client.GetSessionKeyAsync(message.AccountId);
            if (sessionKey.ToByteArray().SequenceEqual(message.SessionKey))
                return new Account
                {
                    Email = message.Email.ToLower(),
                    Id = message.AccountId,
                };
            return null;
        }

        [MessageHandler(GameMessageOpcode.ClientHelloRealm)]
        public static void HandleHelloRealm(WorldSession session, ClientHelloRealm helloRealm)
        {
            // prevent packets from being processed until asynchronous account select task is complete
            session.CanProcessPackets = false;

            session.EnqueueEvent(new TaskGenericEvent<Account>(GetAccountAsync(helloRealm),
                account =>
            {
                if (account == null)
                    throw new InvalidPacketValueException($"Failed to find account, Id:{helloRealm.AccountId}, Email:{helloRealm.Email}, SessionKey:{helloRealm.SessionKey}!");

                session.Account           = account;
                session.CanProcessPackets = true;
                session.SetEncryptionKey(helloRealm.SessionKey);
            }));
        }
    }
}
