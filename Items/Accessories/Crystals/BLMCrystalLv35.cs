using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace BlackMage.Items.Accessories.Crystals
{
	internal class BLMCrystalLv35 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Level 35 Soul of the Black Mage");
			Tooltip.SetDefault(
				"Upon the surface of this multi-aspected crystal are carved the myriad deeds of black mages from eras past.\n" +
				"Allows you to cast Fire III and Blizzard III.\n" +
				"Allows the stacking of a third Astral Fire (1.8x damage) or Umbral Ice (6200 MP/2.5 sec, 0x cost).");
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
			player.GetModPlayer<BlackMagePlayer>().SoulCrystalLevel = 35;
		}

		public override int ChoosePrefix(UnifiedRandom rand) => 0;

		public override void AddRecipes()
		{
			var recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<BLMCrystalLv20>());
			recipe.AddIngredient(ItemID.Hellstone);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
