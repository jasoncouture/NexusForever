﻿using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerRealmFirstAchievement)]
    public class ServerRealmFirstAchievement : IWritable
    {
        public ushort AchievementId { get; set; }
        public bool Unknown { get; set; }
        public string Player { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AchievementId, 15u);
            writer.Write(Unknown);
            writer.WriteStringWide(Player);
        }
    }
}
