using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LeftieTestMod
{
    public class DownedBossSystem : ModSystem
    {
        public static bool downedWallOfFlesh = false;
        public static bool downedEaterOfWorlds = false;
        public static bool downedBrainOfCthulu = false;

        public override void OnWorldLoad()
        {
            downedWallOfFlesh = false;
            downedEaterOfWorlds = false;
            downedBrainOfCthulu = false;
        }

        public override void OnWorldUnload()
        {
            downedWallOfFlesh = false;
            downedEaterOfWorlds = false;
            downedBrainOfCthulu = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            if (downedWallOfFlesh)
            {
                tag["downedWallOfFlesh"] = true;
            }
            if (downedEaterOfWorlds)
            {
                tag["downedEaterOfWorlds"] = true;
            }
            if (downedBrainOfCthulu)
            {
                tag["downedBrainOfCthulu"] = true;
            }
        }

        public override void LoadWorldData(TagCompound tag)
        {
            downedWallOfFlesh = tag.ContainsKey("downedWallOfFlesh");
            downedEaterOfWorlds = tag.ContainsKey("downedEaterOfWorlds");
            downedBrainOfCthulu = tag.ContainsKey("downedBrainOfCthulu");
        }

        public override void NetSend(BinaryWriter writer)
        {
            // Order of operations is important and has to match that of NetReceive
            var flags = new BitsByte();
            flags[0] = downedWallOfFlesh;
            flags[1] = downedEaterOfWorlds;
            flags[2] = downedBrainOfCthulu;
            writer.Write(flags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            // Order of operations is important and has to match that of NetSend
            BitsByte flags = reader.ReadByte();
            downedWallOfFlesh = flags[0];
            downedEaterOfWorlds = flags[1];
            downedBrainOfCthulu = flags[2];
        }
    }
}
