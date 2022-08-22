using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod.Items.Ammo
{
    public class PoisonPotato : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Poisoned Potato");
            Tooltip.SetDefault("Applies poison on hit");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }

        public override void SetDefaults()
        {
            //Common Properties
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(copper: 7);
            Item.rare = ItemRarityID.Blue;
            Item.maxStack = 999;
            Item.consumable = true;

            //Weapon Properties
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 10;
            Item.knockBack = 0;
            Item.crit = -2;

            //Gun Properties
            Item.shoot = ModContent.ProjectileType<Projectiles.PoisonPotatoProjectile>();
            Item.ammo = ItemID.Cannonball;
        }

        public override bool CanShoot(Player player)
        {
            if (Item.type == ModContent.ItemType<PoisonPotato>())
            {
                return false;
            }
            return base.CanShoot(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe(70)
                .AddIngredient<Potato>(70)
                .AddIngredient(ItemID.FlaskofPoison)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
