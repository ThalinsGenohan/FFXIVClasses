using BlackMage.Projectiles;
using Terraria.ModLoader;

namespace BlackMage
{
	internal static class Spells
	{
		public static Spell Blizzard = new Spell("Blizzard",
		                                         180,
		                                         400,
		                                         Elements.Ice,
		                                         -1,
		                                         ModContent.ProjectileType<BlizzardProj>());

		public static Spell Fire = new Spell("Fire",
		                                     180,
		                                     800,
		                                     Elements.Fire,
		                                     1,
		                                     ModContent.ProjectileType<FireProj>());

		/*public static Spell Blizzard2 = new Spell("Blizzard II",
		                                      100,
		                                      800,
		                                      Elements.Ice,
		                                      -3,
		                                      ModContent.ProjectileType<Blizzard2Proj>(),
		                                      false,
		                                      5);*/

		public static Spell Scathe = new Spell("Scathe",
		                                       100,
		                                       800,
		                                       Elements.None,
		                                       0,
		                                       ModContent.ProjectileType<ScatheProj>());

		/*public static Spell Fire2 = new Spell("Fire II",
		                                      100,
		                                      1500,
		                                      Elements.Fire,
		                                      3,
		                                      ModContent.ProjectileType<Fire2Proj>(),
		                                      false,
		                                      5);*/

		/*public static Spell Fire3 = new Spell("Fire III",
		                                      260,
		                                      2000,
		                                      Elements.Fire,
		                                      3,
		                                      ModContent.ProjectileType<Fire3Proj>());*/

		/*public static Spell Blizzard3 = new Spell("Blizzard III",
		                                      260,
		                                      800,
		                                      Elements.Ice,
		                                      -3,
		                                      ModContent.ProjectileType<Blizzard3Proj>());*/

		/*public static Spell Freeze = new Spell("Freeze",
		                                          120,
		                                          1000,
		                                          Elements.Ice,
		                                          -6,
		                                          ModContent.ProjectileType<FreezeProj>(),
		                                          true,
		                                          5);*/

		/*public static Spell Flare = new Spell("Flare",
		                                       220,
		                                       -1,
		                                       Elements.Fire,
		                                       6,
		                                       ModContent.ProjectileType<FlareProj>(),
		                                       true,
		                                       5);*/

		/*public static Spell Blizzard4 = new Spell("Blizzard IV",
		                                          310,
		                                          800,
		                                          Elements.Ice,
		                                          -6,
		                                          ModContent.ProjectileType<Blizzard4Proj>(),
		                                          true);*/

		/*public static Spell Fire4 = new Spell("Fire IV",
		                                      310,
		                                      800,
		                                      Elements.Fire,
		                                      0,
		                                      ModContent.ProjectileType<Fire4Proj>(),
		                                      true);*/

		/*public static Spell Foul = new Spell("Foul",
		                                     560,
		                                     0,
		                                     Elements.Polyglot,
		                                     0,
		                                     ModContent.ProjectileType<FoulProj>(),
		                                     false,
		                                     5);*/

		/*public static Spell Despair = new Spell("Despair",
		                                        340,
		                                        -1,
		                                        Elements.Fire,
		                                        3,
		                                        ModContent.ProjectileType<DespairProj>(),
		                                        true);*/

		/*public static Spell Xenoglossy = new Spell("Xenoglossy",
		                                           760,
		                                           0,
		                                           Elements.Polyglot,
		                                           0,
		                                           ModContent.ProjectileType<XenoglossyProj>());*/

		/*public static Spell HighFire2 = new Spell("High Fire II",
		                                          140,
		                                          1500,
		                                          Elements.Fire,
		                                          3,
		                                          ModContent.ProjectileType<HighFire2Proj>(),
		                                          false,
		                                          5);*/

		/*public static Spell HighBlizzard2 = new Spell("High Blizzard II",
		                                              140,
		                                              800,
		                                              Elements.Ice,
		                                              -3,
		                                              ModContent.ProjectileType<HighBlizzard2Proj>(),
		                                              false,
		                                              5);*/

		/*public static Spell Paradox = new Spell("Paradox",
		                                        500,
		                                        1600,
		                                        Elements.Paradox,
		                                        0,
		                                        ModContent.ProjectileType<ParadoxProj>());*/
	}
}
