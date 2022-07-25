using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace BlackMage.UI;

internal class TargetUI : UIState
{
	private UIText    _text;
	private UIElement _area;
	private UIImage   _barFrame;

	public override void OnInitialize()
	{
		_area = new UIElement();
		_area.Left.Set(-_area.Width.Pixels - 600f, 1f);
		_area.Top.Set(30f, 0f);
		_area.Width.Set(200f, 0f);
		_area.Height.Set(60f, 0f);

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
		if (!Main.LocalPlayer.HasMinionAttackTargetNPC)
			return;

		base.Draw(spriteBatch);
	}

	public override void Update(GameTime gameTime)
	{
		if (!Main.LocalPlayer.HasMinionAttackTargetNPC)
			return;

		NPC target = Main.npc[Main.LocalPlayer.MinionAttackTargetNPC];

		_text.SetText($"{target.FullName}: {target.life} / {target.lifeMax}");
		base.Update(gameTime);
	}

	protected override void DrawChildren(SpriteBatch spriteBatch)
	{
		base.DrawChildren(spriteBatch);
		
		NPC target    = Main.npc[Main.LocalPlayer.MinionAttackTargetNPC];

		float quotient = target.life / (float)target.lifeMax;
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
			                 Color.Lerp(Constants.Colors.HPTopDark,
			                            Constants.Colors.HPTopLight,
			                            i / (float)steps
			                 ));
			spriteBatch.Draw(TextureAssets.MagicPixel.Value,
			                 new Rectangle(left + i, (int)Math.Round(hitbox.Y + hitbox.Height / 2f), 1, size),
			                 Color.Lerp(Constants.Colors.HPBottomDark,
			                            Constants.Colors.HPBottomLight,
			                            i / (float)steps
			                 ));
		}
	}
}
