﻿using LeftieTestMod.Items.Ammo;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace LeftieTestMod.Tiles
{
    public enum PlantStage : byte
    {
        Planted,
        Growing,
        Grown
    }

    public class PotatoHerb : ModTile
    {
        private const int FrameWidth = 18;

        public override void SetStaticDefaults()
        {
			Main.tileFrameImportant[Type] = true;
			Main.tileObsidianKill[Type] = true;
			Main.tileCut[Type] = true;
			Main.tileNoFail[Type] = true;
			TileID.Sets.ReplaceTileBreakUp[Type] = true;
			TileID.Sets.IgnoredInHouseScore[Type] = true;
			TileID.Sets.IgnoredByGrowingSaplings[Type] = true;
			TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]); // Make this tile interact with golf balls in the same way other plants do

			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Potato");
			AddMapEntry(new Color(255, 174, 106), name);

			TileObjectData.newTile.CopyFrom(TileObjectData.StyleAlch);
			TileObjectData.newTile.AnchorValidTiles = new int[] {
				TileID.Grass,
				TileID.HallowedGrass
			};
			TileObjectData.newTile.AnchorAlternateTiles = new int[] {
				TileID.ClayPot,
				TileID.PlanterBox
			};
			TileObjectData.addTile(Type);

			HitSound = SoundID.Grass;
			DustType = DustID.Ambient_DarkBrown;
		}

		public override bool CanPlace(int i, int j)
		{
			Tile tile = Framing.GetTileSafely(i, j); // Safe way of getting a tile instance

			if (tile.HasTile)
			{
				int tileType = tile.TileType;
				if (tileType == Type)
				{
					PlantStage stage = GetStage(i, j); // The current stage of the herb

					// Can only place on the same herb again if it's grown already
					return stage == PlantStage.Grown;
				}
				else
				{
					// Support for vanilla herbs/grasses:
					if (Main.tileCut[tileType] || TileID.Sets.BreakableWhenPlacing[tileType] || tileType == TileID.WaterDrip || tileType == TileID.LavaDrip || tileType == TileID.HoneyDrip || tileType == TileID.SandDrip)
					{
						bool foliageGrass = tileType == TileID.Plants || tileType == TileID.Plants2;
						bool moddedFoliage = tileType >= TileID.Count && (Main.tileCut[tileType] || TileID.Sets.BreakableWhenPlacing[tileType]);
						bool harvestableVanillaHerb = Main.tileAlch[tileType] && WorldGen.IsHarvestableHerbWithSeed(tileType, tile.TileFrameX / 18);

						if (foliageGrass || moddedFoliage || harvestableVanillaHerb)
						{
							WorldGen.KillTile(i, j);
							if (!tile.HasTile && Main.netMode == NetmodeID.MultiplayerClient)
							{
								NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, i, j);
							}

							return true;
						}
					}

					return false;
				}
			}

			return true;
		}

		public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
		{
			if (i % 2 == 0)
			{
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
		}

		public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
		{
			offsetY = -2; // This is -1 for tiles using StyleAlch, but vanilla sets to -2 for herbs, which causes a slight visual offset between the placement preview and the placed tile. 
		}

		public override bool Drop(int i, int j)
		{
			PlantStage stage = GetStage(i, j);

			if (stage == PlantStage.Planted)
			{
				// Do not drop anything when just planted
				return false;
			}

			Vector2 worldPosition = new Vector2(i, j).ToWorldCoordinates();
			Player nearestPlayer = Main.player[Player.FindClosest(worldPosition, 16, 16)];

			int herbItemType = ModContent.ItemType<Potato>();
			int herbItemStack = 4;

			int seedItemType = ModContent.ItemType<Potato>();
			int seedItemStack = 0;

			if (nearestPlayer.active && nearestPlayer.HeldItem.type == ItemID.StaffofRegrowth)
			{
				// Increased yields with Staff of Regrowth, even when not fully grown
				herbItemStack = Main.rand.Next(6, 10);
				seedItemStack = 0;
			}
			else if (stage == PlantStage.Grown)
			{
				// Default yields, only when fully grown
				herbItemStack = 6;
				seedItemStack = 0;
			}

			var source = new EntitySource_TileBreak(i, j);

			if (herbItemType > 0 && herbItemStack > 0)
			{
				Item.NewItem(source, worldPosition, herbItemType, herbItemStack);
			}

			if (seedItemType > 0 && seedItemStack > 0)
			{
				Item.NewItem(source, worldPosition, seedItemType, seedItemStack);
			}

			// Custom drop code, so return false
			return false;
		}

		public override bool IsTileSpelunkable(int i, int j)
		{
			PlantStage stage = GetStage(i, j);

			// Only glow if the herb is grown
			return stage == PlantStage.Grown;
		}

		public override void RandomUpdate(int i, int j)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			PlantStage stage = GetStage(i, j);

			// Only grow to the next stage if there is a next stage. We don't want our tile turning pink!
			if (stage != PlantStage.Grown)
			{
				// Increase the x frame to change the stage
				tile.TileFrameX += FrameWidth;

				// If in multiplayer, sync the frame change
				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					NetMessage.SendTileSquare(-1, i, j, 1);
				}
			}
		}

		// A helper method to quickly get the current stage of the herb (assuming the tile at the coordinates is our herb)
		private static PlantStage GetStage(int i, int j)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			return (PlantStage)(tile.TileFrameX / FrameWidth);
		}
	}
}