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
    public class NanoShellProjectile : ModProjectile
    {
		internal bool seekMode = false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Nano Shell");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
		}

		public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 16;
			Projectile.aiStyle = 1;
			Projectile.friendly = true;
			Projectile.penetrate = 3;
			Projectile.localNPCHitCooldown = -1;
			Projectile.alpha = 0;

			AIType = ProjectileID.CannonballFriendly;
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

        public override void AI()
        {
            //Only act if seekMode == true
			if (seekMode)
            {
				float maxDetectRadius = 200f;
				float projSpeed = 10f;

				NPC closestNPC = FindClosestNPC(maxDetectRadius);

				if (closestNPC == null) 
				{
					//Deactivate seekMode, because we don't want the shells to keep searching
					seekMode = false;

					return; 
				}

				// If found, change the velocity of the projectile and turn it in the direction of the target
				// Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
				Projectile.velocity = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
				Projectile.rotation = Projectile.velocity.ToRotation();

				//Deactivate seekMode, because we don't want the shells to chase enemies
				seekMode = false;
			}

        }

		// Finding the closest NPC to attack within maxDetectDistance range
		// If not found then returns null
		public NPC FindClosestNPC(float maxDetectDistance)
		{
			NPC closestNPC = null;

			// Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
			float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

			// Loop through all NPCs(max always 200)
			for (int k = 0; k < Main.maxNPCs; k++)
			{
				NPC target = Main.npc[k];
				// Check if NPC able to be targeted. It means that NPC is
				// 1. active (alive)
				// 2. chaseable (e.g. not a cultist archer)
				// 3. max life bigger than 5 (e.g. not a critter)
				// 4. can take damage (e.g. moonlord core after all it's parts are downed)
				// 5. hostile (!friendly)
				// 6. not immortal (e.g. not a target dummy)
				if (target.CanBeChasedBy())
				{
					// The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
					float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

					// Check if it is within the radius
					if (sqrDistanceToTarget < sqrMaxDetectDistance)
					{
						sqrMaxDetectDistance = sqrDistanceToTarget;
						closestNPC = target;
					}
				}
			}

			return closestNPC;
		}

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
				SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

                // If the projectile hits the left or right side of the tile, reverse the X velocity
                if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                {
                    Projectile.velocity.X = -oldVelocity.X * 0.33f;
                }

                // If the projectile hits the top or bottom side of the tile, reverse the Y velocity
                if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                {
                    Projectile.velocity.Y = -oldVelocity.Y * 0.33f;
                }

				//Activate Seek Mode
				seekMode = true;
                //Find the nearest enemy, and redirect itself to them at the cost of only dealing 2/3 of it's damage
                //and exploding after contact
                Projectile.damage /= 3;
				Projectile.damage *= 2;
				Projectile.penetrate = 1;

			}

			return false;
		}

		public override void Kill(int timeLeft)
		{

			// This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
			Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
			SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

			//Create an explosion projectile at the center of the projectile
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
				ModContent.ProjectileType<CannonballExplosionProjectile>(), Projectile.damage, 0, Projectile.owner);

			//Try to replicate the explosion VFX of a cannonball
			//There seems to be 3 layers of VFX going on:
			//1. Large smoke clouds, moving out in all 4 diagonals (This is a type of gore apparently? internal IDs 61-63)
			//2. Smaller puffs of smoke that head out further than the main smoke clouds (DustID.Smoke)
			//3. Fire dusts which fly up and then fall down through gravity (Possible DustID.Torch)

			//1. Large smoke clouds
			//Establish the velocity at 5pi/4 with a magnitude of -2
			Vector2 goreVelocity = new Vector2(((-MathF.Sqrt(2)) / 2) * -2, (-MathF.Sqrt(2)) / 2);
			for (int i = 0; i < 4; i++)
			{
				//Rotate the goreVelocity by pi/4 twice to skip over every other radian, keeping our gores spawning at
				//the 4 diagonals
				goreVelocity = goreVelocity.RotatedBy(MathHelper.PiOver4);
				goreVelocity = goreVelocity.RotatedBy(MathHelper.PiOver4);
				//Create the gore
				Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, goreVelocity, Main.rand.Next(61, 64));
			}
			//2. Smaller dusts
			//Establish the velocity at (-2, 0)
			Vector2 smokeDustVelocity = new Vector2(-2, 0);
			for (int i = 0; i < 10; i++)
			{
				//Temporarily create a new velocity by rotating the existing one at random
				Vector2 tempSmokeDustVelocity = smokeDustVelocity;
				tempSmokeDustVelocity = tempSmokeDustVelocity.RotatedByRandom(MathHelper.TwoPi);
				//Also make a scale float and randomize it by a couple decimals
				float dustScale = 1 + Main.rand.NextFloat(-0.1f, 0.2f);

				//Create the dust
				Dust.NewDust(Projectile.Center, 4, 4, DustID.Smoke, tempSmokeDustVelocity.X, tempSmokeDustVelocity.Y,
					Scale: dustScale);
			}
			//3. Fire dusts
			//Very similar to #2, just a slower velocity
			Vector2 fireDustVelocity = new Vector2(-1, 0);
			for (int i = 0; i < 10; i++)
			{
				//Temporarily create a new velocity by rotating the existing one at random
				Vector2 tempFireDustVelocity = fireDustVelocity;
				tempFireDustVelocity = tempFireDustVelocity.RotatedByRandom(MathHelper.TwoPi);

				//Create the dust
				Dust.NewDust(Projectile.Center, 4, 4, DustID.Torch, tempFireDustVelocity.X, tempFireDustVelocity.Y);
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

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			if (Main.rand.NextBool(3))
			{
				target.AddBuff(BuffID.Confused, 180);
			}
			else
            {
				target.AddBuff(BuffID.Confused, 60);
            }
		}
    }
}
