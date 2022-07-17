using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;

namespace BlackMage;

public static class Constants
{
	public enum Elements : byte
	{
		NoElement        = 0,
		FireElement      = 1,
		IceElement       = 2,
		PolyglotElement  = 3,
		ParadoxElement   = 4,
		TransposeElement = 5,
	}

	public static Dictionary<string, string> Keywords = new()
	{
		{ "[MP]", $"[c/{Colors.MP.Hex3()}:MP]" },
		{ "[Astral Fire]", $"[c/{Colors.Fire.Hex3()}:Astral Fire]" },
		{ "[Enhanced Flare]", $"[c/{Colors.Fire.Hex3()}:Enhanced Flare]" },
		{ "[Ice]", $"[c/{Colors.Ice.Hex3()}:Ice]" },
		{ "[Umbral Ice]", $"[c/{Colors.Ice.Hex3()}:Umbral Ice]" },
		{ "[Umbral Heart]", $"[c/{Colors.Ice.Hex3()}:Umbral Heart]" },
		{ "[Umbral Hearts]", $"[c/{Colors.Ice.Hex3()}:Umbral Hearts]" },
		{ "[Polyglot]", $"[c/{Colors.Polyglot.Hex3()}:Polyglot]" },

		{ "[Fire]", $"[c/{Colors.Fire.Hex3()}:Fire]" },
		{ "[Fira]", $"[c/{Colors.Fire.Hex3()}:Fira]" },
		{ "[Firaga]", $"[c/{Colors.Fire.Hex3()}:Firaga]" },
		{ "[Firaja]", $"[c/{Colors.Fire.Hex3()}:Firaja]" },
		{ "[Flare]", $"[c/{Colors.Fire.Hex3()}:Flare]" },
		{ "[Despair]", $"[c/{Colors.Fire.Hex3()}:Despair]" },

		{ "[Blizzard]", $"[c/{Colors.Ice.Hex3()}:Blizzard]" },
		{ "[Blizzara]", $"[c/{Colors.Ice.Hex3()}:Blizzara]" },
		{ "[Blizzaga]", $"[c/{Colors.Ice.Hex3()}:Blizzaga]" },
		{ "[Blizzaja]", $"[c/{Colors.Ice.Hex3()}:Blizzaja]" },
		{ "[Freeze]", $"[c/{Colors.Ice.Hex3()}:Freeze]" },
		{ "[Umbral Soul]", $"[c/{Colors.Ice.Hex3()}:Umbral Soul]" },

		{ "[Foul]", $"[c/{Colors.Polyglot.Hex3()}:Foul]" },
		{ "[Xenoglossy]", $"[c/{Colors.Polyglot.Hex3()}:Xenoglossy]" },

		{
			"[Paradox]", $"[c/{Color.Lerp(Colors.Fire, Colors.Ice, 0f / 6f).Hex3()}:P]" +
			             $"[c/{Color.Lerp(Colors.Fire, Colors.Ice, 1f / 6f).Hex3()}:a]" +
			             $"[c/{Color.Lerp(Colors.Fire, Colors.Ice, 2f / 6f).Hex3()}:r]" +
			             $"[c/{Color.Lerp(Colors.Fire, Colors.Ice, 3f / 6f).Hex3()}:a]" +
			             $"[c/{Color.Lerp(Colors.Fire, Colors.Ice, 4f / 6f).Hex3()}:d]" +
			             $"[c/{Color.Lerp(Colors.Fire, Colors.Ice, 5f / 6f).Hex3()}:o]" +
			             $"[c/{Color.Lerp(Colors.Fire, Colors.Ice, 6f / 6f).Hex3()}:x]"
		},

		{
			"[Transpose]", $"[c/{Color.Lerp(Colors.Fire, Colors.Ice, 0f / 8f).Hex3()}:T]" +
			               $"[c/{Color.Lerp(Colors.Fire, Colors.Ice, 1f / 8f).Hex3()}:r]" +
			               $"[c/{Color.Lerp(Colors.Fire, Colors.Ice, 2f / 8f).Hex3()}:a]" +
			               $"[c/{Color.Lerp(Colors.Fire, Colors.Ice, 3f / 8f).Hex3()}:n]" +
			               $"[c/{Color.Lerp(Colors.Fire, Colors.Ice, 4f / 8f).Hex3()}:s]" +
			               $"[c/{Color.Lerp(Colors.Fire, Colors.Ice, 5f / 8f).Hex3()}:p]" +
			               $"[c/{Color.Lerp(Colors.Fire, Colors.Ice, 6f / 8f).Hex3()}:o]" +
			               $"[c/{Color.Lerp(Colors.Fire, Colors.Ice, 7f / 8f).Hex3()}:s]" +
			               $"[c/{Color.Lerp(Colors.Fire, Colors.Ice, 8f / 8f).Hex3()}:e]"
		},
	};

	public static string ReplaceKeywords(string inStr)
	{
		return Keywords.Aggregate(inStr, (current, keyword) => current.Replace(keyword.Key, keyword.Value));
	}

	public static class Colors
	{
		public static readonly Color BlackMage           = new(0x60, 0x46, 0x88);
		public static readonly Color MPTopDark           = new(0xaf, 0x2c, 0x64);
		public static readonly Color MPTopLight          = new(0xfb, 0x79, 0xb9);
		public static readonly Color MPBottomDark        = new(0x71, 0x04, 0x35);
		public static readonly Color MPBottomLight       = new(0xdb, 0x48, 0x86);
		public static readonly Color MP                  = new(0xbd, 0x3c, 0x76);
		public static readonly Color Fire                = new(0xff, 0x8f, 0x8e);
		public static readonly Color Ice                 = new(0x93, 0xc9, 0xff);
		public static readonly Color Polyglot            = new(0x60, 0x46, 0x88);
		public static readonly Color PolyglotTopDark     = new(0x7b, 0x44, 0x8c);
		public static readonly Color PolyglotTopLight    = new(0xdd, 0x99, 0xf0);
		public static readonly Color PolyglotBottomDark  = new(0x4a, 0x17, 0x5b);
		public static readonly Color PolyglotBottomLight = new(0xac, 0x6b, 0xbf);
	}
}
