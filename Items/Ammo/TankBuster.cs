using LeftieTestMod.Projectiles;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod.Items.Ammo
{
    public class TankBuster : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Deals massive damage to a single target\nat the cost of air speed");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
        }

        public override void SetDefaults()
        {
            //Common Properties
            Item.width = 24;
            Item.height = 16;
            Item.value = Item.buyPrice(silver: 1, copper: 20);
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Lime;
            Item.consumable = true;

            //Weapon Properties
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 50;
            Item.knockBack = 5f;
            Item.crit = -2;

            //Gun Properties
            Item.shoot = ModContent.ProjectileType<TankBusterProjectile>();
            Item.ammo = ItemID.Cannonball;
        }

        public override void PickAmmo(Item weapon, Player player, ref int type, ref float speed, ref StatModifier damage, ref float knockback)
        {
             //Reduce the projectile speed to 2/3 of it's usual
             speed /= 3;
             speed *= 2;
        }
    }
}
