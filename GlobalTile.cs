using BlackMage.Items.Weapons;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlackMage
{
	internal class GlobalTile : Terraria.ModLoader.GlobalTile
	{
		public override bool Drop(int i, int j, int type)
		{
			if (type == TileID.ShadowOrbs && Main.rand.NextBool(5))
			{
				Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 40, 40, ModContent.ItemType<Soulscourge>());
				return false;
			}
			return true;
		}
	}
}
