using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod.Items.Quest
{
    public class DeerclopsQuestItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            //TODO: Make an actual name for the item
            DisplayName.SetDefault("Deerclops Quest Item");
        }

        public override void SetDefaults()
        {
            //Common Properties
            Item.width = 36;
            Item.height = 32;
            Item.rare = ItemRarityID.Quest;
            Item.maxStack = 99;
            Item.questItem = true;
        }
    }
}
