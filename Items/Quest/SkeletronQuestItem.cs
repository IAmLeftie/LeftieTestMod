using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod.Items.Quest
{
    public class SkeletronQuestItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            //TODO: Make an actual name for the item
            DisplayName.SetDefault("Skeletron Quest Item");
        }

        public override void SetDefaults()
        {
            //Common Properties
            Item.width = 28;
            Item.height = 30;
            Item.rare = ItemRarityID.Quest;
            Item.maxStack = 99;
            Item.questItem = true;
        }
    }
}
