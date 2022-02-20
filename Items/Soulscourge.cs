using Terraria.ID;
using Terraria.ModLoader;

namespace BlackMage.Items
{
	public class Soulscourge : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Soulscourge");
			Tooltip.SetDefault("Two-Handed Thaumaturge's Arm");
			ItemID.Sets.LockOnIgnoresCollision[item.type] = true;
		}

		public override void SetDefaults()
		{
			item.magic        = true;
			item.width        = 40;
			item.height       = 40;
			item.useTime      = 20;
			item.useAnimation = 20;
			item.useStyle     = ItemUseStyleID.HoldingOut;
			item.knockBack    = 0f;
			item.value        = 10000;
			item.rare         = ItemRarityID.Purple;
			item.UseSound     = SoundID.Item20;
			item.autoReuse    = false;
			item.shootSpeed   = 10f;
			item.mana         = 0;
			item.damage       = 180;

			item.noMelee = true;
			UseSpell(Spells.Scathe);
		}

		/*public override bool Shoot(Player player,
		                           ref Vector2 position,
		                           ref float speedX,
		                           ref float speedY,
		                           ref int type,
		                           ref int damage,
		                           ref float knockBack)
		{
			var modPlayer = player.GetModPlayer<BlackMagePlayer>();

			return base.Shoot(player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
		}*/

		private void UseSpell(Spell spell)
		{
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.SetNameOverride($"Soulscourge ({spell.Name})");
			item.shoot = spell.Projectile;
		}
	}
}
