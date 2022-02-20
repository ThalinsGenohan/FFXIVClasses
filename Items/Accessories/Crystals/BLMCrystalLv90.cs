using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace BlackMage.Items.Accessories.Crystals
{
	internal class BLMCrystalLv90 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Level 90 Soul of the Black Mage");
			Tooltip.SetDefault(
				"Upon the surface of this multi-aspected crystal are carved the myriad deeds of black mages from eras past.\n" +
				"Allows you to cast Paradox.\n" +
				"Paradox replaces Fire and Blizzard upon switching from Astral Fire 3 to Umbral Ice, or from Umbral Ice 3 with 3 Umbral Hearts to Astral Fire.");
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
			player.GetModPlayer<BlackMagePlayer>().SoulCrystalLevel = 90;
		}

		public override int ChoosePrefix(UnifiedRandom rand) => 0;

		public override void AddRecipes()
		{
			var recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<BLMCrystalLv80>());
			recipe.AddIngredient(ItemID.FragmentNebula);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
