using LeftieTestMod.Items.Quest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod
{
    public class ReclaimerQuest
    {
        public int questId;
        public int rewardScripCount;

        public ReclaimerQuest(int bossId)
        {
            //Assign the quest characteristics depending on the provided boss id
            switch(bossId)
            {
                case NPCID.EyeofCthulhu:
                    questId = 1;
                    rewardScripCount = 1;
                    break;

                case NPCID.EaterofWorldsHead:
                    questId = 2;
                    rewardScripCount = 1;
                    break;

                case NPCID.BrainofCthulhu:
                    questId = 3;
                    rewardScripCount = 1;
                    break;

                case NPCID.SkeletronHead:
                    questId = 4;
                    rewardScripCount = 1;
                    break;

                case NPCID.QueenBee:
                    questId = 5;
                    rewardScripCount = 1;
                    break;

                case NPCID.Deerclops:
                    questId = 6;
                    rewardScripCount = 1;
                    break;

                case NPCID.WallofFlesh:
                    questId = 7;
                    rewardScripCount = 1;
                    break;

                case NPCID.QueenSlimeBoss:
                    questId = 8;
                    rewardScripCount = 1;
                    if (Main.expertMode)
                    {
                        rewardScripCount = 2;
                    }
                    break;

                case NPCID.TheDestroyer:
                    questId = 9;
                    rewardScripCount = 1;
                    if (Main.expertMode)
                    {
                        rewardScripCount = 2;
                    }
                    break;

                case NPCID.Retinazer:
                    questId = 10;
                    rewardScripCount = 1;
                    if (Main.expertMode)
                    {
                        rewardScripCount = 2;
                    }
                    break;

                case NPCID.SkeletronPrime:
                    questId = 11;
                    rewardScripCount = 1;
                    if (Main.expertMode)
                    {
                        rewardScripCount = 2;
                    }
                    break;

                case NPCID.Plantera:
                    questId = 12;
                    rewardScripCount = 1;
                    if (Main.expertMode)
                    {
                        rewardScripCount = 2;
                    }
                    break;

                case NPCID.Golem:
                    questId = 13;
                    rewardScripCount = 1;
                    if (Main.expertMode)
                    {
                        rewardScripCount = 2;
                    }
                    break;

                case NPCID.DukeFishron:
                    questId = 14;
                    rewardScripCount = 1;
                    if (Main.expertMode)
                    {
                        rewardScripCount = 2;
                    }
                    break;

                case NPCID.HallowBoss:
                    questId = 15;
                    rewardScripCount = 2;
                    if (Main.expertMode)
                    {
                        rewardScripCount = 3;
                    }
                    break;

                case NPCID.MoonLordCore:
                    questId = 16;
                    rewardScripCount = 2;
                    if (Main.expertMode)
                    {
                        rewardScripCount = 3;
                    }
                    break;

                case NPCID.KingSlime:
                default:
                    questId = 0;
                    rewardScripCount = 1;
                    break;
            }
        }
    }
}
