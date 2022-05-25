using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlackMage.Projectiles
{
	internal class FireProj : ModProjectile
	{
		private NPC _target;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fire");
			//ProjectileID.Sets.Homing[Projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.width       = Spell.SingleTargetSize;
			Projectile.height      = Spell.SingleTargetSize;
			Projectile.friendly    = true;
			Projectile.DamageType  = DamageClass.Magic;
			Projectile.light       = 0.5f;
			Projectile.knockBack   = 0f;
			Projectile.tileCollide = false;
		}

		public override bool? CanHitNPC(NPC target) => _target?.whoAmI == target.whoAmI;

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			damage += Spells.Fire.Potency;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			Vector2 targetCenter = Projectile.position;
			var foundTarget = false;
			
			if (player.HasMinionAttackTargetNPC)
			{
				NPC npc = Main.npc[player.MinionAttackTargetNPC];
				float between = Vector2.Distance(npc.Center, Projectile.Center);
				if (between < 2000f)
				{
					targetCenter = npc.Center;
					foundTarget = true;
					_target = npc;
				}
			}

			if (!foundTarget)
			{
				for (var i = 0; i < Main.maxNPCs; i++)
				{
					NPC npc = Main.npc[i];
					if (!npc.CanBeChasedBy()) continue;

					float npcDistance = Vector2.Distance(npc.Center, Projectile.Center);
					bool closest = Vector2.Distance(Projectile.Center, targetCenter) > npcDistance;

					if (!closest && foundTarget) continue;

					targetCenter = npc.Center;
					foundTarget = true;
					_target = npc;
				}
			}
			if (!foundTarget)
			{
				Projectile.Kill();
				return;
			}

			const float speed = 24f;
			const float inertia = 8f;
			
			if (!(Vector2.Distance(Projectile.Center, targetCenter) > 40f)) return;
			
			Vector2 direction = targetCenter - Projectile.Center;
			direction.Normalize();
			direction *= speed;
			Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
		}
	}
}
