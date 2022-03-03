using BlackMage.Items.Accessories.Crystals;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlackMage
{
	internal class GlobalNPC : Terraria.ModLoader.GlobalNPC
	{
		public override void SetupShop(int type, Chest shop, ref int nextSlot)
		{
			if (type == NPCID.Clothier)
			{
				shop.item[nextSlot].SetDefaults(ModContent.ItemType<BLMCrystalLv10>());
				nextSlot++;
			}
		}
	}
}
