using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlackMage.Projectiles
{
	internal class BlizzardProj : ModProjectile
	{
		private NPC _target;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blizzard");
			ProjectileID.Sets.Homing[projectile.type]                  = true;
			ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
		}

		public override void SetDefaults()
		{
			projectile.width       = Spell.SingleTargetSize;
			projectile.height      = Spell.SingleTargetSize;
			projectile.friendly    = true;
			projectile.magic       = true;
			projectile.light       = 0.2f;
			projectile.knockBack   = 0f;
			projectile.tileCollide = false;
		}

		public override bool? CanHitNPC(NPC target) => _target?.whoAmI == target.whoAmI;

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			damage += Spells.Blizzard.Potency;
		}

		public override void AI()
		{
			Player  player       = Main.player[projectile.owner];
			Vector2 targetCenter = projectile.position;
			var     foundTarget  = false;
			
			if (player.HasMinionAttackTargetNPC)
			{
				NPC   npc     = Main.npc[player.MinionAttackTargetNPC];
				float between = Vector2.Distance(npc.Center, projectile.Center);
				if (between < 2000f)
				{
					targetCenter = npc.Center;
					foundTarget  = true;
					_target      = npc;
				}
			}

			if (!foundTarget)
			{
				projectile.Kill();
				return;
			}
			projectile.position = targetCenter;
		}
	}
}
