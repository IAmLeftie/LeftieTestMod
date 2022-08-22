using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod.Items
{
    public class GlobalItemModifications : GlobalItem
    {
        public override bool CanConsumeAmmo(Item weapon, Item ammo, Player player)
        {
            //Check if the player has the Buccaneer Bag accessory equipped
            if (player.GetModPlayer<ModPlayerCostume>().hasBuccaneerBag)
            {
                //IF they have this accessory equipped, and the passed-in weapon fires cannonballs...
                if (weapon.useAmmo == ItemID.Cannonball)
                {
                    //Run an RNG check, and if it passes, return false
                    if (Main.rand.NextFloat() <= 0.15f)
                    {
                        //Main.NewText("Buccaneer's Bag has procced!");
                        return false;
                    }
                    //Otherwise, return true and consume the ammo
                    else { return true; }
                }
            }

            //Check if the player has the Operator Kit accessory equipped
            if (player.GetModPlayer<ModPlayerCostume>().hasOperatorKit)
            {
                //IF they have this accessory equipped, and the passed-in weapon fires bullets...
                if (weapon.useAmmo == AmmoID.Bullet)
                {
                    //Run an RNG check, and if it passes, return false
                    if (Main.rand.NextFloat() <= 0.15f)
                    {
                        //Main.NewText("Operator's Kit has procced!");
                        return false;
                    }
                    //Otherwise, return true and consume the ammo
                    else { return true; }
                }
            }

            //Check if the player has the Magic Pouch accessory equipped
            if (player.GetModPlayer<ModPlayerCostume>().hasMagicPouch
                && !player.GetModPlayer<ModPlayerCostume>().hasBuccaneerBag
                && !player.GetModPlayer<ModPlayerCostume>().hasBuccaneerBag)
            {
                //IF they have this accessory equipped, and the passed-in weapon fires bullets or cannonballs...
                if (weapon.useAmmo == AmmoID.Bullet || weapon.useAmmo == ItemID.Cannonball)
                {
                    //Run an RNG check, and if it passes, return false
                    if (Main.rand.NextFloat() <= 0.10f)
                    {
                        //Main.NewText("Magic Pouch has procced!");
                        return false;
                    }
                    //Otherwise, return true and consume the ammo
                    else { return true; }
                }
            }

            //At the end, be sure to return the base function if we still haven't returned out
            return base.CanConsumeAmmo(weapon, ammo, player);
        }

        //public override bool? UseItem(Item item, Player player)
        //{
        //    if (player.GetModPlayer<ModPlayerCostume>().hasBuccaneerBag)
        //    {
        //        if (item.useAmmo == ItemID.Cannonball)
        //        {
        //            //Temporarily store the old values to change them back later
        //            int oldUseTime = item.useTime;
        //            int oldUseAnimation = item.useAnimation;

        //            item.useTime -= (int)(item.useTime * 0.15f);
        //            item.useAnimation -= (int)(item.useAnimation * 0.15f);
        //            Main.NewText("Cannon use time is " + item.useTime);
        //        }
        //    }

        //    else if (item.useAmmo == ItemID.Cannonball)
        //    {
        //        Main.NewText("Cannon use time is " + item.useTime);
        //    }
        //    return null;
        //}
    }
}
