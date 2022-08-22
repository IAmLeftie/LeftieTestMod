using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod.Items
{
    public class BottledLava : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("How is it not melting?");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 30;
        }

        public override void SetDefaults()
        {
            //Common Properties
            Item.width = 20;
            Item.height = 26;
            Item.value = Item.sellPrice(copper: 12);
            Item.rare = ItemRarityID.White;
            Item.maxStack = 999;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Bottle)
                .AddCondition(Recipe.Condition.NearLava)
                .Register();
        }
    }
}
