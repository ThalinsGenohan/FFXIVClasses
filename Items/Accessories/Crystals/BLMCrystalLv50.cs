using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace BlackMage.Items.Accessories.Crystals
{
	internal class BLMCrystalLv50 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Level 50 Soul of the Black Mage");
			Tooltip.SetDefault(
				"Upon the surface of this multi-aspected crystal are carved the myriad deeds of black mages from eras past.\n" +
				"Allows you to cast Flare and Freeze.\n" +
				"Casting Fire II while under Astral Fire grants Enhanced Flare, increasing the potency of Flare until Astral Fire ends.");
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
			player.GetModPlayer<BlackMagePlayer>().SoulCrystalLevel = 50;
		}

		public override int ChoosePrefix(UnifiedRandom rand) => 0;

		public override void AddRecipes() => CreateRecipe()
		                                     .AddIngredient(ModContent.ItemType<BLMCrystalLv35>())
		                                     .AddIngredient(ItemID.SoulofNight)
		                                     .Register();
	}
}
