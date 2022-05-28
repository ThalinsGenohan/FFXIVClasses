using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace BlackMage.Items.Accessories.Crystals
{
	public abstract class BaseCrystal : ModItem
	{
		public const string BaseDisplayName = "Level {0} Soul of the Black Mage";

		public const string BaseTooltip =
			"'Upon the surface of this multi-aspected crystal are carved the myriad deeds of black mages from eras past.'";

		public const           int Width  = 20;
		public const           int Height = 20;
		public const           int Rarity = ItemRarityID.Purple;
		public static readonly int Value  = Item.sellPrice(gold: 10);

		protected virtual int    Level          => 0;
		protected virtual string UnlocksTooltip => "";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault(string.Format(BaseDisplayName, Level));
			Tooltip.SetDefault(UnlocksTooltip + "\n \n" + BaseTooltip);
		}

		public override void SetDefaults()
		{
			item.width     = Width;
			item.height    = Height;
			item.accessory = true;
			item.value     = Value;
			item.rare      = Rarity;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<BlackMagePlayer>().SoulCrystalLevel = Level;
		}

		public override int ChoosePrefix(UnifiedRandom rand) => 0;
	}

	internal class BLMCrystalLv10 : BaseCrystal
	{
		protected override int Level => 10;

		protected override string UnlocksTooltip =>
			$"Allows you to cast {Constants.Strings.Fire1} and {Constants.Strings.Blizzard1}.\n" +
			$"Unlocks {Constants.Strings.MP} and the [c/{Constants.Colors.BlackMage.Hex3()}:Elemental Gauge].\n" +
			$"{Constants.Strings.MP} is used for Black Mage spells, and regenerates at a rate of 200 {Constants.Strings.MP} every 2.5 seconds, up to a max of 10000.\n" +
			$"{Constants.Strings.AstralFire} grants a 1.4x damage increase to [c/{Constants.Colors.Fire.Hex3()}:Fire] spells, but a 2x {Constants.Strings.MP} cost, and halts {Constants.Strings.MP} regeneration.\n" +
			$"{Constants.Strings.UmbralIce} increases {Constants.Strings.MP} regeneration to 3200 every 2.5 seconds, and lowers the {Constants.Strings.MP} cost of [c/{Constants.Colors.Ice.Hex3()}:Ice] spells to 0.75x.\n" +
			$"While under the effect of {Constants.Strings.AstralFire} or {Constants.Strings.UmbralIce}, casting a spell of the opposite element will consume no {Constants.Strings.MP}.";
	}

	internal class BLMCrystalLv20 : BaseCrystal
	{
		protected override int Level => 20;

		protected override string UnlocksTooltip =>
			$"Allows you to cast {Constants.Strings.Fire2} and {Constants.Strings.Blizzard2}.\n" +
			$"Allows the stacking of a second {Constants.Strings.AstralFire} (1.6x damage) and {Constants.Strings.UmbralIce} (4700 {Constants.Strings.MP}/2.5 sec, 0.5x cost).";

		public override void AddRecipes()
		{
			var recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<BLMCrystalLv10>());
			recipe.AddIngredient(ItemID.Meteorite);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}

	internal class BLMCrystalLv35 : BaseCrystal
	{
		protected override int Level => 35;

		protected override string UnlocksTooltip =>
			$"Allows you to cast {Constants.Strings.Fire3} and {Constants.Strings.Blizzard3}.\n" +
			$"Allows the stacking of a third {Constants.Strings.AstralFire} (1.8x damage) and {Constants.Strings.UmbralIce} (6200 {Constants.Strings.MP}/2.5 sec, 0x cost).";

		public override void AddRecipes()
		{
			var recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<BLMCrystalLv20>());
			recipe.AddIngredient(ItemID.Hellstone);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}

	internal class BLMCrystalLv50 : BaseCrystal
	{
		protected override int Level => 50;

		protected override string UnlocksTooltip =>
			$"Allows you to cast {Constants.Strings.Flare} and {Constants.Strings.Freeze}.\n" +
			$"Casting {Constants.Strings.Fire2} while under {Constants.Strings.AstralFire} grants [c/{Constants.Colors.Fire.Hex3()}:Enhanced Flare], increasing the potency of {Constants.Strings.Flare} until {Constants.Strings.AstralFire} ends.";

		public override void AddRecipes()
		{
			var recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<BLMCrystalLv35>());
			recipe.AddIngredient(ItemID.SoulofNight);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}

	internal class BLMCrystalLv60 : BaseCrystal
	{
		protected override int Level => 60;

		protected override string UnlocksTooltip =>
			$"Allows you to cast {Constants.Strings.Fire4} and {Constants.Strings.Blizzard4}.\n" +
			$"{Constants.Strings.UmbralHearts} nullify {Constants.Strings.AstralFire}'s {Constants.Strings.MP} cost increase, and lower the {Constants.Strings.MP} cost for {Constants.Strings.Flare} by 1/3.\n" +
			$"Casting {Constants.Strings.Blizzard4} or {Constants.Strings.Freeze} grants 3 {Constants.Strings.UmbralHearts}.";

		public override void AddRecipes()
		{
			var recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<BLMCrystalLv50>());
			recipe.AddIngredient(ItemID.SoulofMight);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}

	internal class BLMCrystalLv70 : BaseCrystal
	{
		protected override int Level => 70;

		protected override string UnlocksTooltip =>
			$"Allows you to cast {Constants.Strings.Foul}.\n" +
			$"Grants {Constants.Strings.Polyglot} every 30 seconds {Constants.Strings.AstralFire} or {Constants.Strings.UmbralIce} is active, allowing for {Constants.Strings.Foul} to be cast.";

		public override void AddRecipes()
		{
			var recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<BLMCrystalLv60>());
			recipe.AddIngredient(ItemID.Ectoplasm);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}

	internal class BLMCrystalLv75 : BaseCrystal
	{
		protected override int Level => 75;

		protected override string UnlocksTooltip =>
			$"Allows you to cast {Constants.Strings.Despair}.";

		public override void AddRecipes()
		{
			var recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<BLMCrystalLv70>());
			recipe.AddIngredient(ItemID.BeetleHusk);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}

	internal class BLMCrystalLv80 : BaseCrystal
	{
		protected override int Level => 80;

		protected override string UnlocksTooltip =>
			$"Allows you to cast {Constants.Strings.Xenoglossy}.\n" +
			$"Allows the stacking of a second {Constants.Strings.Polyglot}.";

		public override void AddRecipes()
		{
			var recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<BLMCrystalLv75>());
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}

	internal class BLMCrystalLv90 : BaseCrystal
	{
		protected override int Level => 90;

		protected override string UnlocksTooltip =>
			$"Allows you to cast {Constants.Strings.Paradox}.\n" +
			$"{Constants.Strings.Paradox} replaces {Constants.Strings.Fire1} and {Constants.Strings.Blizzard1} upon switching from {Constants.Strings.AstralFire} 3 to {Constants.Strings.UmbralIce}, or from {Constants.Strings.UmbralIce} 3 with 3 {Constants.Strings.UmbralHearts} to {Constants.Strings.AstralFire}.";

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
