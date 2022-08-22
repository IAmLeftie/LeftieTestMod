using LeftieTestMod.Projectiles;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod.Items.Ammo
{
    public class NanoShell : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Causes confusion and bounces back after hitting a wall");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
        }

        public override void SetDefaults()
        {
            //Common Properties
            Item.width = 24;
            Item.height = 16;
            Item.value = Item.buyPrice(silver: 1, copper: 20);
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Yellow;
            Item.consumable = true;

            //Weapon Properties
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 26;
            Item.knockBack = 4.5f;
            Item.crit = -2;

            //Gun Properties
            Item.shoot = ModContent.ProjectileType<NanoShellProjectile>();
            Item.ammo = ItemID.Cannonball;
        }

        public override void AddRecipes()
        {
            CreateRecipe(50)
                .AddIngredient(ModContent.ItemType<MortarShell>(), 50)
                .AddIngredient(ItemID.Nanites)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
