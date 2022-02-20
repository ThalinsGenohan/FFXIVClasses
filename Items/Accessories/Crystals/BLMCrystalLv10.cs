using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace BlackMage.Items.Accessories.Crystals
{
	internal class BLMCrystalLv10 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Level 10 Soul of the Black Mage");
			Tooltip.SetDefault(
				"Upon the surface of this multi-aspected crystal are carved the myriad deeds of black mages from eras past.\n" +
				"Allows you to cast Fire and Blizzard.\n" +
				"Unlocks MP and the Elemental Gauge.\n" +
				"MP is used for Black Mage spells, and regenerates at a rate of 200 MP every 2.5 seconds, up to a max of 10000.\n" +
				"Astral Fire grants a 1.4x damage increase to Fire spells, but a 2x MP cost, and halts MP regeneration.\n" +
				"Umbral Ice increases MP regeneration to 3200 every 2.5 seconds, and lowers the MP cost of Ice spells to 0.75x.\n" +
				"While under the effect of Astral Fire or Umbral Ice, casting a spell of the opposite element will consume no MP.");
		}

		public override void SetDefaults()
		{
			item.width     = 20;
			item.height    = 20;
			item.accessory = true;
			item.value     = Item.sellPrice(gold: 10);
			item.rare      = ItemRarityID.Purple;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<BlackMagePlayer>().SoulCrystalLevel = 10;
		}

		public override int ChoosePrefix(UnifiedRandom rand) => 0;
	}
}
