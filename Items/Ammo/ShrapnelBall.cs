using LeftieTestMod.Projectiles;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod.Items.Ammo
{
    public class ShrapnelBall : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Explodes into smaller projectiles on hit");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 50;
        }

        public override void SetDefaults()
        {
            //Common Properties
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(silver: 8);
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Pink;
            Item.consumable = true;

            //Weapon Properties
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 18;
            Item.knockBack = 2.5f;
            Item.crit = -2;

            //Gun Properties
            Item.shoot = ModContent.ProjectileType<ShrapnelBallProjectile>();
            Item.ammo = ItemID.Cannonball;
        }

        public override void AddRecipes()
        {
            CreateRecipe(50)
                .AddIngredient(ItemID.Cannonball, 50)
                .AddIngredient(ItemID.ExplosivePowder)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
