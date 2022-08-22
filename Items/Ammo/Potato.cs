using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod.Items.Ammo
{
    public class Potato : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }

        public override void SetDefaults()
        {
            //Common Properties
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.placeStyle = 0;
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(copper: 25);
            Item.rare = ItemRarityID.White;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.PotatoHerb>();
            Item.noMelee = true;

            //Weapon Properties
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 12;
            Item.knockBack = 0;
            Item.crit = -2;

            //Gun Properties
            Item.shoot = ModContent.ProjectileType<Projectiles.PotatoProjectile>();
            Item.ammo = ItemID.Cannonball;
        }

        public override bool CanShoot(Player player)
        {
            if (Item.type == ModContent.ItemType<Potato>())
            {
                return false;
            }
            return base.CanShoot(player);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var line = new TooltipLine(Mod, "UseCase", "Ammo");
            tooltips.Add(line);
        }
    }
}
