using System;
using System.Collections.Generic;
using System.Globalization;
using BlackMage.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace BlackMage.UI;

internal class SpellUI : UIState
{
	private const float IconSize = 40f;
	private const float Padding  = 2f;
	public        int   ButtonCount => _buttons.Count;

	private readonly Dictionary<int, SpellButton> _buttons = new();
	private          UIElement                    _area;
	private          int                          _lastCrystalLevel = 0;

	private static void CastSpell(int spellId)
	{
		Main.LocalPlayer.GetModPlayer<BlackMagePlayer>().BeginSpellCast(spellId);
	}

	public override void OnInitialize()
	{
		_area = new UIElement();
		_area.Top.Set(0f, 0.85f);
		_area.Left.Set(0f, 0f);
		_area.Width.Set((IconSize + Padding) * Spell.Data.Count, 0f);
		_area.Height.Set(80f, 0f);
		_area.HAlign = 0.5f;
		Append(_area);

		RefreshSpells();
	}

	public override void Update(GameTime gameTime)
	{
		var blm = Main.LocalPlayer.GetModPlayer<BlackMagePlayer>();

		if (_lastCrystalLevel != blm.SoulCrystalLevel)
		{
			_lastCrystalLevel = blm.SoulCrystalLevel;
			RefreshSpells();
		}

		foreach ((int spellId, SpellButton button) in _buttons)
		{
			Spell.SpellData spellData = Spell.Data[spellId];

			button.Activate();

			int mpCost = blm.GetSpellCost(spellId);
			button.ManaText.SetText(spellData.MPCost == -1 ? "All" : mpCost > 0 ? mpCost.ToString() : "",
			                        0.8f,
			                        false);

			button.ManaText.TextColor = mpCost > blm.MP ? Color.Red : Color.White;

			button.Update(gameTime);
		}

		_area.Width.Set((IconSize + Padding) * _buttons.Count, 0f);

		base.Update(gameTime);
	}

	public void RefreshSpells()
	{
		var i = 0;
		foreach ((int spellId, Spell.SpellData spellData) in Spell.Data)
		{
			if (spellData.SpellName == "Scathe")
				continue;
			if (!Main.LocalPlayer.GetModPlayer<BlackMagePlayer>().IsSpellLearned(spellId))
			{
				if (_buttons.ContainsKey(spellId))
				{
					_buttons[spellId].Deactivate();
					_area.RemoveChild(_buttons[spellId]);
					_buttons.Remove(spellId);
				}

				continue;
			}

			if (!_buttons.ContainsKey(spellId))
			{
				_buttons.Add(spellId,
				             new SpellButton(spellId,
				                             ModContent.Request<Texture2D>(
					                             "BlackMage/UI/Spells/" + spellData.SpellName))
				);
				_buttons[spellId].Activate();
			}

			SpellButton button = _buttons[spellId];
			button.OnMouseUp += (_, _) => CastSpell(spellId);
			button.Top.Set(0f, 0f);
			button.Left.Set((IconSize + Padding) * i, 0f);
			button.Width.Set(IconSize, 0f);
			button.Height.Set(IconSize, 0f);
			button.ManaText.SetText(Spell.Data[spellId].MPCost > 0 ? Spell.Data[spellId].MPCost.ToString() : "");
			button.ManaText.HAlign = 1f;
			button.ManaText.VAlign = 1f;

			_area.Append(button);
			i++;
		}

		_area.Width.Set((IconSize + Padding) * i, 0f);
	}

	internal class SpellButton : UIImageButton
	{
		private const float BorderThickness = 2f;

		private static   BlackMagePlayer BLM          => Main.LocalPlayer.GetModPlayer<BlackMagePlayer>();
		public           UIText          ManaText     { get; set; }
		public           UIText          CooldownText { get; set; }
		private readonly int?            _spellId;
		private          Spell.SpellData SpellData => _spellId == null ? null : Spell.Data[_spellId.Value];

		public SpellButton(int spellId, Asset<Texture2D> texture) : base(texture)
		{
			_spellId = spellId;

			ManaText = new UIText("");
			Append(ManaText);
			CooldownText = new UIText("");
			Append(CooldownText);

			SetVisibility(1f, 1f);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (_spellId == -1)
				return;

			base.DrawSelf(spriteBatch);

			var hitbox = GetInnerDimensions().ToRectangle();
			int left   = hitbox.Left;
			int top    = hitbox.Top;
			int size   = hitbox.Width;

			if (IsMouseHovering)
			{
				Main.hoverItemName = Constants.ReplaceKeywords($"[{SpellData.SpellName}]") + "\n" +
				                     SpellData.Description;

				for (int y = top; y < top + size; y++)
				{
					if (y == top || y == top + size - 1)
					{
						for (int x = left; x < left + size; x++)
							spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(x, y, 1, 1), Color.White);

						continue;
					}

					spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(left,        y, 1, 1), Color.White);
					spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(left + size, y, 1, 1), Color.White);
				}
			}

			if (_spellId != null && !BLM.SpellCooldowns.ContainsKey(_spellId.Value))
				BLM.SpellCooldowns.Add(_spellId.Value, 0);
			if (_spellId != null && (BLM.SpellCooldowns[_spellId.Value] > 0 ||
			                         (SpellData.GlobalCooldown && BLM.GlobalCooldownTimer > 0)))
			{
				if (BLM.MP >= BLM.GetSpellCost(_spellId.Value))
					ManaText.TextColor *= 0.6f;
				DrawCooldown(spriteBatch);
			}
			else if (_spellId != null && (!BLM.CanCastSpell(_spellId.Value) ||
			                              (BLM.GlobalCooldownTimer > 0 && SpellData.GlobalCooldown) ||
			                              BLM.SpellCooldowns[_spellId.Value] > 0))
			{
				if (BLM.MP >= BLM.GetSpellCost(_spellId.Value))
					ManaText.TextColor *= 0.6f;
				spriteBatch.Draw(TextureAssets.MagicPixel.Value,
				                 new Rectangle(left, top, size, size),
				                 new Color(0f, 0f, 0f, 0.5f));
			}

			if (_spellId != null && BLM.SpellCooldowns[_spellId.Value] == 0 &&
			    !(SpellData.GlobalCooldown && BLM.GlobalCooldownTimer > 0))
				CooldownText.SetText("");
		}

		private void DrawCooldown(SpriteBatch spriteBatch)
		{
			const float twoPi = (float)(2f * Math.PI);

			var   hitbox = GetInnerDimensions().ToRectangle();
			int   left   = hitbox.Left;
			int   top    = hitbox.Top;
			int   size   = hitbox.Width;
			var   center = new Vector2(left + size / 2f, top + size / 2f);
			float radius = size / 2f;

			uint currentTimer = _spellId != null && BLM.SpellCooldowns[_spellId.Value] > 0
				                    ? BLM.SpellCooldowns[_spellId.Value]
				                    : SpellData.GlobalCooldown
					                    ? BLM.GlobalCooldownTimer
					                    : 0;

			CooldownText.SetText(Math.Ceiling(currentTimer / 60f).ToString(CultureInfo.CurrentCulture), 0.8f, false);

			uint maxTimer = _spellId != null && BLM.SpellCooldowns[_spellId.Value] > 0
				                ? SpellData.Cooldown
				                : SpellData.GlobalCooldown
					                ? BlackMagePlayer.GlobalCooldownMaxTime
					                : 0;
			float maxAngle = twoPi * (1f - currentTimer / (float)maxTimer);

			for (int y = top; y < top + size; y++)
			for (int x = left; x < left + size; x++)
			{
				var distance =
					(float)Math.Sqrt(Math.Pow(x + 0.5f - center.X, 2) + Math.Pow(y + 0.5f - center.Y, 2));

				var angle = (float)((Math.Atan2(x + 0.5f - center.X, center.Y - (y + 0.5f)) + twoPi) % twoPi);

				float distFromAngle = Math.Abs((float)(Math.Cos(maxAngle) * (x + 0.5f - center.X) -
				                                       Math.Sin(maxAngle) * (center.Y - (y + 0.5f))));

				if (distance > radius || angle > maxAngle)
					spriteBatch.Draw(TextureAssets.MagicPixel.Value,
					                 new Rectangle(x, y, 1, 1),
					                 new Color(0f, 0f, 0f, 0.5f));
				else if (distance > radius - BorderThickness || distance < BorderThickness ||
				         (distFromAngle <= BorderThickness && maxAngle - angle < twoPi / 4f) ||
				         (x - center.X < BorderThickness && x >= center.X && y <= center.Y))
					spriteBatch.Draw(TextureAssets.MagicPixel.Value,
					                 new Rectangle(x, y, 1, 1),
					                 new Color(1f, 1f, 1f, 1f));
			}
		}
	}
}
