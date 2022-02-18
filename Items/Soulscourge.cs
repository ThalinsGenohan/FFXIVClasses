using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlackMage.Items
{
	public class Soulscourge : ModItem
	{
		//private static readonly Spell _blizzard = new Spell("Blizzard", 180, 800, "BlizzardProj");
		//private static readonly Spell _fire     = new Spell("Fire",     180, 800, "FireProj");
		private                 bool  _fireMode = false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Soulscourge");
			Tooltip.SetDefault("Two-Handed Thaumaturge's Arm");
		}

		public override void SetDefaults()
		{
			item.magic        = true;
			item.width        = 40;
			item.height       = 40;
			item.useTime      = 150;
			item.useAnimation = 150;
			item.useStyle     = ItemUseStyleID.HoldingOut;
			item.noMelee      = true;
			item.knockBack    = 0f;
			item.value        = 10000;
			item.rare         = ItemRarityID.Purple;
			item.UseSound     = SoundID.Item20;
			item.autoReuse    = false;
			item.shootSpeed   = 10f;
			item.mana         = 0;
			item.damage       = 180;

			//UseSpell(_blizzard);
		}

		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				item.useStyle = ItemUseStyleID.HoldingUp;
				item.shoot    = ProjectileID.None;
				_fireMode     = !_fireMode;
			}
			else
			{
				UseSpell(Spells.Scathe);
			}

			return base.CanUseItem(player);
		}

		public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat)
		{
			var modPlayer = player.GetModPlayer<BlackMagePlayer>();
			mult *= _fireMode ? modPlayer.FireDamageMult : modPlayer.IceDamageMult;
		}

		public override bool Shoot(Player player,
		                           ref Vector2 position,
		                           ref float speedX,
		                           ref float speedY,
		                           ref int type,
		                           ref int damage,
		                           ref float knockBack)
		{
			var modPlayer = player.GetModPlayer<BlackMagePlayer>();

			if (player.altFunctionUse == 2)
			{
				return false;
			}

			int mp = 0;
			/*if (_fireMode)
				mp = (int)(_fire.Mana * (modPlayer.UmbralHearts > 0 ? 1f : modPlayer.FireMPMult));
			else
				mp = (int)(_blizzard.Mana * modPlayer.IceMPMult);*/

			if (modPlayer.MP < mp) return false;

			if (_fireMode)
			{
				modPlayer.AddElementalStack(1);
				if (modPlayer.UmbralHearts > 0)
				{
					modPlayer.UmbralHearts--;
				}
			}
			else
			{
				modPlayer.AddElementalStack(-1);
			}

			modPlayer.MP -= mp;

			return base.Shoot(player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
		}

		public override bool AltFunctionUse(Player player) => true;

		private void UseSpell(Spell spell)
		{
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.SetNameOverride($"Soulscourge ({spell.Name})");
			item.shoot = spell.Projectile;
		}
	}
}
