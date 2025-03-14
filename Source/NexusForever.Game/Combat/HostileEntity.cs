﻿using NexusForever.Game.Abstract.Combat;
using NexusForever.Game.Abstract.Entity;

namespace NexusForever.Game.Combat
{
    public class HostileEntity : IHostileEntity
    {
        public uint HatedUnitId { get; }
        public uint Threat { get; private set; }

        /// <summary>
        /// Create a new <see cref="IHostileEntity"/> for the given <see cref="IUnitEntity"/>.
        /// </summary>
        public HostileEntity(uint targetGuid)
        {
            HatedUnitId = targetGuid;
        }

        /// <summary>
        /// Modify this <see cref="IHostileEntity"/> threat by the given amount.
        /// </summary>
        /// <remarks>
        /// Value is a delta, if a negative value is supplied it will be deducted from the existing threat if any.
        /// </remarks>
        public void UpdateThreat(int threat)
        {
            Threat = (uint)Math.Clamp(Threat + threat, 0u, uint.MaxValue);
        }
    }
}
