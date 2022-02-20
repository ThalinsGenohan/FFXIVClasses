﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace BlackMage.Items.Accessories.Crystals
{
	internal class BLMCrystalLv60 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Level 60 Soul of the Black Mage");
			Tooltip.SetDefault(
				"Upon the surface of this multi-aspected crystal are carved the myriad deeds of black mages from eras past.\n" +
				"Allows you to cast Fire IV and Blizzard IV.\n" +
				"Umbral Hearts nullify Astral Fire's MP cost increase, and lower the MP cost for Flare by 1/3.\n" +
				"Casting Blizzard IV or Freeze grants 3 Umbral Hearts.");
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
			player.GetModPlayer<BlackMagePlayer>().SoulCrystalLevel = 60;
		}

		public override int ChoosePrefix(UnifiedRandom rand) => 0;

		public override void AddRecipes()
		{
			var recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<BLMCrystalLv50>());
			recipe.AddIngredient(ItemID.SoulofMight);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
