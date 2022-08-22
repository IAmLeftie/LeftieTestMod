using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod.Items.Ammo
{
    public class Cannonball : GlobalItem
    {
        public override void SetDefaults(Item item)
        {
            if (item.type == ItemID.Cannonball)
            {
                item.shoot = ProjectileID.CannonballFriendly;
                item.DamageType = DamageClass.Ranged;
                item.ammo = ItemID.Cannonball;
                item.useStyle = ItemUseStyleID.Swing;
                item.useTurn = true;
                item.useAnimation = 20;
                item.useTime = 20;
                item.autoReuse = true;
                item.maxStack = 999;
                item.consumable = true;
                item.width = 12;
                item.height = 12;
                item.damage = 15;
                item.knockBack = 1.5f;
                item.noMelee = true;
                item.value = Item.buyPrice(copper: 12);
                item.rare = ItemRarityID.LightRed;
                item.crit = -2;
            }
        }

        //public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        //{
        //    if (item.type == ItemID.Cannonball)
        //    {
        //        var line = new TooltipLine(Mod, "ToolTipNew", "The potato cannon, too!");
        //        tooltips.Add(line);
        //    }
        //}

        public override bool CanShoot(Item item, Player player)
        {
            if (item.type == ItemID.Cannonball)
            {
                return false;
            }
            return base.CanShoot(item, player);
        }

        //Increase damage if wearing Shroomite Pirate Hat
        public override void ModifyHitNPC(Item item, Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            if (Main.player[player.whoAmI].GetModPlayer<ModPlayerCostume>().hasShroomitePirateHat)
            {
                damage += (int)(damage * 0.15f);
            }
        }
    }
}
