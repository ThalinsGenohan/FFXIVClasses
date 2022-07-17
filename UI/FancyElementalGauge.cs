using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace BlackMage.UI;

internal class FancyElementalGauge : UIState
{
	private static readonly Asset<Texture2D> BlankTexture = ModContent.Request<Texture2D>("BlackMage/UI/Blank");

	private const string UIPath = "BlackMage/UI/ElementalGauge/Fancy/";

	//private static readonly Texture2D Example = ModContent.Request<Texture2D>(UIPath + "Example");
	private static readonly Asset<Texture2D> Frame         = ModContent.Request<Texture2D>(UIPath + "Frame");
	private static readonly Asset<Texture2D> ClockInactive = ModContent.Request<Texture2D>(UIPath + "ClockInactive");
	private static readonly Asset<Texture2D> ClockActive   = ModContent.Request<Texture2D>(UIPath + "ClockActive");
	private static readonly Asset<Texture2D> ClockCenter   = ModContent.Request<Texture2D>(UIPath + "ClockCenter");
	private static readonly Asset<Texture2D> ClockHand     = ModContent.Request<Texture2D>(UIPath + "ClockHand");

	private static readonly Asset<Texture2D> PearlInactive = ModContent.Request<Texture2D>(UIPath + "PearlInactive");
	private static readonly Asset<Texture2D> PearlFire     = ModContent.Request<Texture2D>(UIPath + "PearlFire");
	private static readonly Asset<Texture2D> PearlIce      = ModContent.Request<Texture2D>(UIPath + "PearlIce");

	private static readonly Asset<Texture2D>[] ShardsFire =
	{
		ModContent.Request<Texture2D>(UIPath + "ShardsFire1"),
		ModContent.Request<Texture2D>(UIPath + "ShardsFire2"),
		ModContent.Request<Texture2D>(UIPath + "ShardsFire3"),
	};

	private static readonly Asset<Texture2D>[] ShardsIce =
	{
		ModContent.Request<Texture2D>(UIPath + "ShardsIce1"),
		ModContent.Request<Texture2D>(UIPath + "ShardsIce2"),
		ModContent.Request<Texture2D>(UIPath + "ShardsIce3"),
	};

	private UIElement _area;

	//private UIImage _example;
	private UIText  _countdown;
	private UIImage _frame;
	private UIImage _clock;
	private UIImage _pearl;

	private UIImage _clockCenter;

	//private UIImage _clockHand;
	private UIImage _shards;

	public override void OnInitialize()
	{
		_area = new UIElement();
		_area.Left.Set(-_area.Width.Pixels - 600f, 1f);
		_area.Top.Set(90f, 0f);
		_area.Width.Set(Frame.Width(), 0f);
		_area.Height.Set(Frame.Height(), 0f);

		/*_example = new UIImage(Example);
		_example.Left.Set(0f, 0f);
		_example.Top.Set(0f, 0f);
		_example.Width.Set(148f, 0f);
		_example.Height.Set(128f, 0f);*/

		_countdown = new UIText("", 0.8f);
		_countdown.Left.Set(105f, 0f);
		_countdown.Top.Set(38f, 0f);

		_frame = new UIImage(Frame);
		_frame.Left.Set(0f, 0f);
		_frame.Top.Set(0f, 0f);
		_frame.Width.Set(Frame.Width(), 0f);
		_frame.Height.Set(Frame.Height(), 0f);

		_clock = new UIImage(ClockInactive);
		_clock.Left.Set(0f, 0f);
		_clock.Top.Set(0f, 0f);
		_clock.Width.Set(Frame.Width(), 0f);
		_clock.Height.Set(Frame.Height(), 0f);

		_pearl = new UIImage(PearlInactive);
		_pearl.Left.Set(0f, 0f);
		_pearl.Top.Set(0f, 0f);
		_pearl.Width.Set(Frame.Width(), 0f);
		_pearl.Height.Set(Frame.Height(), 0f);

		_clockCenter = new UIImage(BlankTexture);
		_clockCenter.Left.Set(0f, 0f);
		_clockCenter.Top.Set(0f, 0f);
		_clockCenter.Width.Set(Frame.Width(), 0f);
		_clockCenter.Height.Set(Frame.Height(), 0f);

		/*_clockHand = new UIImage(BlankTexture);
		_clockHand.Left.Set(0f, 0f);
		_clockHand.Top.Set(0f, 0f);
		_clockHand.Width.Set(Frame.Width, 0f);
		_clockHand.Height.Set(Frame.Height, 0f);*/

		_shards = new UIImage(BlankTexture);
		_shards.Left.Set(0f, 0f);
		_shards.Top.Set(0f, 0f);
		_shards.Width.Set(Frame.Width(), 0f);
		_shards.Height.Set(Frame.Height(), 0f);

		//_area.Append(_example);
		_area.Append(_clock);
		_area.Append(_frame);
		_area.Append(_countdown);
		_area.Append(_pearl);
		_area.Append(_shards);
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

		if (blm.AstralFire > 0)
		{
			_pearl.SetImage(PearlFire);
			_shards.SetImage(ShardsFire[blm.AstralFire - 1]);
		}
		else if (blm.UmbralIce > 0)
		{
			_pearl.SetImage(PearlIce);
			_shards.SetImage(ShardsIce[blm.UmbralIce - 1]);
		}
		else
		{
			_pearl.SetImage(PearlInactive);
			_shards.SetImage(BlankTexture);
		}

		if (blm.SoulCrystalLevel >= 56 && blm.ElementalCharge != 0)
			_clock.SetImage(ClockActive);
		else
			_clock.SetImage(ClockInactive);

		_countdown.SetText(blm.ElementalCharge != 0 ? $"{(int)Math.Floor(blm.ElementalChargeTimer / 60f),2}" : "");

		base.Update(gameTime);
	}

	protected override void DrawChildren(SpriteBatch spriteBatch)
	{
		base.DrawChildren(spriteBatch);
		DrawFancy(spriteBatch);
	}

	private void DrawFancy(SpriteBatch spriteBatch)
	{
		var blm = Main.LocalPlayer.GetModPlayer<BlackMagePlayer>();

		var angle = (float)(MathHelper.PiOver2 - MathHelper.PiOver2 *
		                    (Math.Floor(blm.PolyglotTimer / 60f) / BlackMagePlayer.PolyglotChargeSeconds));

		if (blm.AllowedPolyglots > 0 && blm.ElementalCharge != 0)
		{
			spriteBatch.Draw(ClockHand.Value,
			                 _area.GetDimensions().Center(),
			                 null,
			                 Color.White,
			                 angle,
			                 ClockHand.Value.Bounds.Center(),
			                 Vector2.One,
			                 SpriteEffects.None,
			                 0f);
			spriteBatch.Draw(ClockCenter.Value, _area.GetDimensions().Position(), Color.White);
		}
	}
}
