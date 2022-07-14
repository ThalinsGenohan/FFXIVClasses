using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace BlackMage.UI
{
	internal class SimpleElementalGauge : UIState
	{
		private static readonly Texture2D BlankTexture = ModContent.GetTexture("BlackMage/UI/Blank");

		private static readonly Texture2D EmptyDiamondTexture =
			ModContent.GetTexture("BlackMage/UI/ElementalGauge/Simple/EmptyDiamond");

		private static readonly Texture2D FireDiamondTexture =
			ModContent.GetTexture("BlackMage/UI/ElementalGauge/Simple/FireDiamond");

		private static readonly Texture2D IceDiamondTexture =
			ModContent.GetTexture("BlackMage/UI/ElementalGauge/Simple/IceDiamond");

		private static readonly Texture2D HeartDiamondTexture =
			ModContent.GetTexture("BlackMage/UI/ElementalGauge/Simple/HeartDiamond");

		private static readonly Texture2D PolyglotDiamondTexture =
			ModContent.GetTexture("BlackMage/UI/ElementalGauge/Simple/PolyglotDiamond");

		private static readonly Texture2D PolyglotBarFrameTexture =
			ModContent.GetTexture("BlackMage/UI/ElementalGauge/Simple/PolyglotBarFrame");

		private static readonly Texture2D ParadoxDiamondTexture =
			ModContent.GetTexture("BlackMage/UI/ElementalGauge/Simple/ParadoxDiamond");

		private const float DiamondSize = 28f;
		private const float BarWidth    = 200f;

		private static readonly Vector2 ElementStackPos = new Vector2(DiamondSize, DiamondSize + PolyglotBarFrameTexture.Height);

		private static readonly Vector2 HeartPos =
			ElementStackPos + new Vector2(DiamondSize * 3f + 2f, 0f);

		private static readonly Vector2 PolyglotBarPos = new Vector2(0f, DiamondSize);
		private static readonly Vector2 PolyglotPos = PolyglotBarPos + new Vector2(BarWidth, -2f);
		private static readonly Vector2 ParadoxPos = new Vector2(HeartPos.X - (float)Math.Floor(DiamondSize / 2f), 0f);
		private static readonly Vector2 ElementCountdownPos = new Vector2(0f, DiamondSize + PolyglotBarFrameTexture.Height + 2f);

		private UIElement _area;
		private UIImage[] _elementStacks;
		private UIImage[] _hearts;
		private UIImage[] _polyglots;
		private UIImage   _paradox;
		private UIImage   _polyglotBarFrame;
		private UIText    _elementCountdown;

		public override void OnInitialize()
		{
			_area = new UIElement();
			_area.Left.Set(-_area.Width.Pixels - 600f, 1f);
			_area.Top.Set(90f, 0f);
			_area.Width.Set(BarWidth + DiamondSize * 2f, 0f);
			_area.Height.Set(DiamondSize * 3f, 0f);

			_elementStacks = new UIImage[BlackMagePlayer.MaxElementStacks];
			for (var i = 0; i < _elementStacks.Length; i++)
			{
				_elementStacks[i] = new UIImage(EmptyDiamondTexture);
				_elementStacks[i].Left.Set(ElementStackPos.X + DiamondSize * i, 0f);
				_elementStacks[i].Top.Set(ElementStackPos.Y, 0f);
				_elementStacks[i].Width.Set(DiamondSize, 0f);
				_elementStacks[i].Height.Set(DiamondSize, 0f);
			}

			_hearts = new UIImage[BlackMagePlayer.MaxUmbralHearts];
			for (var i = 0; i < _hearts.Length; i++)
			{
				_hearts[i] = new UIImage(BlankTexture);
				_hearts[i].Left.Set(HeartPos.X + DiamondSize * i, 0f);
				_hearts[i].Top.Set(HeartPos.Y, 0f);
				_hearts[i].Width.Set(DiamondSize, 0f);
				_hearts[i].Height.Set(DiamondSize, 0f);
			}

			_polyglots = new UIImage[BlackMagePlayer.MaxPolyglots];
			for (var i = 0; i < _polyglots.Length; i++)
			{
				_polyglots[i] = new UIImage(EmptyDiamondTexture);
				_polyglots[i].Left.Set(PolyglotPos.X + DiamondSize * i, 0f);
				_polyglots[i].Top.Set(PolyglotPos.Y, 0f);
				_polyglots[i].Width.Set(DiamondSize, 0f);
				_polyglots[i].Height.Set(DiamondSize, 0f);
			}

			_paradox = new UIImage(EmptyDiamondTexture);
			_paradox.Left.Set(ParadoxPos.X, 0f);
			_paradox.Top.Set(ParadoxPos.Y, 0f);
			_paradox.Width.Set(DiamondSize, 0f);
			_paradox.Height.Set(DiamondSize, 0f);

			_polyglotBarFrame = new UIImage(PolyglotBarFrameTexture);
			_polyglotBarFrame.Left.Set(PolyglotBarPos.X, 0f);
			_polyglotBarFrame.Top.Set(PolyglotBarPos.Y, 0f);
			_polyglotBarFrame.Width.Set(BarWidth, 0f);
			_polyglotBarFrame.Height.Set(PolyglotBarFrameTexture.Height, 0f);

			_elementCountdown = new UIText("0");
			_elementCountdown.Left.Set(ElementCountdownPos.X, 0f);
			_elementCountdown.Top.Set(ElementCountdownPos.Y, 0f);
			_elementCountdown.Width.Set(DiamondSize, 0f);
			_elementCountdown.Height.Set(DiamondSize, 0f);

			foreach (UIImage elementStack in _elementStacks)
				_area.Append(elementStack);
			foreach (UIImage heart in _hearts)
				_area.Append(heart);
			foreach (UIImage polyglot in _polyglots)
				_area.Append(polyglot);
			_area.Append(_paradox);
			_area.Append(_polyglotBarFrame);
			_area.Append(_elementCountdown);
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

			_elementCountdown.SetText(blm.ElementalCharge != 0 ? (blm.ElementalChargeTimer / 60).ToString() : "");

			base.Update(gameTime);
		}

		private void DrawSimple(SpriteBatch spriteBatch)
		{
			var blm = Main.LocalPlayer.GetModPlayer<BlackMagePlayer>();

			if (blm.AstralFire > 0)
				for (var i = 0; i < blm.AstralFire; i++)
					_elementStacks[i].SetImage(FireDiamondTexture);
			else if (blm.UmbralIce > 0)
				for (var i = 0; i < blm.UmbralIce; i++)
					_elementStacks[i].SetImage(IceDiamondTexture);

			for (int i = Math.Max(blm.AstralFire, blm.UmbralIce); i < blm.AllowedElementStacks; i++)
				_elementStacks[i].SetImage(EmptyDiamondTexture);
			for (int i = blm.AllowedElementStacks; i < BlackMagePlayer.MaxElementStacks; i++)
				_elementStacks[i].SetImage(BlankTexture);

			if (blm.CanHaveUmbralHearts)
			{
				for (var i = 0; i < blm.UmbralHearts; i++)
					_hearts[i].SetImage(HeartDiamondTexture);
				for (int i = blm.UmbralHearts; i < BlackMagePlayer.MaxUmbralHearts; i++)
					_hearts[i].SetImage(BlankTexture);
			}
			else
				for (var i = 0; i < BlackMagePlayer.MaxUmbralHearts; i++)
					_hearts[i].SetImage(BlankTexture);

			if (blm.AllowedPolyglots > 0)
			{
				for (var i = 0; i < blm.Polyglots; i++)
					_polyglots[i].SetImage(PolyglotDiamondTexture);
				for (int i = blm.Polyglots; i < blm.AllowedPolyglots; i++)
					_polyglots[i].SetImage(EmptyDiamondTexture);
				for (int i = blm.AllowedPolyglots; i < BlackMagePlayer.MaxPolyglots; i++)
					_polyglots[i].SetImage(BlankTexture);

				_polyglotBarFrame.SetImage(PolyglotBarFrameTexture);

				float quotient = (BlackMagePlayer.PolyglotMaxTime - blm.PolyglotTimer) /
								 (float)BlackMagePlayer.PolyglotMaxTime;
				quotient = Utils.Clamp(quotient, 0f, 1f);

				var hitbox = _polyglotBarFrame.GetInnerDimensions().ToRectangle();
				hitbox.X      += 6;
				hitbox.Width  -= 12;
				hitbox.Y      += 6;
				hitbox.Height -= 12;

				var steps = (int)((hitbox.Right - hitbox.Left) * quotient);

				for (var i = 0; i < steps; i++)
				{
					int size = hitbox.Height / 2;
					if (i < 6 || i > hitbox.Width - 7)
					{
						size = 6;
					}

					if (i < 2 || i > hitbox.Width - 3)
					{
						size = 2;
					}

					spriteBatch.Draw(TextureAssets.MagicPixel.Value,
					                 new Rectangle(hitbox.Left + i, hitbox.Y + (hitbox.Height / 2 - size), 1, size),
					                 Color.Lerp(Constants.Colors.PolyglotTopDark,
					                            Constants.Colors.PolyglotTopLight,
					                            i / (float)steps
					                 ));
					spriteBatch.Draw(TextureAssets.MagicPixel.Value,
					                 new Rectangle(hitbox.Left + i, hitbox.Y + hitbox.Height / 2, 1, size),
					                 Color.Lerp(Constants.Colors.PolyglotBottomDark,
					                            Constants.Colors.PolyglotBottomLight,
					                            i / (float)steps
					                 ));
				}
			}
			else
			{
				for (var i = 0; i < BlackMagePlayer.MaxPolyglots; i++)
					_polyglots[i].SetImage(BlankTexture);
				_polyglotBarFrame.SetImage(BlankTexture);
			}

			if (blm.CanUseParadox)
				_paradox.SetImage(blm.ParadoxReady ? ParadoxDiamondTexture : EmptyDiamondTexture);
			else
				_paradox.SetImage(BlankTexture);
		}

		protected override void DrawChildren(SpriteBatch spriteBatch)
		{
			base.DrawChildren(spriteBatch);
			DrawSimple(spriteBatch);
		}
	}
}
