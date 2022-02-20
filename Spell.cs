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
		public string Name       { get; }
		public int    Potency    { get; }
		public int    MPCost     { get; }
		public int    Projectile { get; }

		// Black Mage properties
		public byte ElementStack  { get; set; } = Elements.NoElement | Elements.NoStack;
		public bool StackRequired { get; set; } = false;

		public Spell(string name, int potency, int mpCost, int projectile)
		{
			Name       = name;
			Potency    = potency;
			MPCost     = mpCost;
			Projectile = projectile;
		}
	}
}
