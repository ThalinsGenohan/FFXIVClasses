namespace BlackMage
{
	internal class Spell
	{
		public string   Name           { get; }
		public int      Potency        { get; }
		public int      MPCost         { get; }
		public Elements Element        { get; }
		public int      ElementalStack { get; }
		public int      Projectile     { get; }
		public bool     StackRequired  { get; }
		public int      Radius         { get; }

		public Spell(string name, int potency, int mpCost, Elements element, int elementalStack, int projectile, bool stackRequired = false, int radius = 0)
		{
			Name           = name;
			Potency        = potency;
			MPCost         = mpCost;
			Element        = element;
			ElementalStack = elementalStack;
			Projectile     = projectile;
			StackRequired  = stackRequired;
			Radius         = radius;
		}
	}
}
