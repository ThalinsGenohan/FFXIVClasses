using System.Collections.Generic;
using BlackMage.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace BlackMage
{
	public class BlackMage : Mod
	{
		internal MPBar          MPBar          { get; private set; }
		internal ElementalGauge ElementalGauge { get; private set; }
		private  UserInterface  _mpBarUI;
		private  UserInterface  _elementalGaugeUI;

		public override void Load()
		{
			if (!Main.dedServ)
			{
				MPBar    = new MPBar();
				_mpBarUI = new UserInterface();
				_mpBarUI.SetState(MPBar);

				ElementalGauge    = new ElementalGauge();
				_elementalGaugeUI = new UserInterface();
				_elementalGaugeUI.SetState(ElementalGauge);
			}
			base.Load();
		}

		public override void UpdateUI(GameTime gameTime)
		{
			_mpBarUI?.Update(gameTime);
			_elementalGaugeUI?.Update(gameTime);
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
			if (resourceBarIndex != -1)
			{
				layers.Insert(resourceBarIndex,
				              new LegacyGameInterfaceLayer(
					              "BlackMage: MP Bar",
					              delegate
					              {
						              _mpBarUI.Draw(Main.spriteBatch, new GameTime());
						              return true;
					              },
					              InterfaceScaleType.UI)
				);
				layers.Insert(resourceBarIndex,
				              new LegacyGameInterfaceLayer(
					              "BlackMage: Elemental Gauge",
					              delegate
					              {
						              _elementalGaugeUI.Draw(Main.spriteBatch, new GameTime());
						              return true;
					              },
					              InterfaceScaleType.UI
				              )
				);
			}
		}
	}
}