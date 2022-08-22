using LeftieTestMod.Projectiles;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod.Items.Ammo
{
    public class MortarShell : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Flies very far");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
        }

        public override void SetDefaults()
        {
            //Common Properties
            Item.width = 24;
            Item.height = 16;
            Item.value = Item.buyPrice(copper: 45);
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Lime;
            Item.consumable = true;

            //Weapon Properties
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 22;
            Item.knockBack = 3.5f;
            Item.crit = -2;

            //Gun Properties
            Item.shoot = ModContent.ProjectileType<MortarShellProjectile>();
            Item.ammo = ItemID.Cannonball;
        }
    }
}
