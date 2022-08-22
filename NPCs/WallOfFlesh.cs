using LeftieTestMod.Items.Quest;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod.NPCs
{
    public class WallOfFlesh : GlobalNPC
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            if (entity.type == NPCID.WallofFlesh)
            {
                return true;
            }
            else return false;
        }

        public override void OnKill(NPC npc)
        {
            //Set the WoF's downed flag in DownedBossSystem to true
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedWallOfFlesh, -1);

            //If a reclaimer quest is currently active for this boss, drop the quest item
            if (ReclaimerNPC.currentQuest.questId == 7)
            {
                Item.NewItem(npc.GetSource_Death(), npc.position, ModContent.ItemType<WoFQuestItem>());
            }
        }
    }
}
