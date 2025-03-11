﻿using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NexusForever.Game.Configuration.Model;
using NexusForever.Game.Static.RBAC;
using NexusForever.Network.Session;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared.Game;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Handler.Character;

namespace NexusForever.WorldServer
{
    public sealed class LoginQueueManager : ILoginQueueManager
    {
        public class QueueData
        {
            public string Id { get; init; }
            public uint Position { get; set; }
        }

        /// <summary>
        /// Amount of sessions admitted to server.
        /// </summary>
        /// <remarks>
        /// This will not necessarily match the total amount of connected sessions or players.
        /// </remarks>
        public uint ConnectedPlayers { get; private set; }

        private readonly Dictionary<string, QueueData> queueData = new();
        private readonly Queue<string> queue = new();

        private uint maximumPlayers;
        private readonly UpdateTimer queueCheck = new(TimeSpan.FromSeconds(5));

        #region Dependency Injection

        private readonly ILogger<LoginQueueManager> log;

        private readonly INetworkManager<IWorldSession> networkManager;
        private readonly ICharacterListManager characterListManager;

        public LoginQueueManager(
            ILogger<LoginQueueManager> log,
            IOptions<RealmConfig> realmOptions,
            INetworkManager<IWorldSession> networkManager,
            ICharacterListManager characterListManager)
        {
            this.log            = log;

            this.networkManager = networkManager;
            this.characterListManager = characterListManager;
            this.maximumPlayers = realmOptions.Value.MaxPlayers;
        }

        #endregion

        public void Update(double lastTick)
        {
            queueCheck.Update(lastTick);
            if (!queueCheck.HasElapsed)
                return;

            if (queue.Count > 0u)
            {
                while (queue.Count > 0u && CanEnterWorld())
                {
                    if (queue.TryDequeue(out string id))
                    {
                        queueData.Remove(id);
                        AdmitSession(id);
                    }
                }

                RecalculateQueuePositions();
            }

            queueCheck.Reset();
        }

        /// <summary>
        /// Attempt to admit session to realm, if the world is full the session will be queued.
        /// </summary>
        /// <remarks>
        /// Returns <see cref="true"/> if the session was admited to the realm.
        /// </remarks>
        public bool OnNewSession(IWorldSession session)
        {
            // check if session is already in queue
            // this allows the account to rejoin the queue after a disconnect
            if (queueData.TryGetValue(session.Id, out QueueData data))
            {
                log.LogTrace($"Session {session.Id} has rejoined the queue.");

                SendQueueStatus(session, data.Position);
                return false;
            }

            // session is not in the queue
            // check if the realm is currently accepting new sessions
            if (!CanEnterWorld(session))
            {
                log.LogTrace($"Session {session.Id} has joined the queue.");

                uint position = (uint)queue.Count + 1u;

                queue.Enqueue(session.Id);
                queueData.Add(session.Id, new QueueData
                {
                    Id       = session.Id,
                    Position = position
                });

                SendQueueStatus(session, position);
                return false;
            }

            AdmitSession(session);
            return true;
        }

        /// <summary>
        /// Remove session from realm queue.
        /// </summary>
        public void OnDisconnect(IWorldSession session)
        {
            // current admited session count will be reduced if session isn't queued
            if (session.IsQueued != false)
                return;

            checked
            {
                ConnectedPlayers--;
            }
        }

        /// <summary>
        /// Set the maximum number of admitted sessions allowed in the realm.
        /// </summary>
        public void SetMaxPlayers(uint newMaximumPlayers)
        {
            maximumPlayers = newMaximumPlayers;
            log.LogInformation($"Updated realm session limit to {maximumPlayers}.");
        }

        private void AdmitSession(string id)
        {
            // there is a possibility the session will not exist if the player has disconnected during the queue
            IWorldSession session = networkManager.GetSession(id);
            if (session == null)
                return;

            AdmitSession(session);

            session.EnqueueMessageEncrypted(new ServerQueueFinish());
            characterListManager.SendCharacterListPackets(session);
        }

        private void AdmitSession(IWorldSession session)
        {
            log.LogTrace($"Admitting session {session.Id} into the realm.");

            session.IsQueued = false;

            checked
            {
                ConnectedPlayers++;
            }
        }

        private bool CanEnterWorld(IWorldSession session)
        {
            // accounts with GM permission are exempt from queue (lucky you!)
            if (session.Account.RbacManager.HasPermission(Permission.GMFlag))
                return true;

            return CanEnterWorld();
        }

        private bool CanEnterWorld()
        {
            // potentially more checks?
            // world locked, ect...?
            return ConnectedPlayers < maximumPlayers;
        }

        private void RecalculateQueuePositions()
        {
            uint position = 1u;
            foreach (string id in queue)
            {
                if (!queueData.TryGetValue(id, out QueueData data))
                    continue;

                data.Position = position++;

                // there is a possibility the session will not exist if the player has disconnected during the queue
                IWorldSession session = networkManager.GetSession(data.Id);
                if (session != null)
                    SendQueueStatus(session, data.Position);
            }
        }

        private static void SendQueueStatus(IWorldSession session, uint queuePosition)
        {
            session.EnqueueMessageEncrypted(new ServerQueueStatus
            {
                QueuePosition   = queuePosition,
                WaitTimeSeconds = (uint)(queuePosition * 30f)
            });
        }
    }
}
