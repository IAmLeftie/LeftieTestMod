using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod.Items.Consumables
{
    public class BakedPotatoFood : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Baked Potato");
            Tooltip.SetDefault("{$CommonItemTooltip.MinorStats}");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;

            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));

            ItemID.Sets.FoodParticleColors[Item.type] = new Color[3]
            {
                new Color(255, 194, 132),
                new Color(255, 251, 149),
                new Color(255, 212, 80)
            };

            ItemID.Sets.IsFood[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(26, 25, BuffID.WellFed, 28800);
            Item.value = Item.sellPrice(silver: 4, copper: 20);
            Item.rare = ItemRarityID.Green;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Ammo.Potato>()
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}
