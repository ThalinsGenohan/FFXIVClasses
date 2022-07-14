using Terraria;
using Terraria.ModLoader;

namespace BlackMage.Buffs
{
	public class EnhancedFlare : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Enhanced Flare");
			Description.SetDefault("Increases potency of Flare");
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.buffTime[buffIndex]                           = 18000;
			var blm = player.GetModPlayer<BlackMagePlayer>();

			if (blm.AstralFire == 0)
			{
				player.ClearBuff(Type);
				return;
			}

			//blm.EnhancedFlare = true;
		}
	}
}
