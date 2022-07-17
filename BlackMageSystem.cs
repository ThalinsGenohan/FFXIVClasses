using System.Collections.Generic;
using BlackMage.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace BlackMage;

public class BlackMageSystem : ModSystem
{
	internal MPBar                MPBar                { get; private set; }
	internal Castbar              Castbar              { get; private set; }
	internal SimpleElementalGauge SimpleElementalGauge { get; private set; }
	internal FancyElementalGauge  FancyElementalGauge  { get; private set; }
	internal SpellUI              SpellUI              { get; private set; }

	private UserInterface _mpBarUI;
	private UserInterface _castbarUI;
	private UserInterface _elementalGaugeUI;
	private UserInterface _spellUI;

	public override void Load()
	{
		if (!Main.dedServ)
		{
			MPBar    = new MPBar();
			_mpBarUI = new UserInterface();
			_mpBarUI.SetState(MPBar);

			Castbar    = new Castbar();
			_castbarUI = new UserInterface();
			_castbarUI.SetState(Castbar);

			SimpleElementalGauge = new SimpleElementalGauge();
			FancyElementalGauge  = new FancyElementalGauge();
			_elementalGaugeUI    = new UserInterface();
			_elementalGaugeUI.SetState(SimpleElementalGauge);

			SpellUI  = new SpellUI();
			_spellUI = new UserInterface();
			_spellUI.SetState(SpellUI);
		}

		base.Load();
	}

	public override void UpdateUI(GameTime gameTime)
	{
		if (Main.dedServ)
			return;

		Player player = Main.LocalPlayer;
		var    blm    = player.GetModPlayer<BlackMagePlayer>();

		if (blm.SoulCrystalLevel == 0)
		{
			if (_mpBarUI.CurrentState != null)
				_mpBarUI.SetState(null);
			if (_castbarUI.CurrentState != null)
				_castbarUI.SetState(null);
			if (_elementalGaugeUI.CurrentState != null)
				_elementalGaugeUI.SetState(null);
			if (_spellUI.CurrentState != null)
				_spellUI.SetState(null);
		}
		else
		{
			if (_mpBarUI.CurrentState == null)
				_mpBarUI.SetState(MPBar);
			if (_castbarUI.CurrentState == null)
				_castbarUI.SetState(Castbar);
			if (_elementalGaugeUI.CurrentState == null)
				_elementalGaugeUI.SetState(SimpleElementalGauge);
			if (_spellUI.CurrentState == null)
				_spellUI.SetState(SpellUI);

			if (SpellUI.ButtonCount == 0)
				SpellUI.RefreshSpells();
		}

		_mpBarUI.Update(gameTime);
		_castbarUI.Update(gameTime);
		_elementalGaugeUI.Update(gameTime);
		_spellUI.Update(gameTime);
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
				              "BlackMage: Castbar",
				              delegate
				              {
					              _castbarUI.Draw(Main.spriteBatch, new GameTime());
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

		int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
		if (mouseTextIndex != -1)
			layers.Insert(mouseTextIndex,
			              new LegacyGameInterfaceLayer(
				              "BlackMage: Spell UI",
				              delegate
				              {
					              _spellUI.Draw(Main.spriteBatch, new GameTime());
					              return true;
				              },
				              InterfaceScaleType.UI
			              )
			);
	}
}