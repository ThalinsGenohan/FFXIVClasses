using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace BlackMage.UI
{
	internal class SpellUI : UIState
	{
		private const float IconSize = 40f;
		private const float Padding  = 2f;

		private Dictionary<Spell, UIImageButton> _buttons;
		private UIText                           _cooldownText;
		private UIElement                        _area;

		public override void OnInitialize()
		{
			_area = new UIElement();
			_area.Top.Set(120f, 0f);
			_area.Left.Set(10f, 0f);
			_area.Width.Set((IconSize + Padding) * typeof(Spells).GetFields().Length, 0f);
			_area.Height.Set(80f, 0f);
			Append(_area);

			_cooldownText = new UIText("");
			_cooldownText.Top.Set(50f, 0f);
			_cooldownText.Left.Set(0f, 0f);
			_cooldownText.Width.Set(40f, 0f);
			_cooldownText.Height.Set(40f, 0f);
			_area.Append(_cooldownText);

			_buttons = new Dictionary<Spell, UIImageButton>();
			var i = 0;
			foreach (FieldInfo fieldInfo in typeof(Spells).GetFields())
			{
				var spell = (Spell)fieldInfo.GetValue(null);
				if (spell != null)
				{
					_buttons.Add(spell,
					             new UIImageButton(ModContent.Request<Texture2D>("BlackMage/UI/Spells/" + spell.Name)));
					_buttons[spell].OnMouseUp += (_, _) => CastSpell(spell);
					_buttons[spell].Top.Set(0f, 0f);
					_buttons[spell].Left.Set((IconSize + Padding) * i, 0f);
					_buttons[spell].Width.Set(IconSize, 0f);
					_buttons[spell].Height.Set(IconSize, 0f);

					_area.Append(_buttons[spell]);
				}

				i++;
			}
		}

		public override void Update(GameTime gameTime)
		{
			var blm = Main.LocalPlayer.GetModPlayer<BlackMagePlayer>();

			_cooldownText.SetText(blm.GlobalCooldownTimer.ToString());

			base.Update(gameTime);
		}

		private static void CastSpell(Spell spell)
		{
			Main.LocalPlayer.GetModPlayer<BlackMagePlayer>().CastSpell(spell);
		}
	}
}
