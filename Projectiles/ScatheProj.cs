using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlackMage.Projectiles
{
	internal class ScatheProj : ModProjectile
	{
		private NPC  _target;
		private bool _alreadyHit;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Scathe");
			ProjectileID.Sets.Homing[projectile.type]                  = true;
			ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
		}

		public override void SetDefaults()
		{
			projectile.width               = Spell.SingleTargetSize;
			projectile.height              = Spell.SingleTargetSize;
			projectile.friendly            = true;
			projectile.magic               = true;
			projectile.light               = 0.1f;
			projectile.knockBack           = 0f;
			projectile.tileCollide         = false;
			projectile.penetrate           = -1;
			projectile.localNPCHitCooldown = -1;
		}

		public override bool? CanHitNPC(NPC target) => _target?.whoAmI == target.whoAmI;

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			damage = Spells.Scathe.Potency;
			if (Main.rand.Next(4) == 0)
				damage += Spells.Scathe.Potency;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			_alreadyHit         = true;
			projectile.timeLeft = 120;
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

			if (!foundTarget && !_alreadyHit)
			{
				projectile.Kill();
				return;
			}
			if (_target.active)
				projectile.Center = targetCenter;
		}
	}
}
