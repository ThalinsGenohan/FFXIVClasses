using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace BlackMage.Items.Accessories.Crystals
{
	internal class BLMCrystalLv20 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Level 20 Soul of the Black Mage");
			Tooltip.SetDefault(
				"Upon the surface of this multi-aspected crystal are carved the myriad deeds of black mages from eras past.\n" +
				"Allows you to cast Fire II and Blizzard II.\n" +
				"Allows the stacking of a second Astral Fire (1.6x damage) and Umbral Ice (4700 MP/2.5 sec, 0.5x cost).");
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
			player.GetModPlayer<BlackMagePlayer>().SoulCrystalLevel = 20;
		}

		public override int ChoosePrefix(UnifiedRandom rand) => 0;

		public override void AddRecipes()
		{
			var recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<BLMCrystalLv10>());
			recipe.AddIngredient(ItemID.Meteorite);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
