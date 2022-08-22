using LeftieTestMod.Projectiles;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod.Items.Ammo
{
    public class LuminiteShell : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Explodes into smaller shells");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
        }

        public override void SetDefaults()
        {
            //Common Properties
            Item.width = 24;
            Item.height = 16;
            Item.value = Item.sellPrice(copper: 2);
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Cyan;
            Item.consumable = true;

            //Weapon Properties
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 36;
            Item.knockBack = 7f;

            //Gun Properties
            Item.shoot = ModContent.ProjectileType<LuminiteShellProjectile>();
            Item.ammo = ItemID.Cannonball;
        }

        public override void AddRecipes()
        {
            CreateRecipe(333)
                .AddIngredient(ItemID.LunarBar)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
