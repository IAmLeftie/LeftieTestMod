using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.Personalities;
using Terraria.DataStructures;
using System.Collections.Generic;
using ReLogic.Content;
using LeftieTestMod;
using LeftieTestMod.Items.Accessories;
using LeftieTestMod.Items.Quest;
using LeftieTestMod.Items;

namespace LeftieTestMod.NPCs
{
    [AutoloadHead]
    internal class ReclaimerNPC : ModNPC
    {
        internal static List<int> validBosses;
        internal static bool dayStarted = false;
        internal static bool canGiveQuest = false;
        internal static bool questGiven = false;
        public static ReclaimerQuest currentQuest;
        internal static int expectingQuestItemId = -1; //Set to -1 so no item is ever unexpectedly accepted
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reclaimer");
            Main.npcFrameCount[Type] = 26;

            NPCID.Sets.ExtraFramesCount[Type] = 9;
            NPCID.Sets.AttackFrameCount[Type] = 5;
            NPCID.Sets.DangerDetectRange[Type] = 700;
            NPCID.Sets.AttackType[Type] = 3;
            NPCID.Sets.AttackTime[Type] = 90; // The amount of time it takes for the NPC's attack animation to be over once it starts.
            NPCID.Sets.AttackAverageChance[Type] = 30;
            NPCID.Sets.HatOffsetY[Type] = 4; // For when a party is active, the party hat spawns at a Y offset.

            // Influences how the NPC looks in the Bestiary
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Velocity = 1f, // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
                Direction = -1
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);


            NPC.Happiness
                .SetBiomeAffection<DesertBiome>(AffectionLevel.Like)
                .SetBiomeAffection<SnowBiome>(AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.Guide, AffectionLevel.Love)
                .SetNPCAffection(NPCID.Dryad, AffectionLevel.Like)
                .SetNPCAffection(NPCID.GoblinTinkerer, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.Demolitionist, AffectionLevel.Hate);
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true; // Sets NPC to be a Town NPC
            NPC.friendly = true; // NPC Will not attack player
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = 7;
            NPC.damage = 10;
            NPC.defense = 15;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;

            AnimationType = NPCID.Guide;

            //Initialize the list of valid bosses
            validBosses = new List<int>();
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Desert,

                new FlavorTextBestiaryInfoElement("Obsessed with the curious relics and mysteries of the world, the " +
                "Reclaimer does his best to preserve history and track down the remains of great beings for the sake " +
                "of research.")
            });
        }

        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        { // Requirements for the town NPC to spawn.
            for (int k = 0; k < 255; k++)
            {
                Player player = Main.player[k];
                if (!player.active)
                {
                    continue;
                }

                //World has to have a cumulative Boss Kill score of 5 and the player, a cumulative Item Score of 6 in
                //order to spawn the NPC
                if (GetBossKillScore() >= 5 && player.GetModPlayer<ModPlayerCostume>().GetItemScore() >= 6)
                {
                    return true;
                }

            }

            return false;
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string>()
            {
                //Add more names later!(?)
                "Toby",
                "Edward",
                "Henri",
                "Joel",
                "Arthur",
                "Percy",
                "Franz",
                "Henry",
                "Joseph"
            };
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.64");
            button2 = Language.GetTextValue("LegacyInterface.28");
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            bool questJustCompleted = false;

            if (firstButton)
            {
                //If the quest for the day has not been given, tick the flag so that the reclaimer will start
                //expecting an item
                if (!questGiven) { questGiven = true; }
                //If a quest has been given, and the player has the respective item in their inventory, accept it,
                //reward the player, and remove their ability to provide another request or accept items for the day
                else
                {
                    if (Main.LocalPlayer.HasItem(expectingQuestItemId))
                    {
                        questJustCompleted = true;
                        //Take the item from the player's inventory
                        int expectingItemInvSlot = Main.LocalPlayer.FindItem(expectingQuestItemId);
                        Main.LocalPlayer.inventory[expectingItemInvSlot].stack--;

                        //Give the player the respective amount of Reclaimer Scrips
                        Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_FromThis(),
                                ModContent.ItemType<ReclaimerScrip>(), currentQuest.rewardScripCount);
                        //Reset the respective flags to prevent the reclaimer from giving another quest until daily reset
                        canGiveQuest = false;
                        expectingQuestItemId = -1;
                        currentQuest = null;
                    }
                }
                //TODO: Set up actual proper chitchat in these quests!
                //If the currentQuest isnt null...
                if (currentQuest != null)
                {
                    switch (currentQuest.questId)
                    {
                        //King Slime
                        case 0:
                            Main.npcChatText = $"Kill King Slime";
                            //If the player doesn't already have the summon item in their inventory, provide them with it
                            if (!Main.LocalPlayer.HasItem(ItemID.SlimeCrown))
                            {
                                Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_FromThis(),
                                    ItemID.SlimeCrown);
                            }
                            //Set the reclaimer to expect the respective quest item
                            expectingQuestItemId = ModContent.ItemType<KingSlimeQuestItem>();
                            break;

                        case 1:
                            Main.npcChatText = $"Kill Eye of Cthulhu";
                            //If the player doesn't already have the summon item in their inventory, provide them with it
                            if (!Main.LocalPlayer.HasItem(ItemID.SuspiciousLookingEye))
                            {
                                Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_FromThis(),
                                    ItemID.SuspiciousLookingEye);
                            }
                            //Set the reclaimer to expect the respective quest item
                            expectingQuestItemId = ModContent.ItemType<EoCQuestItem>();
                            break;

                        case 2:
                            Main.npcChatText = $"Kill Eater of Worlds";
                            //If the player doesn't already have the summon item in their inventory, provide them with it
                            if (!Main.LocalPlayer.HasItem(ItemID.WormFood))
                            {
                                Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_FromThis(),
                                    ItemID.WormFood);
                            }
                            //Set the reclaimer to expect the respective quest item
                            expectingQuestItemId = ModContent.ItemType<EoWQuestItem>();
                            break;

                        case 3:
                            Main.npcChatText = $"Kill Brain of Cthulhu";
                            //If the player doesn't already have the summon item in their inventory, provide them with it
                            if (!Main.LocalPlayer.HasItem(ItemID.BloodySpine))
                            {
                                Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_FromThis(),
                                    ItemID.BloodySpine);
                            }
                            //Set the reclaimer to expect the respective quest item
                            expectingQuestItemId = ModContent.ItemType<BoCQuestItem>();
                            break;

                        case 4:
                            Main.npcChatText = $"Kill Skeletron";
                            //If the player doesn't already have the summon item in their inventory, provide them with it
                            if (!Main.LocalPlayer.HasItem(ItemID.ClothierVoodooDoll))
                            {
                                Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_FromThis(),
                                    ItemID.ClothierVoodooDoll);
                            }
                            //Set the reclaimer to expect the respective quest item
                            expectingQuestItemId = ModContent.ItemType<SkeletronQuestItem>();
                            break;

                        case 5:
                            Main.npcChatText = $"Kill Queen Bee";
                            //If the player doesn't already have the summon item in their inventory, provide them with it
                            if (!Main.LocalPlayer.HasItem(ItemID.Abeemination))
                            {
                                Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_FromThis(),
                                    ItemID.Abeemination);
                            }
                            //Set the reclaimer to expect the respective quest item
                            expectingQuestItemId = ModContent.ItemType<QueenBeeQuestItem>();
                            break;

                        case 6:
                            Main.npcChatText = $"Kill Deerclops";
                            //If the player doesn't already have the summon item in their inventory, provide them with it
                            if (!Main.LocalPlayer.HasItem(ItemID.DeerThing))
                            {
                                Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_FromThis(),
                                    ItemID.DeerThing);
                            }
                            //Set the reclaimer to expect the respective quest item
                            expectingQuestItemId = ModContent.ItemType<DeerclopsQuestItem>();
                            break;

                        case 7:
                            Main.npcChatText = $"Kill Wall of Flesh";
                            //If the player doesn't already have the summon item in their inventory, provide them with it
                            if (!Main.LocalPlayer.HasItem(ItemID.GuideVoodooDoll))
                            {
                                Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_FromThis(),
                                    ItemID.GuideVoodooDoll);
                            }
                            //Set the reclaimer to expect the respective quest item
                            expectingQuestItemId = ModContent.ItemType<WoFQuestItem>();
                            break;

                        case 8:
                            Main.npcChatText = $"Kill Queen Slime";
                            //If the player doesn't already have the summon item in their inventory, provide them with it
                            if (!Main.LocalPlayer.HasItem(ItemID.QueenSlimeCrystal))
                            {
                                Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_FromThis(),
                                    ItemID.QueenSlimeCrystal);
                            }
                            //Set the reclaimer to expect the respective quest item
                            expectingQuestItemId = ModContent.ItemType<QueenSlimeQuestItem>();
                            break;

                        case 9:
                            Main.npcChatText = $"Kill The Destroyer";
                            //If the player doesn't already have the summon item in their inventory, provide them with it
                            if (!Main.LocalPlayer.HasItem(ItemID.MechanicalWorm))
                            {
                                Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_FromThis(),
                                    ItemID.MechanicalWorm);
                            }
                            //Set the reclaimer to expect the respective quest item
                            expectingQuestItemId = ModContent.ItemType<DestroyerQuestItem>();
                            break;

                        case 10:
                            Main.npcChatText = $"Kill The Twins";
                            //If the player doesn't already have the summon item in their inventory, provide them with it
                            if (!Main.LocalPlayer.HasItem(ItemID.MechanicalEye))
                            {
                                Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_FromThis(),
                                    ItemID.MechanicalEye);
                            }
                            //Set the reclaimer to expect the respective quest item
                            expectingQuestItemId = ModContent.ItemType<TwinsQuestItem>();
                            break;

                        case 11:
                            Main.npcChatText = $"Kill Skeletron Prime";
                            //If the player doesn't already have the summon item in their inventory, provide them with it
                            if (!Main.LocalPlayer.HasItem(ItemID.MechanicalSkull))
                            {
                                Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_FromThis(),
                                    ItemID.MechanicalSkull);
                            }
                            //Set the reclaimer to expect the respective quest item
                            expectingQuestItemId = ModContent.ItemType<PrimeQuestItem>();
                            break;

                        case 12:
                            Main.npcChatText = $"Kill Plantera";
                            //TODO: Make the summon item
                            ////If the player doesn't already have the summon item in their inventory, provide them with it
                            //if (!Main.LocalPlayer.HasItem(ItemID.SlimeCrown))
                            //{
                            //    Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_FromThis(),
                            //        ItemID.SlimeCrown);
                            //}
                            //Set the reclaimer to expect the respective quest item
                            expectingQuestItemId = ModContent.ItemType<PlanteraQuestItem>();
                            break;

                        case 13:
                            Main.npcChatText = $"Kill Golem";
                            //If the player doesn't already have the summon item in their inventory, provide them with it
                            if (!Main.LocalPlayer.HasItem(ItemID.LihzahrdPowerCell))
                            {
                                Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_FromThis(),
                                    ItemID.LihzahrdPowerCell);
                            }
                            //Set the reclaimer to expect the respective quest item
                            expectingQuestItemId = ModContent.ItemType<GolemQuestItem>();
                            break;

                        case 14:
                            Main.npcChatText = $"Kill Duke Fishron";
                            //If the player doesn't already have the summon item in their inventory, provide them with it
                            if (!Main.LocalPlayer.HasItem(ItemID.TruffleWorm))
                            {
                                Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_FromThis(),
                                    ItemID.TruffleWorm);
                            }
                            //Set the reclaimer to expect the respective quest item
                            expectingQuestItemId = ModContent.ItemType<FishronQuestItem>();
                            break;

                        case 15:
                            Main.npcChatText = $"Kill Empress of Light";
                            //If the player doesn't already have the summon item in their inventory, provide them with it
                            if (!Main.LocalPlayer.HasItem(ItemID.EmpressButterfly))
                            {
                                Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_FromThis(),
                                    ItemID.EmpressButterfly);
                            }
                            //Set the reclaimer to expect the respective quest item
                            expectingQuestItemId = ModContent.ItemType<EmpressQuestItem>();
                            break;

                        case 16:
                            Main.npcChatText = $"Kill Moon Lord";
                            //If the player doesn't already have the summon item in their inventory, provide them with it
                            if (!Main.LocalPlayer.HasItem(ItemID.CelestialSigil))
                            {
                                Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_FromThis(),
                                    ItemID.CelestialSigil);
                            }
                            //Set the reclaimer to expect the respective quest item
                            expectingQuestItemId = ModContent.ItemType<MoonLordQuestItem>();
                            break;
                    }
                }
                else if (questJustCompleted)
                {
                    Main.npcChatText = $"Thanks!";
                }
                //If it is null, that likely means the player has already done their quest for the day, so turn them away
                else
                {
                    Main.npcChatText = $"No Quest available right now, sorry!";
                }
            }
            else
            {
                shop = true;
            }
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            //We gotta set this up very inefficiently, because the way I wanna do this wont allow for iteration
            //So here we go!

            #region Shop Population Logic

            #region Zenith
            //Copper Shortsword
            if (!Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.Zenith]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.CopperShortsword);
                shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 1);
                nextSlot++;
            }

            //Starfury
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.Starfury]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.Zenith]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.Starfury);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Enchanted Sword
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.EnchantedSword]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.Zenith]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.EnchantedSword);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Bee Keeper
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.BeeKeeper]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.Zenith]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.BeeKeeper);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Excalibur
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.Excalibur]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.TrueExcalibur]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.Excalibur);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //True Excalibur
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.TrueExcalibur]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.TerraBlade]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.TrueExcalibur);
                shop.item[nextSlot].shopCustomPrice = 2;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Broken Hero Sword
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.BrokenHeroSword]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.TerraBlade]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.BrokenHeroSword);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Blade of Grass
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.BladeofGrass]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.NightsEdge]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.BladeofGrass);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Muramasa
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.Muramasa]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.NightsEdge]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.Muramasa);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Fiery Greatsword
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.FieryGreatsword]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.NightsEdge]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.FieryGreatsword);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Light's Bane
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.LightsBane]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.NightsEdge]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.LightsBane);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Night's Edge
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.NightsEdge]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.TrueNightsEdge]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.NightsEdge);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //True Night's Edge
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.TrueNightsEdge]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.TerraBlade]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.TrueNightsEdge);
                shop.item[nextSlot].shopCustomPrice = 2;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Terra Blade
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.TerraBlade]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.Zenith]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.TerraBlade);
                shop.item[nextSlot].shopCustomPrice = 3;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Seedler
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.Seedler]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.Zenith]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.Seedler);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //The Horseman's Blade
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.TheHorsemansBlade]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.Zenith]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.TheHorsemansBlade);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Influx Waver
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.InfluxWaver]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.Zenith]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.InfluxWaver);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Star Wrath
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.StarWrath]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.Zenith]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.StarWrath);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Meowmere
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.Meowmere]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.Zenith]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.Meowmere);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Zenith
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.Zenith]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.Zenith);
                shop.item[nextSlot].shopCustomPrice = 4;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }
            #endregion

            #region Ankh Shield
            //Cobalt Shield
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.CobaltShield]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.ObsidianShield]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.CobaltShield);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Blindfold
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.Blindfold]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.AnkhCharm]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.Blindfold);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Vitamins
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.Vitamins]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.ArmorBracing]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.Vitamins);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Armor Polish
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.ArmorPolish]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.ArmorBracing]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.ArmorPolish);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Adhesive Bandage
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.AdhesiveBandage]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.MedicatedBandage]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.AdhesiveBandage);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Bezoar
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.Bezoar]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.MedicatedBandage]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.Bezoar);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Nazar
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.Nazar]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.CountercurseMantra]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.Nazar);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Megaphone
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.Megaphone]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.CountercurseMantra]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.Megaphone);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Trifold Map
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.TrifoldMap]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.ThePlan]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.TrifoldMap);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Fast Clock
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.FastClock]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.ThePlan]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.FastClock);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Armor Bracing
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.ArmorBracing]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.AnkhCharm]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.ArmorBracing);
                shop.item[nextSlot].shopCustomPrice = 2;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Medicated Bandage
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.MedicatedBandage]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.AnkhCharm]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.MedicatedBandage);
                shop.item[nextSlot].shopCustomPrice = 2;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Countercurse Mantra
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.CountercurseMantra]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.AnkhCharm]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.CountercurseMantra);
                shop.item[nextSlot].shopCustomPrice = 2;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //The Plan
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.ThePlan]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.AnkhCharm]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.ThePlan);
                shop.item[nextSlot].shopCustomPrice = 2;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Ankh Charm
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.AnkhCharm]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.AnkhShield]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.AnkhCharm);
                shop.item[nextSlot].shopCustomPrice = 3;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Obsidian Shield
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.ObsidianShield]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.AnkhShield]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.ObsidianShield);
                shop.item[nextSlot].shopCustomPrice = 2;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Ankh Shield
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.AnkhShield]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.AnkhShield);
                shop.item[nextSlot].shopCustomPrice = 4;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }
            #endregion

            #region Cell Phone
            //Depth Meter
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.DepthMeter]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.GPS]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.DepthMeter);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Compass
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.Compass]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.GPS]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.Compass);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Radar
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.Radar]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.REK]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.Radar);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Tally Counter
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.TallyCounter]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.REK]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.TallyCounter);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Metal Detector
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.MetalDetector]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.GoblinTech]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.MetalDetector);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Fisherman's Pocket Guide
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.FishermansGuide]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.FishFinder]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.FishermansGuide);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Weather Radio
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.WeatherRadio]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.FishFinder]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.WeatherRadio);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Sextant
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.Sextant]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.FishFinder]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.Sextant);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //GPS
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.GPS]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.PDA]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.GPS);
                shop.item[nextSlot].shopCustomPrice = 2;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //REK 3000
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.REK]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.PDA]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.REK);
                shop.item[nextSlot].shopCustomPrice = 2;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Goblin Tech
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.GoblinTech]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.PDA]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.GoblinTech);
                shop.item[nextSlot].shopCustomPrice = 2;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Fish Finder
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.FishFinder]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.PDA]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.FishFinder);
                shop.item[nextSlot].shopCustomPrice = 2;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //PDA
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.PDA]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.CellPhone]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.PDA);
                shop.item[nextSlot].shopCustomPrice = 3;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Magic Mirror
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.MagicMirror]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.CellPhone]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.MagicMirror);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Cell Phone
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.CellPhone]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.CellPhone);
                shop.item[nextSlot].shopCustomPrice = 4;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }
            #endregion

            #region Terraspark Boots
            //Hermes Boots
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.HermesBoots]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.SpectreBoots]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.HermesBoots);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Water Walking Boots
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.WaterWalkingBoots]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.LavaWaders]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.WaterWalkingBoots);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Lava Charm
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.LavaCharm]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.MoltenCharm]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.LavaCharm);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Obsidian Rose
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.ObsidianRose]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.LavaWaders]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.ObsidianRose);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Molten Charm
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.MoltenCharm]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.LavaWaders]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.MoltenCharm);
                shop.item[nextSlot].shopCustomPrice = 2;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Aglet
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.Aglet]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.LightningBoots]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.Aglet);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Spectre Boots
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.SpectreBoots]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.LightningBoots]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.SpectreBoots);
                shop.item[nextSlot].shopCustomPrice = 2;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Anklet of the Wind
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.AnkletoftheWind]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.LightningBoots]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.AnkletoftheWind);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Ice Skates
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.IceSkates]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.FrostsparkBoots]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.IceSkates);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Lightning Boots
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.LightningBoots]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.FrostsparkBoots]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.LightningBoots);
                shop.item[nextSlot].shopCustomPrice = 3;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Frostspark Boots
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.FrostsparkBoots]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.TerrasparkBoots]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.FrostsparkBoots);
                shop.item[nextSlot].shopCustomPrice = 4;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Lava Waders
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.LavaWaders]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.TerrasparkBoots]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.LavaWaders);
                shop.item[nextSlot].shopCustomPrice = 3;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Terraspark Boots
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.TerrasparkBoots]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.TerrasparkBoots);
                shop.item[nextSlot].shopCustomPrice = 5;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }
            #endregion

            #region Master Ninja Gear
            //Climbing Claws
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.ClimbingClaws]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.TigerClimbingGear]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.ClimbingClaws);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Shoe Spikes
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.ShoeSpikes]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.TigerClimbingGear]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.ShoeSpikes);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Tabi
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.Tabi]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.MasterNinjaGear]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.Tabi);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Black Belt
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.BlackBelt]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.MasterNinjaGear]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.BlackBelt);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Tiger Climbing Gear
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.TigerClimbingGear]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.MasterNinjaGear]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.TigerClimbingGear);
                shop.item[nextSlot].shopCustomPrice = 2;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Master Ninja Gear
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.MasterNinjaGear]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.MasterNinjaGear);
                shop.item[nextSlot].shopCustomPrice = 3;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }
            #endregion

            #region Celestial Cuffs
            //Band of Regeneration
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.BandofRegeneration]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.ManaRegenerationBand]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.BandofRegeneration);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Band of Starpower
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.BandofStarpower]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.ManaRegenerationBand]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.BandofStarpower);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Mana Regeneration Band
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.ManaRegenerationBand]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.MagicCuffs]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.ManaRegenerationBand);
                shop.item[nextSlot].shopCustomPrice = 2;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Shackle
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.Shackle]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.MagicCuffs]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.Shackle);
                shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 50);
                //Just sell it in so coins, its so early in progression it'd be a scam to use scrip
                nextSlot++;
            }

            //Magic Cuffs
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.MagicCuffs]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.CelestialCuffs]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.MagicCuffs);
                shop.item[nextSlot].shopCustomPrice = 3;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Celestial Cuffs
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.CelestialCuffs]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.CelestialCuffs);
                shop.item[nextSlot].shopCustomPrice = 4;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }
            #endregion

            #region Celestial Emblem
            //Avenger Emblem
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.AvengerEmblem]
                && (!Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.CelestialEmblem]
                || !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.DestroyerEmblem]
                || !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.MechanicalGlove])
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.AvengerEmblem);
                shop.item[nextSlot].shopCustomPrice = 2;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Celestial Emblem
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.CelestialEmblem]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.CelestialEmblem);
                shop.item[nextSlot].shopCustomPrice = 3;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }
            #endregion

            #region Recon Scope
            //Destroyer Emblem
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.DestroyerEmblem]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.SniperScope]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.DestroyerEmblem);
                shop.item[nextSlot].shopCustomPrice = 3;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Rifle Scope
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.RifleScope]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.SniperScope]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.RifleScope);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Sniper Scope
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.SniperScope]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.ReconScope]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.SniperScope);
                shop.item[nextSlot].shopCustomPrice = 4;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Putrid Scent
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.PutridScent]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.ReconScope]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.PutridScent);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Recon Scope
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.ReconScope]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.ReconScope);
                shop.item[nextSlot].shopCustomPrice = 5;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }
            #endregion

            #region Fire Gauntlet
            //Feral Claws
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.FeralClaws]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.PowerGlove]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.FeralClaws);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Titan Glove
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.TitanGlove]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.PowerGlove]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.TitanGlove);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Power Glove
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.PowerGlove]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.MechanicalGlove]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.PowerGlove);
                shop.item[nextSlot].shopCustomPrice = 2;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Mechanical Glove
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.MechanicalGlove]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.FireGauntlet]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.MechanicalGlove);
                shop.item[nextSlot].shopCustomPrice = 3;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Magma Stone
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.MagmaStone]
                && !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.FireGauntlet]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.MagmaStone);
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Fire Gauntlet
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>().itemChecklist[ItemID.FireGauntlet]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ItemID.FireGauntlet);
                shop.item[nextSlot].shopCustomPrice = 4;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }
            #endregion

            //TODO: Celestial Shell, Arctic Diving Gear

            #region Buccaneer Bag
            //Annihilator Rounds
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>()
                .itemChecklist[ModContent.ItemType<AnnihilatorRoundsAccessory>()]
                && (!Main.LocalPlayer.GetModPlayer<ModPlayerCostume>()
                .itemChecklist[ModContent.ItemType<BuccaneerBagAccessory>()] ||
                !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>()
                .itemChecklist[ModContent.ItemType<OperatorKitAccessory>()])
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<AnnihilatorRoundsAccessory>());
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Black Powder
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>()
                .itemChecklist[ModContent.ItemType<BlackPowderAccessory>()]
                && (!Main.LocalPlayer.GetModPlayer<ModPlayerCostume>()
                .itemChecklist[ModContent.ItemType<BuccaneerBagAccessory>()] ||
                !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>()
                .itemChecklist[ModContent.ItemType<OperatorKitAccessory>()])
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<BlackPowderAccessory>());
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Magic Pouch
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>()
                .itemChecklist[ModContent.ItemType<MagicPouchAccessory>()]
                && (!Main.LocalPlayer.GetModPlayer<ModPlayerCostume>()
                .itemChecklist[ModContent.ItemType<BuccaneerBagAccessory>()] ||
                !Main.LocalPlayer.GetModPlayer<ModPlayerCostume>()
                .itemChecklist[ModContent.ItemType<OperatorKitAccessory>()])
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<MagicPouchAccessory>());
                shop.item[nextSlot].shopCustomPrice = 1;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            //Buccaneer Bag
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>()
                .itemChecklist[ModContent.ItemType<BuccaneerBagAccessory>()]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<BuccaneerBagAccessory>());
                shop.item[nextSlot].shopCustomPrice = 2;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }
            #endregion

            //Operator's Kit
            if (Main.LocalPlayer.GetModPlayer<ModPlayerCostume>()
                .itemChecklist[ModContent.ItemType<OperatorKitAccessory>()]
                && nextSlot < 40)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<OperatorKitAccessory>());
                shop.item[nextSlot].shopCustomPrice = 2;
                shop.item[nextSlot].shopSpecialCurrency = LeftieTestMod.ReclaimerScripsCurrencyId;
                nextSlot++;
            }

            #endregion
        }

        public override void PostAI()
        {
            //Check if any new bosses have been killed, and if so, add them to the list
            //However, we take care to make sure the boss is not already in the list, as we don't want to add
            //every boss every frame and overflow the List
            #region Boss Kill Checks
            //Eye of Cthulu
            if (NPC.downedBoss1 && !validBosses.Contains(NPCID.EyeofCthulhu))
            {
                validBosses.Add(NPCID.EyeofCthulhu);
            }
            //Eater of Worlds
            if (DownedBossSystem.downedEaterOfWorlds && !validBosses.Contains(NPCID.EaterofWorldsHead))
            {
                validBosses.Add(NPCID.EaterofWorldsHead);
            }
            //Brain of Cthulu
            if (DownedBossSystem.downedBrainOfCthulu && !validBosses.Contains(NPCID.BrainofCthulhu))
            {
                validBosses.Add(NPCID.BrainofCthulhu);
            }
            //Skeletron
            if (NPC.downedBoss3 && !validBosses.Contains(NPCID.SkeletronHead))
            {
                validBosses.Add(NPCID.SkeletronHead);
            }
            //King Slime
            if (NPC.downedSlimeKing && !validBosses.Contains(NPCID.KingSlime))
            {
                validBosses.Add(NPCID.KingSlime);
            }
            //Queen Bee
            if (NPC.downedQueenBee && !validBosses.Contains(NPCID.QueenBee))
            {
                validBosses.Add(NPCID.QueenBee);
            }
            //Deerclops
            if (NPC.downedDeerclops && !validBosses.Contains(NPCID.Deerclops))
            {
                validBosses.Add(NPCID.Deerclops);
            }
            //Wall of Flesh
            if (DownedBossSystem.downedWallOfFlesh && !validBosses.Contains(NPCID.WallofFlesh))
            {
                validBosses.Add(NPCID.WallofFlesh);
            }

            //Queen Slime
            if (NPC.downedQueenSlime && !validBosses.Contains(NPCID.QueenSlimeBoss))
            {
                validBosses.Add(NPCID.QueenSlimeBoss);
            }
            //The Destroyer
            if (NPC.downedMechBoss1 && !validBosses.Contains(NPCID.TheDestroyer))
            {
                validBosses.Add(NPCID.TheDestroyer);
            }
            //The Twins
            if (NPC.downedMechBoss2 && !validBosses.Contains(NPCID.Retinazer))
            {
                validBosses.Add(NPCID.Retinazer); //We only really need to add one of the two twins, I'm not miffed by
                                                  //the possible outliers.
            }
            //Skeletron Prime
            if (NPC.downedMechBoss3 && !validBosses.Contains(NPCID.SkeletronPrime))
            {
                validBosses.Add(NPCID.SkeletronPrime);
            }
            //Plantera
            if (NPC.downedPlantBoss && !validBosses.Contains(NPCID.Plantera))
            {
                validBosses.Add(NPCID.Plantera);
            }
            //Golem
            if (NPC.downedGolemBoss && !validBosses.Contains(NPCID.Golem))
            {
                validBosses.Add(NPCID.Golem);
            }
            //Duke Fishron
            if (NPC.downedFishron && !validBosses.Contains(NPCID.DukeFishron))
            {
                validBosses.Add(NPCID.DukeFishron);
            }
            //Empress of Light
            if (NPC.downedEmpressOfLight && !validBosses.Contains(NPCID.HallowBoss))
            {
                validBosses.Add(NPCID.HallowBoss);
            }
            //Moon Lord
            if (NPC.downedMoonlord && !validBosses.Contains(NPCID.MoonLordCore))
            {
                validBosses.Add(NPCID.MoonLordCore);
            }
            #endregion

            //If it is currently daytime and the dayStarted bool is false, set both dayStarted and canGiveQuest to true
            if (Main.dayTime && !dayStarted)
            {
                dayStarted = true;
                canGiveQuest = true;
                //Randomly select a boss from the list of valid bosses
                currentQuest = new ReclaimerQuest(validBosses[Main.rand.Next(0, validBosses.Count)]);
                //uncheck the given quest status and reset expecting item ID to -1
                questGiven = false;
                expectingQuestItemId = -1;
            }
            //If it is currently nighttime, set dayStarted to false, just for functionality in the prior check
            else if (!Main.dayTime)
            {
                dayStarted = false;
                //If the current quest is null, yet a quest can be given somehow (most likely loading into a world
                //currently at night), generate a new quest
                if (currentQuest == null && canGiveQuest)
                {
                    currentQuest = new ReclaimerQuest(validBosses[Main.rand.Next(0, validBosses.Count)]);
                }
            }
        }

        ////Method to refresh Quest availability on the start of every day
        //public static void RefreshQuests()
        //{
        //    //If it is currently daytime and the dayStarted bool is false, set both dayStarted and canGiveQuest to true
        //    if (Main.dayTime && !dayStarted)
        //    {
        //        dayStarted = true;
        //        canGiveQuest = true;
        //        //Randomly select a boss from the list of valid bosses
        //        currentQuest = new ReclaimerQuest(Main.rand.Next(0, validBosses.Count));
        //    }
        //    //If it is currently nighttime, set dayStarted to false, just for functionality in the prior check
        //    else if (!Main.dayTime)
        //    {
        //        dayStarted = false;
        //    }
        //}

        public override bool CanGoToStatue(bool toKingStatue) => true;

        //Helper method to return an int representing the current world's "Boss Kill score"
        public int GetBossKillScore()
        {
            //Initialize a score counter
            int score = 0;
            //Run through the following checks:
            #region Boss Kill Checks
            //Eye of Cthulu
            if (NPC.downedBoss1)
            {
                score++;
            }
            //Eater of Worlds
            if (DownedBossSystem.downedEaterOfWorlds)
            {
                score++;
            }
            //Brain of Cthulu
            if (DownedBossSystem.downedBrainOfCthulu)
            {
                score++;
            }
            //Skeletron
            if (NPC.downedBoss3)
            {
                score++;
            }
            //King Slime
            if (NPC.downedSlimeKing)
            {
                score++;
            }
            //Queen Bee
            if (NPC.downedQueenBee)
            {
                score++;
            }
            //Deerclops
            if (NPC.downedDeerclops)
            {
                score++;
            }
            //Wall of Flesh
            if (DownedBossSystem.downedWallOfFlesh)
            {
                score++;
            }

            //Queen Slime
            if (NPC.downedQueenSlime)
            {
                score++;
            }
            //The Destroyer
            if (NPC.downedMechBoss1)
            {
                score++;
            }
            //The Twins
            if (NPC.downedMechBoss2)
            {
                score++; //We only really need to add one of the two twins, I'm not miffed by
                         //the possible outliers.
            }
            //Skeletron Prime
            if (NPC.downedMechBoss3)
            {
                score++;
            }
            //Plantera
            if (NPC.downedPlantBoss)
            {
                score++;
            }
            //Golem
            if (NPC.downedGolemBoss)
            {
                score++;
            }
            //Duke Fishron
            if (NPC.downedFishron)
            {
                score++;
            }
            //Empress of Light
            if (NPC.downedEmpressOfLight)
            {
                score++;
            }
            //Lunatic Cultist
            if (NPC.downedAncientCultist)
            {
                score++;
            }
            //Moon Lord
            if (NPC.downedMoonlord)
            {
                score++;
            }
            #endregion

            return score;
        }
    }
}
