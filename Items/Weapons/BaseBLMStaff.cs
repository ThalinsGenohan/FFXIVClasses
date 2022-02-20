using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BlackMage.Items.Weapons
{
	internal abstract class BaseBLMStaff : ModItem
	{
		private NPC   _target;
		private Spell _spell;

		public override bool Shoot(Player player,
		                           ref Vector2 position,
		                           ref float speedX,
		                           ref float speedY,
		                           ref int type,
		                           ref int damage,
		                           ref float knockBack)
		{
			var blm = player.GetModPlayer<BlackMagePlayer>();

			

			return base.Shoot(player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
		}
	}
}
