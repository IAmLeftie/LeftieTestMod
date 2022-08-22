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
	public class PoisonPotatoProjectile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Poisoned Potato");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
		}

		public override void SetDefaults()
		{
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.aiStyle = 2;
			Projectile.friendly = true;
			Projectile.penetrate = 2;
			Projectile.localNPCHitCooldown = -1;
			Projectile.alpha = 255;

			AIType = ProjectileID.CannonballFriendly;
		}

		//public override string Texture => "LeftieTestMod/Projectiles/PotatoProjectile";

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			// If collide with tile, reduce the penetrate.
			// So the projectile can reflect at most 5 times
			Projectile.penetrate--;
			if (Projectile.penetrate <= 0)
			{
				Projectile.Kill();
			}
			else
			{
				Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
				for (int i = 0; i < 3; i++)
				{
					Vector2 randOffset = new Vector2(Main.rand.NextFloat(-3f, 21f), Main.rand.NextFloat(-3f, 21f));
					Vector2 offsetPosition = Projectile.position + randOffset;

					//Dust.QuickDust(offsetPosition, Color.LightYellow);
					Dust.NewDust(offsetPosition, 1, 1, DustID.Gold);
				}
				SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

				// If the projectile hits the left or right side of the tile, reverse the X velocity
				if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
				{
					Projectile.velocity.X = -oldVelocity.X * 0.05f;
				}

				// If the projectile hits the top or bottom side of the tile, reverse the Y velocity
				if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
				{
					Projectile.velocity.Y = -oldVelocity.Y * 0.05f;
				}
			}

			return false;
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
			for (int i = 0; i < 10; i++)
			{
				Vector2 randOffset = new Vector2(Main.rand.NextFloat(-5f, 23f), Main.rand.NextFloat(-5f, 23f));
				Vector2 offsetPosition = Projectile.position + randOffset;

				//Dust.QuickDust(offsetPosition, Color.LightYellow);
				Dust.NewDust(offsetPosition, 1, 1, DustID.Gold);
			}
			SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			if (Main.rand.NextBool(2, 5))
			{
				target.AddBuff(BuffID.Poisoned, 300);
			}
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
