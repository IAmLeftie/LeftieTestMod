using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using LeftieTestMod.Items.Accessories;
using LeftieTestMod.Items.Weapons;
using LeftieTestMod.Conditions;
using LeftieTestMod.Items.Quest;

namespace LeftieTestMod.NPCs
{
    internal class NPCLoot : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, Terraria.ModLoader.NPCLoot npcLoot)
        {
            //Pirate Invasion Enemies
            if (npc.type == NPCID.PirateCaptain || npc.type == NPCID.PirateCorsair || npc.type == NPCID.Parrot
                || npc.type == NPCID.PirateCrossbower || npc.type == NPCID.PirateDeadeye || npc.type == NPCID.PirateDeckhand
                || npc.type == NPCID.PirateGhost)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BlackPowderAccessory>(), 200));
            }

            if (npc.type == NPCID.PirateShip)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LooseCannon>(), 10));
            }

            //Hallowed Enemies
            if (npc.type == NPCID.Pixie || npc.type == NPCID.Unicorn || npc.type == NPCID.RainbowSlime
                || npc.type == NPCID.Gastropod || npc.type == NPCID.LightMummy || npc.type == NPCID.IlluminantSlime
                || npc.type == NPCID.IlluminantBat || npc.type == NPCID.ChaosElemental || npc.type == NPCID.PigronHallow
                || npc.type == NPCID.EnchantedSword || npc.type == NPCID.DesertGhoulHallow)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MagicPouchAccessory>(), 100));
            }

            //Underworld Enemies
            if (npc.type == NPCID.Hellbat || npc.type == NPCID.LavaSlime || npc.type == NPCID.FireImp
                || npc.type == NPCID.Demon || npc.type == NPCID.VoodooDemon || npc.type == NPCID.BoneSerpentHead
                || npc.type == NPCID.Lavabat || npc.type == NPCID.RedDevil)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AnnihilatorRoundsAccessory>(), 100));
            }

            //Plantera
            if (npc.type == NPCID.Plantera)
            {
                IItemDropRule boomestStickDropRule = new LeadingConditionRule(new SimpleDropCondition());

                boomestStickDropRule.OnSuccess(new CommonDrop(ModContent.ItemType<BoomestStick>(),
                    chanceNumerator: 3, chanceDenominator: 20));
                npcLoot.Add(boomestStickDropRule);
            }

            //Martian Saucer
            if (npc.type == NPCID.MartianSaucerCore)
            {
                IItemDropRule subspaceEjectorDropRule = new LeadingConditionRule(new SimpleDropCondition());

                subspaceEjectorDropRule.OnSuccess(new CommonDrop(ModContent.ItemType<SubspaceEjector>(),
                    chanceNumerator: 167, chanceDenominator: 1000));
                npcLoot.Add(subspaceEjectorDropRule);
            }

            //Moon Lord
            if (npc.type == NPCID.MoonLordCore)
            {
                IItemDropRule yeetCannonDropRule = new LeadingConditionRule(new SimpleDropCondition());

                yeetCannonDropRule.OnSuccess(new CommonDrop(ModContent.ItemType<YeetCannon>(),
                    chanceNumerator: 111, chanceDenominator: 1000));
                npcLoot.Add(yeetCannonDropRule);
            }
        }

        //Drop Reclaimer quest items when applicable
        public override void OnKill(NPC npc)
        {
            #region Reclaimer Boss Drop Logic
            //King Slime
            if (npc.type == NPCID.KingSlime && ReclaimerNPC.currentQuest != null && ReclaimerNPC.currentQuest.questId == 0)
            {
                Item.NewItem(npc.GetSource_Death(), npc.position, ModContent.ItemType<KingSlimeQuestItem>());
            }
            //EoC
            if (npc.type == NPCID.EyeofCthulhu && ReclaimerNPC.currentQuest != null && ReclaimerNPC.currentQuest.questId == 1)
            {
                Item.NewItem(npc.GetSource_Death(), npc.position, ModContent.ItemType<KingSlimeQuestItem>());
            }

            //EoW, BoC, and WoF are handled in their respective classes to avoid redundant code

            //Skeletron
            if (npc.type == NPCID.SkeletronHead && ReclaimerNPC.currentQuest != null && ReclaimerNPC.currentQuest.questId == 4)
            {
                Item.NewItem(npc.GetSource_Death(), npc.position, ModContent.ItemType<SkeletronQuestItem>());
            }
            //Queen Bee
            if (npc.type == NPCID.QueenBee && ReclaimerNPC.currentQuest != null && ReclaimerNPC.currentQuest.questId == 5)
            {
                Item.NewItem(npc.GetSource_Death(), npc.position, ModContent.ItemType<QueenBeeQuestItem>());
            }
            //Deerclops
            if (npc.type == NPCID.Deerclops && ReclaimerNPC.currentQuest != null && ReclaimerNPC.currentQuest.questId == 6)
            {
                Item.NewItem(npc.GetSource_Death(), npc.position, ModContent.ItemType<DeerclopsQuestItem>());
            }

            //Queen Slime
            if (npc.type == NPCID.QueenSlimeBoss && ReclaimerNPC.currentQuest != null && ReclaimerNPC.currentQuest.questId == 8)
            {
                Item.NewItem(npc.GetSource_Death(), npc.position, ModContent.ItemType<QueenSlimeQuestItem>());
            }
            //Destroyer
            if (npc.type == NPCID.TheDestroyer && ReclaimerNPC.currentQuest != null && ReclaimerNPC.currentQuest.questId == 9)
            {
                Item.NewItem(npc.GetSource_Death(), npc.position, ModContent.ItemType<DestroyerQuestItem>());
            }
            //Twins
            if (npc.type == NPCID.Retinazer && ReclaimerNPC.currentQuest != null && ReclaimerNPC.currentQuest.questId == 10)
            {
                Item.NewItem(npc.GetSource_Death(), npc.position, ModContent.ItemType<TwinsQuestItem>());
            }
            //Skeletron Prime
            if (npc.type == NPCID.SkeletronPrime && ReclaimerNPC.currentQuest != null && ReclaimerNPC.currentQuest.questId == 11)
            {
                Item.NewItem(npc.GetSource_Death(), npc.position, ModContent.ItemType<PrimeQuestItem>());
            }
            //Plantera
            if (npc.type == NPCID.Plantera && ReclaimerNPC.currentQuest != null && ReclaimerNPC.currentQuest.questId == 12)
            {
                Item.NewItem(npc.GetSource_Death(), npc.position, ModContent.ItemType<PlanteraQuestItem>());
            }
            //Golem
            if (npc.type == NPCID.Golem && ReclaimerNPC.currentQuest != null && ReclaimerNPC.currentQuest.questId == 13)
            {
                Item.NewItem(npc.GetSource_Death(), npc.position, ModContent.ItemType<GolemQuestItem>());
            }
            //Duke Fishron
            if (npc.type == NPCID.DukeFishron && ReclaimerNPC.currentQuest != null && ReclaimerNPC.currentQuest.questId == 14)
            {
                Item.NewItem(npc.GetSource_Death(), npc.position, ModContent.ItemType<FishronQuestItem>());
            }
            //Empress of Light
            if (npc.type == NPCID.HallowBoss && ReclaimerNPC.currentQuest != null && ReclaimerNPC.currentQuest.questId == 15)
            {
                Item.NewItem(npc.GetSource_Death(), npc.position, ModContent.ItemType<EmpressQuestItem>());
            }
            //Moon Lord
            if (npc.type == NPCID.MoonLordCore && ReclaimerNPC.currentQuest != null && ReclaimerNPC.currentQuest.questId == 16)
            {
                Item.NewItem(npc.GetSource_Death(), npc.position, ModContent.ItemType<EmpressQuestItem>());
            }
            #endregion
        }
    }
}
