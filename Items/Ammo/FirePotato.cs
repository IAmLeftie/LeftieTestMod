using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod.Items.Ammo
{
    public class FirePotato : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Diablo Potato");
            Tooltip.SetDefault("Lights enemies on fire\n\"6.66M on the Scoville scale\"");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }

        public override void SetDefaults()
        {
            //Common Properties
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(copper: 9);
            Item.rare = ItemRarityID.Green;
            Item.maxStack = 999;
            Item.consumable = true;

            //Weapon Properties
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 11;
            Item.knockBack = 0;
            Item.crit = -2;

            //Gun Properties
            Item.shoot = ModContent.ProjectileType<Projectiles.FirePotatoProjectile>();
            Item.ammo = ItemID.Cannonball;
        }

        public override bool CanShoot(Player player)
        {
            if (Item.type == ModContent.ItemType<FirePotato>())
            {
                return false;
            }
            return base.CanShoot(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe(25)
                .AddIngredient<Potato>(25)
                .AddIngredient<BottledLava>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
