using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using LeftieTestMod.Items.Weapons;
using LeftieTestMod.Items.Ammo;

namespace LeftieTestMod.NPCs
{
    public class NPCShop : GlobalNPC
    {
        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {
            if (type == NPCID.GoblinTinkerer)
            {
                //Add the basic Potato Cannon to the shop
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<PotatoCannon>());
                nextSlot++;
                //As well as potatoes
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<Potato>());
            }

            if (type == NPCID.Cyborg)
            {
                //Add the Mortar Shell to the shop (We shouldnt need to add a condition for Plantera to be dead, because
                //the Cyborg only appears once Plantera is dead anyways)
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<MortarShell>());
                nextSlot++;

                //Add the Tank Buster if it is currently nighttime
                if (Main.dayTime == false)
                {
                    shop.item[nextSlot].SetDefaults(ModContent.ItemType<TankBuster>());
                    nextSlot++;
                }
            }
        }
    }
}
