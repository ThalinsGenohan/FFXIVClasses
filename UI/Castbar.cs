using System;
using BlackMage.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace BlackMage.UI;

internal class Castbar : UIState
{
	private UIText    _countdownText;
	private UIText    _spellText;
	private UIElement _area;
	private UIImage   _barFrame;
	private UIImage   _spellIcon;

	public override void OnInitialize()
	{
		_area = new UIElement();
		_area.Left.Set(-100f, 0.5f);
		_area.Top.Set(0f, 0.7f);
		_area.Width.Set(242f, 0f);
		_area.Height.Set(40f, 0f);

		_barFrame = new UIImage(ModContent.Request<Texture2D>("BlackMage/UI/MPBarFrame"));
		_barFrame.Left.Set(42f, 0f);
		_barFrame.Width.Set(200f, 0f);
		_barFrame.Height.Set(24f, 0f);
		_barFrame.VAlign = 1f;

		_spellText = new UIText("Spell", 0.8f);
		_spellText.Left.Set(42f, 0f);
		_spellText.Top.Set(0f, 0f);

		_countdownText = new UIText("0/0", 0.8f);
		_countdownText.Top.Set(0f, 0f);
		_countdownText.HAlign = 1f;

		_spellIcon = new UIImage(ModContent.Request<Texture2D>("BlackMage/UI/Spells/Scathe"));
		_spellIcon.Left.Set(0f, 0f);
		_spellIcon.Top.Set(0f, 0f);

		_area.Append(_countdownText);
		_area.Append(_spellText);
		_area.Append(_barFrame);
		_area.Append(_spellIcon);
		Append(_area);
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		if (Main.LocalPlayer.GetModPlayer<BlackMagePlayer>().CurrentSpell == null)
			return;

		base.Draw(spriteBatch);
	}

	public override void Update(GameTime gameTime)
	{
		var blm = Main.LocalPlayer.GetModPlayer<BlackMagePlayer>();

		if (blm.CurrentSpell == null)
			return;

		_countdownText.SetText($"{Math.Round(blm.CastTimer / 60f, 2),5:F2}");
		_spellText.SetText(Spell.Data[blm.CurrentSpell.Value].SpellName);
		_spellIcon.SetImage(
			ModContent.Request<Texture2D>($"BlackMage/UI/Spells/{Spell.Data[blm.CurrentSpell.Value].SpellName}"));
		base.Update(gameTime);
	}

	protected override void DrawChildren(SpriteBatch spriteBatch)
	{
		base.DrawChildren(spriteBatch);

		var blm = Main.LocalPlayer.GetModPlayer<BlackMagePlayer>();

		if (blm.CurrentSpell == null)
			return;

		CalculatedStyle hitbox = _barFrame.GetInnerDimensions();
		hitbox.X      += 6;
		hitbox.Width  -= 12;
		hitbox.Y      += 6;
		hitbox.Height -= 12;

		float quotient = 1f - blm.CastTimer / (float)blm.MaxCastTimer;
		quotient = Utils.Clamp(quotient, 0f, 1f);

		var left  = (int)Math.Round(hitbox.X);
		var right = (int)Math.Round(hitbox.X + hitbox.Width);
		var steps = (int)((right - left) * quotient);
		for (var i = 0; i < steps; i++)
		{
			var size = (int)Math.Round(hitbox.Height / 2f);
			if (i < 2 || i > hitbox.Width - 3)
				size = 4;

			spriteBatch.Draw(TextureAssets.MagicPixel.Value,
			                 new Rectangle(left + i, (int)Math.Round(hitbox.Y + (hitbox.Height / 2f - size)), 1, size),
			                 Color.Lerp(Constants.Colors.MPTopDark,
			                            Constants.Colors.MPTopLight,
			                            i / (float)steps
			                 ));
			spriteBatch.Draw(TextureAssets.MagicPixel.Value,
			                 new Rectangle(left + i, (int)Math.Round(hitbox.Y + hitbox.Height / 2f), 1, size),
			                 Color.Lerp(Constants.Colors.MPBottomDark,
			                            Constants.Colors.MPBottomLight,
			                            i / (float)steps
			                 ));
		}
	}
}
