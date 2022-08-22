using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod.Items.Quest
{
    public class EoCQuestItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            //TODO: Make an actual name for the item
            DisplayName.SetDefault("EoC Quest Item");
        }

        public override void SetDefaults()
        {
            //Common Properties
            Item.width = 30;
            Item.height = 32;
            Item.rare = ItemRarityID.Quest;
            Item.maxStack = 99;
            Item.questItem = true;
        }
    }
}