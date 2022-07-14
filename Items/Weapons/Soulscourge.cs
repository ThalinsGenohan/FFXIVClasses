using BlackMage.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlackMage.Items.Weapons
{
	public class Soulscourge : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Soulscourge");
			Tooltip.SetDefault("Two-Handed Thaumaturge's Arm");
			ItemID.Sets.LockOnIgnoresCollision[Item.type] = false;
		}

		public override void SetDefaults()
		{
			Item.DamageType        = DamageClass.Magic;
			Item.width        = 40;
			Item.height       = 40;
			Item.useTime      = 20;
			Item.useAnimation = 20;
			Item.useStyle     = ItemUseStyleID.Swing;
			Item.knockBack    = 0f;
			Item.value        = 10000;
			Item.rare         = ItemRarityID.Purple;
			Item.UseSound     = SoundID.Item20;
			Item.autoReuse    = false;
			Item.shootSpeed   = 10f;
			Item.mana         = 1;
			Item.damage       = 180;
			Item.noMelee      = true;
			Item.shoot        = ModContent.ProjectileType<Scathe>();
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			return !player.HasMinionAttackTargetNPC &&
			       base.Shoot(player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
		}
	}
}
