using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace BlackMage.UI
{
	internal class MPBar : UIState
	{
		private UIText    _text;
		private UIElement _area;
		private UIImage   _barFrame;
		private Color     _gradientA;
		private Color     _gradientB;

		public override void OnInitialize()
		{
			_area = new UIElement();
			_area.Left.Set(-_area.Width.Pixels - 600f, 1f);
			_area.Top.Set(30f, 0f);
			_area.Width.Set(182f, 0f);
			_area.Height.Set(60f, 0f);

			_barFrame = new UIImage(ModContent.GetTexture("BlackMage/UI/MPBarFrame"));
			_barFrame.Left.Set(22f, 0f);
			_barFrame.Top.Set(0f, 0f);
			_barFrame.Width.Set(138f, 0f);
			_barFrame.Height.Set(34f, 0f);

			_text = new UIText("0/0", 0.8f);
			_text.Width.Set(138f, 0f);
			_text.Height.Set(34f, 0f);
			_text.Top.Set(40f, 0f);
			_text.Left.Set(0f, 0f);

			_gradientA = new Color(123, 25, 138);
			_gradientB = new Color(187, 91, 201);

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

			_text.SetText($"MP: {blm.MP} / {BlackMagePlayer.MaxMP}");
			base.Update(gameTime);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);

			var modPlayer = Main.LocalPlayer.GetModPlayer<BlackMagePlayer>();

			float quotient = modPlayer.MP / (float)BlackMagePlayer.MaxMP;
			quotient = Utils.Clamp(quotient, 0f, 1f);

			var hitbox = _barFrame.GetInnerDimensions().ToRectangle();
			hitbox.X      += 12;
			hitbox.Width  -= 24;
			hitbox.Y      += 8;
			hitbox.Height -= 16;

			int left  = hitbox.Left;
			int right = hitbox.Right;
			var steps = (int)((right - left) * quotient);
			for (var i = 0; i < steps; i++)
			{
				float percent = 1f / (right - left);
				spriteBatch.Draw(Main.magicPixel,
				                 new Rectangle(left + i, hitbox.Y, 1, hitbox.Height),
				                 Color.Lerp(_gradientA, _gradientB, percent));
			}
		}
	}
}
