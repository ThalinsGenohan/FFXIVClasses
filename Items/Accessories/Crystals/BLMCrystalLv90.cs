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
			Item.width     = 20;
			Item.height    = 20;
			Item.accessory = true;
			Item.value     = Item.sellPrice(gold: 10);
			Item.rare      = ItemRarityID.Purple;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<BlackMagePlayer>().SoulCrystalLevel = 90;
		}

		public override int ChoosePrefix(UnifiedRandom rand) => 0;

		public override void AddRecipes() => CreateRecipe()
		                                     .AddIngredient(ModContent.ItemType<BLMCrystalLv80>())
		                                     .AddIngredient(ItemID.FragmentNebula)
		                                     .Register();
	}
}
