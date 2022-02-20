using BlackMage.Projectiles;
using Terraria.ModLoader;

namespace BlackMage
{
	internal static class Spells
	{
		public static Spell Blizzard = new Spell("Blizzard", 180, 400, ModContent.ProjectileType<BlizzardProj>())
		{
			ElementStack = Elements.IceElement | Elements.OneStack,
		};

		public static Spell Fire = new Spell("Fire", 180, 800, ModContent.ProjectileType<FireProj>())
		{
			ElementStack = Elements.FireElement | Elements.OneStack,
		};

		/*public static Spell Blizzard2 = new Spell("Blizzard II", 100, 800, ModContent.ProjectileType<Blizzard2Proj>())
		{
			ElementStack = Elements.IceElement | Elements.FullStack,
		};*/

		public static Spell Scathe = new Spell("Scathe", 100, 800, ModContent.ProjectileType<ScatheProj>());

		/*public static Spell Fire2 = new Spell("Fire II", 100, 1500, ModContent.ProjectileType<Fire2Proj>())
		{
			ElementStack = Elements.FireElement | Elements.FullStack,
		};*/

		/*public static Spell Fire3 = new Spell("Fire III", 260, 2000, ModContent.ProjectileType<Fire3Proj>())
		{
			ElementStack = Elements.FireElement | Elements.FullStack,
		}*/

		/*public static Spell Blizzard3 = new Spell("Blizzard III", 260, 800,ModContent.ProjectileType<Blizzard3Proj>())
		{
			ElementStack = Elements.IceElement | Elements.FullStack,
		}*/

		/*public static Spell Freeze = new Spell("Freeze", 120, 1000, ModContent.ProjectileType<FreezeProj>())
		{
			ElementStack = Elements.IceElement | Elements.HeartStack,
			StackRequired = true,
		}*/

		/*public static Spell Flare = new Spell("Flare", 220, -1, ModContent.ProjectileType<FlareProj>())
		{
			ElementStack = Elements.FireElement | Elements.HeartStack,
			StackRequired = true,
		}*/

		/*public static Spell Blizzard4 = new Spell("Blizzard IV", 310, 800, ModContent.ProjectileType<Blizzard4Proj>())
		{
			ElementStack = Elements.IceElement | Elements.HeartStack,
			StackRequired = true,
		}*/

		/*public static Spell Fire4 = new Spell("Fire IV", 310, 800, ModContent.ProjectileType<Fire4Proj>())
		{
			ElementStack = Elements.FireElement | Elements.NoStack,
			StackRequired = true,
		}*/

		/*public static Spell Foul = new Spell("Foul", 560, 0, ModContent.ProjectileType<FoulProj>())
		{
			ElementStack = Elements.PolyglotElement | Elements.NoStack,
		}*/

		/*public static Spell Despair = new Spell("Despair", 340, -1, ModContent.ProjectileType<DespairProj>())
		{
			ElementStack = Elements.FireElement | Elements.FullStack,
			StackRequired = true,
		}*/

		/*public static Spell Xenoglossy = new Spell("Xenoglossy", 760, 0, ModContent.ProjectileType<XenoglossyProj>())
		{
			ElementStack = Elements.PolyglotElement | Elements.NoStack,
		}*/

		/*public static Spell HighFire2 = new Spell("High Fire II", 140, 1500, ModContent.ProjectileType<HighFire2Proj>())
		{
			ElementStack = Elements.FireElement | Elements.FullStack,
		}*/

		/*public static Spell HighBlizzard2 = new Spell("High Blizzard II", 140, 800, ModContent.ProjectileType<HighBlizzard2Proj>())
		{
			ElementStack = Elements.IceElement | Elements.FullStack,
		}*/

		/*public static Spell Paradox = new Spell("Paradox", 500, 1600, ModContent.ProjectileType<ParadoxProj>())
		{
			ElementStack = Elements.ParadoxElement | Elements.NoStack,
		}*/
	}
}
