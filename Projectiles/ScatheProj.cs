using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlackMage.Projectiles
{
	internal class ScatheProj : ModProjectile
	{
		private NPC _target;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Scathe");
			ProjectileID.Sets.Homing[projectile.type]                  = true;
			ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
		}

		public override void SetDefaults()
		{
			projectile.width        = Spell.SingleTargetSize;
			projectile.height       = Spell.SingleTargetSize;
			projectile.friendly     = true;
			projectile.magic        = true;
			projectile.light        = 0.1f;
			projectile.knockBack    = 0f;
			projectile.tileCollide  = false;
		}

		public override bool? CanHitNPC(NPC target) => _target?.whoAmI == target.whoAmI;

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			damage += Spells.Scathe.Potency;
		}

		public override void AI()
		{
			Player  player       = Main.player[projectile.owner];
			Vector2 targetCenter = projectile.position;
			var     foundTarget  = false;

			// This code is required if your minion weapon has the targeting feature
			if (player.HasMinionAttackTargetNPC)
			{
				NPC   npc     = Main.npc[player.MinionAttackTargetNPC];
				float between = Vector2.Distance(npc.Center, projectile.Center);
				// Reasonable distance away so it doesn't target across multiple screens
				if (between < 2000f)
				{
					targetCenter = npc.Center;
					foundTarget  = true;
					_target      = npc;
				}
			}

			if (!foundTarget)
			{
				// This code is required either way, used for finding a target
				for (var i = 0; i < Main.maxNPCs; i++)
				{
					NPC npc = Main.npc[i];
					if (!npc.CanBeChasedBy()) continue;

					float npcDistance = Vector2.Distance(npc.Center, projectile.Center);
					bool  closest     = Vector2.Distance(projectile.Center, targetCenter) > npcDistance;

					if (!closest && foundTarget) continue;

					// Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
					// The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
					/*bool closeThroughWall = npcDistance < 100f;
					bool lineOfSight = Collision.CanHitLine(projectile.position,
					                                        projectile.width,
					                                        projectile.height,
					                                        npc.position,
					                                        npc.width,
					                                        npc.height);

					if (!lineOfSight && !closeThroughWall) continue;*/

					targetCenter = npc.Center;
					foundTarget  = true;
					_target      = npc;
				}
			}

			// MOVEMENT
			// Default movement parameters (here for attacking)
			const float speed   = 24f;
			const float inertia = 8f;

			if (!foundTarget) return;
			// Minion has a target: attack (here, fly towards the enemy)
			if (!(Vector2.Distance(projectile.Center, targetCenter) > 40f)) return;

			// The immediate range around the target (so it doesn't latch onto it when close)
			Vector2 direction = targetCenter - projectile.Center;
			direction.Normalize();
			direction           *= speed;
			projectile.velocity =  (projectile.velocity * (inertia - 1) + direction) / inertia;
		}
	}
}
