using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod.Items
{
    public class ReclaimerScrip : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Used to purchase items from the Reclaimer");
        }

        public override void SetDefaults()
        {
            //Common Properties
            Item.width = 20;
            Item.height = 26;
            Item.rare = ItemRarityID.LightPurple;
            Item.maxStack = 999;
        }
    }
}
