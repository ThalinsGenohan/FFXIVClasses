using Terraria.ID;
using Terraria.ModLoader;

namespace BlackMage.Projectiles
{
	internal class BlizzardProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blizzard");
			ProjectileID.Sets.Homing[projectile.type]                  = true;
			ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
		}

		public override void SetDefaults()
		{
			projectile.width     = 10;
			projectile.height    = 10;
			projectile.friendly  = true;
			projectile.magic     = true;
			projectile.light     = 0.2f;
			projectile.knockBack = 0f;
		}
	}
}
