using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.GameTable.Static;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game;

namespace NexusForever.WorldServer.Command.Handler
{
    public class GoHandler : NamedCommand
    {
        public GoHandler()
            : base(false, "go")
        {
        }

        private IEnumerable<uint> GetTextIds(WorldLocation2Entry entry)
        {
            WorldZoneEntry worldZone = GameTableManager.WorldZone.GetEntry(entry.WorldZoneId);
            if (worldZone != null && worldZone.LocalizedTextIdName != 0)
                yield return worldZone.LocalizedTextIdName;
            WorldEntry world = GameTableManager.World.GetEntry(entry.WorldId);
            if (world != null && world.LocalizedTextIdName != 0)
                yield return world.LocalizedTextIdName;
        }
        protected override async Task HandleCommandAsync(CommandContext context, string command, string[] parameters)
        {
            int? index = null;
            if (parameters.Length == 0)
            {
                await context.SendErrorAsync("Usage: go <zone> [index]");
                return;
            }
            if (parameters.Length > 0 && int.TryParse(parameters[0], out int tmp))
            {
                index = tmp;
                parameters = parameters.Skip(1).ToArray();
                if (tmp < 0)
                {
                    await context.SendErrorAsync("Index must be greater than or equal to zero");
                    return;
                }
            }
            else if (int.TryParse(parameters[parameters.Length - 1], out tmp))
            {
                index = tmp;
                parameters = parameters.Take(parameters.Length - 1).ToArray();
                if (tmp < 0)
                {
                    await context.SendErrorAsync("Index must be greater than or equal to zero");
                    return;
                }
            }
            string zoneName = string.Join(" ", parameters);
            List<WorldLocation2Entry> searchResults =
                SearchManager.Search<WorldLocation2Entry>(zoneName, context.Language, GetTextIds).ToList();
            WorldLocation2Entry zone;
            if (index > 0)
            {
                if (index >= searchResults.Count)
                {
                    await context.SendErrorAsync($"Index must be less than the number of search results ({searchResults.Count})");
                    return;
                }

                zone = searchResults[index.Value];
            }
            else
            {
                zone = searchResults[0];
            }
            if (searchResults.Count > 1 && index == null)
            {
                var zoneList = string.Join(Environment.NewLine,
                    searchResults.Select((i, idx) => $"  {idx}: {i.WorldId} {i.Position0} {i.Position1} {i.Position2}").Take(5));
                if (searchResults.Count < 5)
                {
                    await context.SendWarningAsync($"{searchResults.Count} matches were found for {zoneName}");
                }
                else
                {
                    await context.SendWarningAsync($"{searchResults.Count} matches were found for {zoneName}, here's a list of the first 5");
                }

                await context.SendWarningAsync(zoneList);
                await context.SendWarningAsync("Using first entry.");
            }
            else if (searchResults.Count > 1)
            {
                await context.SendMessageAsync($"Search returned {searchResults.Count} results");
            }

            if (zone == null)
                await context.SendErrorAsync($"Unknown zone: {zoneName}");
            else
            {
                context.Session?.Player.TeleportTo((ushort)zone.WorldId, zone.Position0, zone.Position1, zone.Position2);
                await context.SendMessageAsync($"{zoneName}: {zone.WorldId} {zone.Position0} {zone.Position1} {zone.Position2}");
            }
        }
    }
}
