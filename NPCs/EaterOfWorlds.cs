using LeftieTestMod.Items.Quest;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod.NPCs
{
    public class EaterOfWorlds : GlobalNPC
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            if (entity.type == NPCID.EaterofWorldsHead || entity.type == NPCID.EaterofWorldsBody || entity.type == NPCID.EaterofWorldsTail)
            {
                return true;
            }
            else return false;
        }

        public override void OnKill(NPC npc)
        {
            //If there are no other EoW segments at the time of the death of this segment, set the downed flag in the
            //DownedBossSystem to true.
            if (!NPC.AnyNPCs(NPCID.EaterofWorldsHead) || !NPC.AnyNPCs(NPCID.EaterofWorldsBody) || !NPC.AnyNPCs(NPCID.EaterofWorldsTail))
            {
                NPC.SetEventFlagCleared(ref DownedBossSystem.downedEaterOfWorlds, -1);

                //If a reclaimer quest is currently active for this boss, drop the quest item
                if (ReclaimerNPC.currentQuest.questId == 2)
                {
                    Item.NewItem(npc.GetSource_Death(), npc.position, ModContent.ItemType<EoWQuestItem>());
                }
            }
        }
    }
}
