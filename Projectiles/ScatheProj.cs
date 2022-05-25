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
			//ProjectileID.Sets.Homing[Projectile.type]                  = true;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.width               = Spell.SingleTargetSize;
			Projectile.height              = Spell.SingleTargetSize;
			Projectile.friendly            = true;
			Projectile.DamageType          = DamageClass.Magic;
			Projectile.light               = 0.1f;
			Projectile.knockBack           = 0f;
			Projectile.tileCollide         = false;
			Projectile.penetrate           = -1;
			Projectile.localNPCHitCooldown = -1;
		}

		public override bool? CanHitNPC(NPC target) => _target?.whoAmI == target.whoAmI;

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			damage = Spells.Scathe.Potency;
			if (Main.rand.NextBool(4))
				damage += Spells.Scathe.Potency;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			_alreadyHit         = true;
			Projectile.timeLeft = 120;
		}

		public override void AI()
		{
			Player  player       = Main.player[Projectile.owner];
			Vector2 targetCenter = Projectile.position;
			var     foundTarget  = false;

			if (player.HasMinionAttackTargetNPC)
			{
				NPC   npc     = Main.npc[player.MinionAttackTargetNPC];
				float between = Vector2.Distance(npc.Center, Projectile.Center);
				if (between < 2000f)
				{
					targetCenter = npc.Center;
					foundTarget  = true;
					_target      = npc;
				}
			}

			if (!foundTarget && !_alreadyHit)
			{
				Projectile.Kill();
				return;
			}
			if (_target.active)
				Projectile.Center = targetCenter;
		}
	}
}
