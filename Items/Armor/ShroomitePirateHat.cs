using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class ShroomitePirateHat : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shroomite Corsair");
            Tooltip.SetDefault("15% increased cannonball damage\n5% increased ranged critical strike chance");
            //We increase the actual cannonball damage in the projectiles themselves via ModifyHitNPC

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

            ArmorIDs.Head.Sets.DrawBackHair[Item.headSlot] = true;
        }

        public override void SetDefaults()
        {
            //Common Properties
            Item.width = 22;
            Item.height = 22;
            Item.value = Item.sellPrice(gold: 7, silver: 50);
            Item.rare = ItemRarityID.Yellow;
            //Armor Properties
            Item.defense = 11;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemID.ShroomiteBreastplate && legs.type == ItemID.ShroomiteLeggings;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Not moving puts you in stealth,\nIncreasing ranged ability and reducing chance for enemies to target you";
            player.shroomiteStealth = true;
            player.GetModPlayer<ModPlayerCostume>().hasShroomitePirateHat = true;
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ShroomiteBar, 12)
                .AddTile(TileID.MythrilAnvil) //Also works with Orichalcum anvil
                .Register();
        }
    }
}
