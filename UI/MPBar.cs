using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace BlackMage.UI;

internal class MPBar : UIState
{
	private UIText    _text;
	private UIElement _area;
	private UIImage   _barFrame;

	public override void OnInitialize()
	{
		_area = new UIElement();
		_area.Left.Set(0f, 0f);
		_area.Top.Set(0f, 0.75f);
		_area.Width.Set(200f, 0f);
		_area.Height.Set(60f, 0f);
		_area.HAlign = 0.5f;

		_barFrame = new UIImage(ModContent.Request<Texture2D>("BlackMage/UI/MPBarFrame"));
		_barFrame.Left.Set(0f, 0f);
		_barFrame.Top.Set(0f, 0f);
		_barFrame.Width.Set(200f, 0f);
		_barFrame.Height.Set(24f, 0f);

		_text = new UIText("0/0", 0.8f);
		_text.Width.Set(200f, 0f);
		_text.Height.Set(34f, 0f);
		_text.Top.Set(30f, 0f);
		_text.Left.Set(0f, 0f);

		_area.Append(_text);
		_area.Append(_barFrame);
		Append(_area);
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		if (Main.LocalPlayer.GetModPlayer<BlackMagePlayer>().SoulCrystalLevel == 0)
			return;

		base.Draw(spriteBatch);
	}

	public override void Update(GameTime gameTime)
	{
		var blm = Main.LocalPlayer.GetModPlayer<BlackMagePlayer>();

		if (blm.SoulCrystalLevel == 0)
			return;

		_text.SetText($"MP: {blm.MP,5}");
		base.Update(gameTime);
	}

	protected override void DrawChildren(SpriteBatch spriteBatch)
	{
		base.DrawChildren(spriteBatch);

		var modPlayer = Main.LocalPlayer.GetModPlayer<BlackMagePlayer>();

		float quotient = modPlayer.MP / (float)BlackMagePlayer.MaxMP;
		quotient = Utils.Clamp(quotient, 0f, 1f);

		CalculatedStyle hitbox = _barFrame.GetInnerDimensions();
		hitbox.X      += 6;
		hitbox.Width  -= 12;
		hitbox.Y      += 6;
		hitbox.Height -= 12;

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
