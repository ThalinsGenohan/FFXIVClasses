using Terraria.ModLoader;

namespace BlackMage.Projectiles
{
	internal class ScatheProj : ModProjectile
	{
		public override void SetDefaults()
		{
			projectile.width     = 20;
			projectile.height    = 20;
			projectile.friendly  = true;
			projectile.magic     = true;
			projectile.light     = 0.1f;
			projectile.knockBack = 0f;
			projectile.aiStyle   = -1;
		}
	}
}
