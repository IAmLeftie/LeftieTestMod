using LeftieTestMod.Items.Quest;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod.NPCs
{
    public class BrainOfCthulu : GlobalNPC
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            if (entity.type == NPCID.BrainofCthulhu)
            {
                return true;
            }
            else return false;
        }

        public override void OnKill(NPC npc)
        {
            //Set the BoC's downed flag in DownedBossSystem to true
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedBrainOfCthulu, -1);

            //If a reclaimer quest is currently active for this boss, drop the quest item
            if (ReclaimerNPC.currentQuest.questId == 3)
            {
                Item.NewItem(npc.GetSource_Death(), npc.position, ModContent.ItemType<BoCQuestItem>());
            }
        }
    }
}
