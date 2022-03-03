using System;

namespace BlackMage
{
	public class Spell
	{
		public enum ProjectileTypes
		{
			DirectHit        = 0,
			HomingProjectile = 1,
		}

		public const int SingleTargetSize = 5;
		public const int AoESize          = 40;

		// General properties
		public string Name           { get; }
		public int    Potency        { get; }
		public int    MPCost         { get; }
		public int    Projectile     => _getProjectile();
		public uint   Cooldown       { get; set; } = 0;
		public bool   GlobalCooldown { get; set; } = true;

		// Black Mage properties
		public byte ElementStack  { get; set; } = Elements.NoElement | Elements.NoStack;
		public bool StackRequired { get; set; } = false;

		private readonly Func<int> _getProjectile;

		public Spell(string name, int potency, int mpCost, Func<int> getProjectile)
		{
			Name       = name;
			Potency    = potency;
			MPCost     = mpCost;
			_getProjectile = getProjectile;
		}
	}
}
