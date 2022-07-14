using Terraria;
using Terraria.ModLoader;

namespace BlackMage.Buffs
{
	public class Firestarter : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Firestarter");
			Description.SetDefault("Next [Firaga] costs no MP and has no cast time");
			Main.buffNoTimeDisplay[Type] = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			var blm = player.GetModPlayer<BlackMagePlayer>();
			blm.Firestarter = true;
		}
	}
}
