using LeftieTestMod.Items.Accessories;
using LeftieTestMod.Items.Armor;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace LeftieTestMod
{
    enum ModAccessoryID
    {
        BlackPowder,
        AnnihilatorRounds,
        MagicPouch,
        OperatorKit,
        BuccaneerBag
    }

    public class ModPlayerCostume : ModPlayer
    {
        public bool hasShroomitePirateHat;
        public bool hasBlackPowder;
        public bool hasAnnihilatorRounds;
        public bool hasMagicPouch;
        public bool hasOperatorKit;
        public bool hasBuccaneerBag;

        public Dictionary<int, bool> itemChecklist = new Dictionary<int, bool>();
        public TagCompound saveData;

        public override void Initialize()
        {
            saveData = new TagCompound();

            itemChecklist = new Dictionary<int, bool>()
            {
                //Buccaneer's Bag
                {ModContent.ItemType<AnnihilatorRoundsAccessory>(), false},
                {ModContent.ItemType<BlackPowderAccessory>(), false},
                {ModContent.ItemType<MagicPouchAccessory>(), false},
                {ModContent.ItemType<BuccaneerBagAccessory>(), false},

                //Operator's Kit
                {ModContent.ItemType<OperatorKitAccessory>(), false},

                //Ankh Shield
                {ItemID.CobaltShield, false},
                {ItemID.Blindfold, false},
                {ItemID.Vitamins, false},
                {ItemID.ArmorPolish, false},
                {ItemID.AdhesiveBandage, false},
                {ItemID.Bezoar, false},
                {ItemID.Nazar, false},
                {ItemID.Megaphone, false},
                {ItemID.TrifoldMap,false},
                {ItemID.FastClock, false},
                {ItemID.ObsidianShield, false},
                {ItemID.ArmorBracing, false},
                {ItemID.MedicatedBandage, false},
                {ItemID.CountercurseMantra, false},
                {ItemID.ThePlan, false},
                {ItemID.AnkhCharm, false},
                {ItemID.AnkhShield, false},

                //Arctic Diving Gear
                {ItemID.Flipper, false},
                {ItemID.DivingHelmet, false},
                {ItemID.DivingGear, false},
                {ItemID.JellyfishNecklace, false},
                {ItemID.JellyfishDivingGear,false},
                {ItemID.IceSkates, false},
                {ItemID.ArcticDivingGear, true},

                //Cell Phone
                {ItemID.DepthMeter, false},
                {ItemID.Compass, false},
                {ItemID.Radar, false},
                {ItemID.TallyCounter, false},
                {ItemID.LifeformAnalyzer, false},
                {ItemID.DPSMeter, false},
                {ItemID.Stopwatch, false},
                {ItemID.MetalDetector, false},
                {ItemID.FishermansGuide, false},
                {ItemID.WeatherRadio, false},
                {ItemID.Sextant, false},
                {ItemID.GPS, false},
                {ItemID.REK, false},
                {ItemID.GoblinTech, false},
                {ItemID.FishFinder, false},
                {ItemID.MagicMirror, false}, //For leniency we'll mark this when the Ice Mirror is detected too... somehow
                {ItemID.PDA, false},
                {ItemID.CellPhone, false},

                //Celestial Cuffs
                {ItemID.BandofRegeneration, false},
                {ItemID.BandofStarpower, false},
                {ItemID.Shackle, false},
                {ItemID.ManaRegenerationBand, false},
                {ItemID.MagicCuffs, false},
                {ItemID.CelestialMagnet, false},
                {ItemID.CelestialCuffs, false},

                //Celestial Emblem
                {ItemID.AvengerEmblem, false},
                {ItemID.CelestialEmblem, false},

                //Celestial Shell
                {ItemID.MoonCharm, false},
                {ItemID.NeptunesShell, false},
                {ItemID.MoonStone, false},
                {ItemID.SunStone, false},
                {ItemID.MoonShell, false},
                {ItemID.CelestialStone, false},
                {ItemID.CelestialShell, false},

                //Fire Gauntlet
                {ItemID.FeralClaws, false},
                {ItemID.TitanGlove, false},
                {ItemID.PowerGlove, false},
                {ItemID.MagmaStone, false},
                {ItemID.MechanicalGlove, false},
                {ItemID.FireGauntlet, false},

                //Master Ninja Gear
                {ItemID.ClimbingClaws, false},
                {ItemID.ShoeSpikes, false},
                {ItemID.TigerClimbingGear, false},
                {ItemID.Tabi, false},
                {ItemID.BlackBelt, false},
                {ItemID.MasterNinjaGear, false},

                //Recon Scope
                {ItemID.DestroyerEmblem, false},
                {ItemID.RifleScope, false},
                {ItemID.SniperScope, false},
                {ItemID.PutridScent, false},
                {ItemID.ReconScope, false},

                //Terraspark Boots
                {ItemID.HermesBoots, false}, //Also register this in the case of Sailfish/Dunerider/Flurry Boots
                {ItemID.Aglet, false},
                {ItemID.AnkletoftheWind, false},
                {ItemID.SpectreBoots, false},
                {ItemID.LavaCharm, false},
                {ItemID.LightningBoots, false},
                {ItemID.WaterWalkingBoots, false},
                {ItemID.ObsidianRose, false},
                {ItemID.MoltenCharm, false},
                {ItemID.LavaWaders, false},
                {ItemID.FrostsparkBoots, false},
                {ItemID.TerrasparkBoots, false},

                //Zenith (oh boy here we go)
                {ItemID.CopperShortsword, false},
                {ItemID.Starfury, false},
                {ItemID.EnchantedSword, false},
                {ItemID.BeeKeeper, false},
                {ItemID.Excalibur, false},
                {ItemID.TrueExcalibur, false},
                {ItemID.BrokenHeroSword, false},
                {ItemID.BladeofGrass, false},
                {ItemID.Muramasa, false},
                {ItemID.FieryGreatsword, false},
                {ItemID.LightsBane, false}, //Also register this in the case of Blood Butcherer, we'll differentiate later
                {ItemID.NightsEdge, false},
                {ItemID.TrueNightsEdge, false},
                {ItemID.TerraBlade, false},
                {ItemID.Seedler, false},
                {ItemID.TheHorsemansBlade, false},
                {ItemID.InfluxWaver, false},
                {ItemID.StarWrath, false},
                {ItemID.Meowmere, false},
                {ItemID.Zenith, false}
            };
    }

        public override void SaveData(TagCompound tag)
        {
            tag["itemChecklistNames"] = itemChecklist.Keys.ToList();
            tag["itemChecklistValues"] = itemChecklist.Values.ToList();
        }

        public override void LoadData(TagCompound tag)
        {
            var names = tag.Get<List<int>>("itemChecklistNames");
            var values = tag.Get<List<bool>>("itemChecklistValues");
            itemChecklist = names.Zip(values, (k, v) => new { Key = k, Value = v }).ToDictionary(x => x.Key, x => x.Value);
        }


        public override void UpdateEquips()
        {
            //Update capacity as accs are added!
            Dictionary<ModAccessoryID, bool> accFlagDictionary = new Dictionary<ModAccessoryID, bool>(5)
            {
                { ModAccessoryID.BlackPowder, false },
                { ModAccessoryID.AnnihilatorRounds, false },
                { ModAccessoryID.MagicPouch, false },
                { ModAccessoryID.OperatorKit, false },
                { ModAccessoryID.BuccaneerBag, false },
            };

            //Check for Shroomite Pirate Hat
            if (Player.armor[0].type == ModContent.ItemType<ShroomitePirateHat>())
            {
                hasShroomitePirateHat = true;
            }
            else
            {
                hasShroomitePirateHat = false;
            }
            //Check for Accesories

            //BUG: For some reason any extra acc slots dont work? look into it later, too lazy right now lol
            //Store an array of the player's current accessories, so were not constantly checking for every acc in every slot every frame
            int[] slots = new int[5 + Player.extraAccessorySlots];
            for (int i = 3; i < 7 + Player.extraAccessorySlots; i++)
            {
                //Put the accessory in this slot into its respective place in the array
                slots[i - 3] = Player.armor[i].type;
            }
            //Now, check the array for specific accessories, and tick the respecive flags if found
            //We wont apply directly as we search, rather we'll modify the flagDictionary and then use that
            foreach (int acc in slots)
            {
                if (acc == ModContent.ItemType<BlackPowderAccessory>())
                {
                    accFlagDictionary[ModAccessoryID.BlackPowder] = true;
                }
                else if (acc == ModContent.ItemType<AnnihilatorRoundsAccessory>())
                {
                    accFlagDictionary[ModAccessoryID.AnnihilatorRounds] = true;
                }
                else if (acc == ModContent.ItemType<MagicPouchAccessory>())
                {
                    accFlagDictionary[ModAccessoryID.MagicPouch] = true;
                }
                else if (acc == ModContent.ItemType<BuccaneerBagAccessory>())
                {
                    accFlagDictionary[ModAccessoryID.BuccaneerBag] = true;
                }
                else if (acc == ModContent.ItemType<OperatorKitAccessory>())
                {
                    accFlagDictionary[ModAccessoryID.OperatorKit] = true;
                }
            }
            //Finally, use the flagDictionary to enable and disable flags
            if (accFlagDictionary[ModAccessoryID.BlackPowder])
            {
                hasBlackPowder = true;
            }
            else { hasBlackPowder = false; }

            if (accFlagDictionary[ModAccessoryID.AnnihilatorRounds])
            {
                hasAnnihilatorRounds = true;
            }
            else { hasAnnihilatorRounds = false; }

            if (accFlagDictionary[ModAccessoryID.MagicPouch])
            {
                hasMagicPouch = true;
            }
            else { hasMagicPouch = false; }

            if (accFlagDictionary[ModAccessoryID.BuccaneerBag])
            {
                hasBuccaneerBag = true;
            }
            else { hasBuccaneerBag = false; }

            if (accFlagDictionary[ModAccessoryID.OperatorKit])
            {
                hasOperatorKit = true;
            }
            else { hasOperatorKit = false; }
            //Continue as new accs are made...
        }

        public override void PostUpdate()
        {
            //Check the player's inventory and crosscompare it with the itemChecklist
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                //For each item ID in the item checklist...
                foreach (int id in itemChecklist.Keys)
                {
                    //If the ID of the item in the current inv. slot matches the current item ID in the dictionary,
                    //set the Dictionary entry to true and break out of the foreach loop
                    if (Player.inventory[i].type == id)
                    {
                        itemChecklist[id] = true;
                        break;
                    }
                    //In the case of SPECIFICALLY the Magic and Ice Mirrors, if the current inv item ID is either mirror
                    //and the current checklist item ID is the Magic Mirror, count it anyways
                    else if (Player.inventory[i].type == ItemID.IceMirror && id == ItemID.MagicMirror)
                    {
                        itemChecklist[id] = true;
                        break;
                    }
                    //In the case of SPECIFICALLY the Herme's Boots and it's equivalents, if the current inv item ID is any
                    //equivalent  accessory and the current checklist item ID is the Hermes' Boots, count it anyways
                    else if ((Player.inventory[i].type == ItemID.FlurryBoots || Player.inventory[i].type == ItemID.SandBoots
                        || Player.inventory[i].type == ItemID.SailfishBoots) && id == ItemID.HermesBoots)
                    {
                        itemChecklist[id] = true;
                        break;
                    }
                    //In the case of SPECIFICALLY the Light's Bane and the Blood Butcherer, if the current inv item ID is the
                    //Blood Butcherer and the current checklist item ID is the Light's Bane, count it anyways
                    else if (Player.inventory[i].type == ItemID.BloodButcherer && id == ItemID.LightsBane)
                    {
                        itemChecklist[id] = true;
                        break;
                    }
                }
            }
        }

        //Helper method which returns a player's "Item Score" (how much of the item checklist has been filled out)
        public int GetItemScore()
        {
            //Initialize a tracker variable for the item score
            int score = 0;
            //Run through every entry in the provided player's itemChecklist, and for every entry that is true, add
            //1 to the score counter
            foreach(int id in itemChecklist.Keys)
            {
                if (Player.GetModPlayer<ModPlayerCostume>().itemChecklist[id])
                {
                    score++;
                }
            }
            return score;
        }
    }
}
