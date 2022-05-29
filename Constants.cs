using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;

namespace BlackMage
{
	internal static class Constants
	{
		internal static class Colors
		{
			public static readonly Color BlackMage = new Color(0x60, 0x46, 0x88);
			public static readonly Color MPDark    = new Color(0x8c, 0x30, 0x53);
			public static readonly Color MPLight   = new Color(0xca, 0x6d, 0x85);
			public static readonly Color MP        = new Color(0xfb, 0x60, 0xae);
			public static readonly Color Fire      = new Color(0xff, 0x8f, 0x8e);
			public static readonly Color Ice       = new Color(0x93, 0xc9, 0xff);
			public static readonly Color Polyglot  = new Color(0x60, 0x46, 0x88);
		}

		internal static string ReplaceKeywords(string inStr)
		{
			return Keywords.Aggregate(inStr, (current, keyword) => current.Replace(keyword.Key, keyword.Value));
		}

		internal static Dictionary<string, string> Keywords = new Dictionary<string, string>()
		{
			{ "[Fire]", $"[c/{Colors.Fire.Hex3()}:Fire]" },
			{ "[Fira]", $"[c/{Colors.Fire.Hex3()}:Fira]" },
			{ "[Firaga]", $"[c/{Colors.Fire.Hex3()}:Firaga]" },
			{ "[Firaja]", $"[c/{Colors.Fire.Hex3()}:Firaja]" },
			{ "[Flare]", $"[c/{Colors.Fire.Hex3()}:Flare]" },
			{ "[Despair]", $"[c/{Colors.Fire.Hex3()}:Despair]" },
		};

		internal static class Strings
		{
			public static readonly string Fire1   = $"[c/{Colors.Fire.Hex3()}:Fire]";
			public static readonly string Fire2   = $"[c/{Colors.Fire.Hex3()}:Fira]";
			public static readonly string Fire3   = $"[c/{Colors.Fire.Hex3()}:Firaga]";
			public static readonly string Fire4   = $"[c/{Colors.Fire.Hex3()}:Firaja]";
			public static readonly string Flare   = $"[c/{Colors.Fire.Hex3()}:Flare]";
			public static readonly string Despair = $"[c/{Colors.Fire.Hex3()}:Despair]";

			public static readonly string Blizzard1 = $"[c/{Colors.Ice.Hex3()}:Blizzard]";
			public static readonly string Blizzard2 = $"[c/{Colors.Ice.Hex3()}:Blizzara]";
			public static readonly string Blizzard3 = $"[c/{Colors.Ice.Hex3()}:Blizzaga]";
			public static readonly string Blizzard4 = $"[c/{Colors.Ice.Hex3()}:Blizzaja]";
			public static readonly string Freeze    = $"[c/{Colors.Ice.Hex3()}:Freeze]";

			public static readonly string Foul       = $"[c/{Colors.Polyglot.Hex3()}:Foul]";
			public static readonly string Xenoglossy = $"[c/{Colors.Polyglot.Hex3()}:Xenoglossy]";

			public static readonly string Paradox = $"[c/{Color.Lerp(Colors.Fire, Colors.Ice, 0f / 6f).Hex3()}:P]" +
			                                        $"[c/{Color.Lerp(Colors.Fire, Colors.Ice, 1f / 6f).Hex3()}:a]" +
			                                        $"[c/{Color.Lerp(Colors.Fire, Colors.Ice, 2f / 6f).Hex3()}:r]" +
			                                        $"[c/{Color.Lerp(Colors.Fire, Colors.Ice, 3f / 6f).Hex3()}:a]" +
			                                        $"[c/{Color.Lerp(Colors.Fire, Colors.Ice, 4f / 6f).Hex3()}:d]" +
			                                        $"[c/{Color.Lerp(Colors.Fire, Colors.Ice, 5f / 6f).Hex3()}:o]" +
			                                        $"[c/{Color.Lerp(Colors.Fire, Colors.Ice, 6f / 6f).Hex3()}:x]";

			public static readonly string MP           = $"[c/{Colors.MP.Hex3()}:MP]";
			public static readonly string AstralFire   = $"[c/{Colors.Fire.Hex3()}:Astral Fire]";
			public static readonly string UmbralIce    = $"[c/{Colors.Ice.Hex3()}:Umbral Ice]";
			public static readonly string UmbralHearts = $"[c/{Colors.Ice.Hex3()}:Umbral Hearts]";
			public static readonly string Polyglot     = $"[c/{Colors.Polyglot.Hex3()}:Polyglot]";
		}
	}
}
