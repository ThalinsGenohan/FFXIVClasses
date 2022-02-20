using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace BlackMage.Items.Accessories.Crystals
{
	internal class BLMCrystalLv75 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Level 75 Soul of the Black Mage");
			Tooltip.SetDefault(
				"Upon the surface of this multi-aspected crystal are carved the myriad deeds of black mages from eras past.\n" +
				"Allows you to cast Despair.");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.accessory = true;
			item.value = Item.sellPrice(gold: 10);
			item.rare = ItemRarityID.Purple;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<BlackMagePlayer>().SoulCrystalLevel = 75;
		}

		public override int ChoosePrefix(UnifiedRandom rand) => 0;

		public override void AddRecipes()
		{
			var recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<BLMCrystalLv70>());
			recipe.AddIngredient(ItemID.BeetleHusk);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
