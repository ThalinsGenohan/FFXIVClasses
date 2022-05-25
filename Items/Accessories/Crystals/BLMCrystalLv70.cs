using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace BlackMage.Items.Accessories.Crystals
{
	internal class BLMCrystalLv70 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Level 70 Soul of the Black Mage");
			Tooltip.SetDefault(
				"Upon the surface of this multi-aspected crystal are carved the myriad deeds of black mages from eras past.\n" +
				"Allows you to cast Foul.\n" +
				"Grants Polyglot every 30 seconds Astral Fire or Umbral Ice is active, allowing for Foul to be cast.");
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
			player.GetModPlayer<BlackMagePlayer>().SoulCrystalLevel = 70;
		}

		public override int ChoosePrefix(UnifiedRandom rand) => 0;

		public override void AddRecipes() => CreateRecipe()
		                                     .AddIngredient(ModContent.ItemType<BLMCrystalLv60>())
		                                     .AddIngredient(ItemID.Ectoplasm)
		                                     .Register();
	}
}
