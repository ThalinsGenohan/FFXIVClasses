using BlackMage.Projectiles;
using Terraria;
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
			Item.DamageType   = DamageClass.Magic;
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
			Item.shoot        = ModContent.ProjectileType<ScatheProj>();
		}

		public override bool CanShoot(Player player)
		{
			return !player.HasMinionAttackTargetNPC && base.CanShoot(player);
		}
	}
}
