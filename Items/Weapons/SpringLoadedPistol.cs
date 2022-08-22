using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod.Items.Weapons
{
    public class SpringLoadedPistol : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spring-Loaded Pistol");
            Tooltip.SetDefault("It's like a DIY project!");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            //Common Properties
            Item.width = 38;
            Item.height = 26;
            Item.value = Item.buyPrice(silver: 3, copper: 50);
            Item.rare = ItemRarityID.White;

            //Use Properties
            Item.useTime = 35;
            Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;
            Item.UseSound = SoundID.Item11;

            //Weapon Properties
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 8;
            Item.knockBack = 0;
            Item.noMelee = true;

            //Gun Properties
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 4.75f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 5)
                .AddRecipeGroup(RecipeGroupID.IronBar, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
