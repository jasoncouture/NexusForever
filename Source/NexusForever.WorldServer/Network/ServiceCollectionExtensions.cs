﻿using Microsoft.Extensions.DependencyInjection;
using NexusForever.Network;
using NexusForever.Network.World;
using NexusForever.WorldServer.Network.Message.Handler;
using NexusForever.WorldServer.Network.Message.Handler.Character;

namespace NexusForever.WorldServer.Network
{
    public static class ServiceCollectionExtensions
    {
        public static void AddWorldNetwork(this IServiceCollection sc)
        {
            sc.AddNetwork();
            sc.AddNetworkConnectivity<IWorldSession, WorldSession>();
            sc.AddTransient<ICharacterListManager, CharacterListManager>();

            sc.AddNetworkWorld();
            sc.AddWorldNetworkMessageHandler();
        }
    }
}
