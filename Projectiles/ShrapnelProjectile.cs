using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LeftieTestMod.Projectiles
{
    public class ShrapnelProjectile : ModProjectile
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Shrapnel");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
		}

		public override void SetDefaults()
		{
			Projectile.width = 6;
			Projectile.height = 6;
			//Projectile.aiStyle = 24;
			Projectile.aiStyle = 2;
			Projectile.friendly = true;
			Projectile.penetrate = 2;
			Projectile.localNPCHitCooldown = 2;
			Projectile.alpha = 0;
			Projectile.tileCollide = true;
			Projectile.scale = 1.2f;

			AIType = ProjectileID.CrystalShard;
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 5;
        }

        public override bool PreDraw(ref Color lightColor)
		{
			Main.instance.LoadProjectile(Projectile.type);
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

			// Redraw the projectile with the color not influenced by light
			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
			//for (int k = 0; k < Projectile.oldPos.Length; k++)
			//{
			//	Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
			//	Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
			//	Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
			//}

			return true;
		}

		public override void Kill(int timeLeft)
		{
			// This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
			Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
			//for (int i = 0; i < 10; i++)
			//{
			//	Vector2 randOffset = new Vector2(Main.rand.NextFloat(-5f, 23f), Main.rand.NextFloat(-5f, 23f));
			//	Vector2 offsetPosition = Projectile.position + randOffset;

			//	//Dust.QuickDust(offsetPosition, Color.LightYellow);
			//	Dust.NewDust(offsetPosition, 1, 1, DustID.Copper);
			//}
			//SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
		}

		//Increase damage if wearing Shroomite Pirate Hat
		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			//If the player is wearing the Shroomite Pirate Hat, add 15% of the projectile's damage to itself
			if (Main.player[Projectile.owner].GetModPlayer<ModPlayerCostume>().hasShroomitePirateHat)
			{
				damage += (int)(damage * 0.15f);
			}
		}
	}
}
