using BlackMage.Items;
using BlackMage.Items.Weapons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlackMage
{
	internal class GlobalTile : Terraria.ModLoader.GlobalTile
	{
		public override bool Drop(int i, int j, int type)
		{
			if (type == TileID.ShadowOrbs && Main.rand.Next(5) == 0)
			{
				Item.NewItem(i * 16, j * 16, 40, 40, ModContent.ItemType<Soulscourge>());
				return false;
			}
			return true;
		}
	}
}
