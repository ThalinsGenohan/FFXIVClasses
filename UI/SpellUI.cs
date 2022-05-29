using System.Collections.Generic;
using BlackMage.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace BlackMage.UI
{
	internal class SpellUI : UIState
	{
		public int ButtonCount => _buttons.Count;

		private const float IconSize = 40f;
		private const float Padding  = 2f;

		private Dictionary<int, UIImageButton> _buttons;
		private UIText                         _cooldownText;
		private UIElement                      _area;

		public override void OnInitialize()
		{
			_area = new UIElement();
			_area.Top.Set(120f, 0f);
			_area.Left.Set(10f, 0f);
			_area.Width.Set((IconSize + Padding) * Spell.Data.Count, 0f);
			_area.Height.Set(80f, 0f);
			Append(_area);

			_cooldownText = new UIText("");
			_cooldownText.Top.Set(50f, 0f);
			_cooldownText.Left.Set(0f, 0f);
			_cooldownText.Width.Set(40f, 0f);
			_cooldownText.Height.Set(40f, 0f);
			_area.Append(_cooldownText);

			RefreshSpells();
		}

		public override void Update(GameTime gameTime)
		{
			var blm = Main.LocalPlayer.GetModPlayer<BlackMagePlayer>();

			_cooldownText.SetText(blm.GlobalCooldownTimer.ToString());

			List<int> spells        = blm.CastableSpells;
			var       activeButtons = 0;
			foreach (KeyValuePair<int, UIImageButton> buttonPair in _buttons)
			{
				if (spells.Contains(buttonPair.Key))
				{
					buttonPair.Value.Activate();
					activeButtons++;
				}
				else
					buttonPair.Value.Deactivate();
			}
			_area.Width.Set((IconSize + Padding) * activeButtons, 0f);

			base.Update(gameTime);
		}

		public void RefreshSpells()
		{
			_buttons = new Dictionary<int, UIImageButton>();
			var i = 0;
			foreach (KeyValuePair<int, Spell.SpellData> spellDataPair in Spell.Data)
			{
				int             spellId   = spellDataPair.Key;
				Spell.SpellData spellData = spellDataPair.Value;

				if (spellData.SpellName == "Scathe")
					continue;

				_buttons.Add(spellId,
				             new UIImageButton(ModContent.GetTexture("BlackMage/UI/Spells/" + spellData.SpellName)));
				_buttons[spellId].OnMouseUp += (evt, element) => CastSpell(spellId);
				_buttons[spellId].Top.Set(0f, 0f);
				_buttons[spellId].Left.Set((IconSize + Padding) * i, 0f);
				_buttons[spellId].Width.Set(IconSize, 0f);
				_buttons[spellId].Height.Set(IconSize, 0f);

				_area.Append(_buttons[spellId]);
				i++;
			}
			_area.Width.Set((IconSize + Padding) * _buttons.Count, 0f);
		}

		private static void CastSpell(int spellId)
		{
			Main.LocalPlayer.GetModPlayer<BlackMagePlayer>().CastSpell(spellId);
		}
	}
}
