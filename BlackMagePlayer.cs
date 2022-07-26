using System;
using System.Collections.Generic;
using System.Linq;
using BlackMage.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BlackMage;

internal class BlackMagePlayer : ModPlayer
{
	public const float GlobalCooldownSeconds  = 2.5f;
	public const float MPTickSeconds          = 2.5f;
	public const float ElementalChargeSeconds = 15f;
	public const float PolyglotChargeSeconds  = 30f;

	public const uint GlobalCooldownMaxTime  = (uint)(GlobalCooldownSeconds * 60);  // 150
	public const uint MPTickMaxTime          = (uint)(MPTickSeconds * 60);          // 150
	public const uint ElementalChargeMaxTime = (uint)(ElementalChargeSeconds * 60); // 900
	public const uint PolyglotMaxTime        = (uint)(PolyglotChargeSeconds * 60);  // 1800
	public const int  MaxMP                  = 10000;

	public const int MaxElementStacks = 3;
	public const int MaxUmbralHearts  = 3;
	public const int MaxPolyglots     = 2;
	public const int MinAllMP         = 800;

	public static (
		int MPRegenRate,
		float FireMPMultiplier,
		float IceMPMultiplier,
		float FireDamageMultiplier,
		float IceDamageMultiplier
		)[] ElementChargeValues { get; } =
	{
		// MP  FireMP  IceMP  FireDam IceDam
		(6200, 0f,     0f,    0.7f,   1f  ), // Umbral Ice III
		(4700, 0f,     0.5f,  0.8f,   1f  ), // Umbral Ice II
		(3200, 0f,     0.75f, 0.9f,   1f  ), // Umbral Ice I
		( 200, 1f,     1f,    1f,     1f  ), // No Charge
		(   0, 2f,     0f,    1.4f,   0.9f), // Astral Fire I
		(   0, 2f,     0f,    1.6f,   0.8f), // Astral Fire II
		(   0, 2f,     0f,    1.8f,   0.7f), // Astral Fire III
	};

	public Dictionary<string, uint> SpellCooldowns { get; } = new();

	public float FireMPMultiplier => UmbralHearts > 0 && AstralFire > 0
		                                 ? ElementChargeValues[0 + MaxElementStacks].FireMPMultiplier
		                                 : ElementChargeValues[ElementalCharge + MaxElementStacks].FireMPMultiplier;

	public float IceMPMultiplier      => ElementChargeValues[ElementalCharge + MaxElementStacks].IceMPMultiplier;
	public float FireDamageMultiplier => ElementChargeValues[ElementalCharge + MaxElementStacks].FireDamageMultiplier;
	public float IceDamageMultiplier  => ElementChargeValues[ElementalCharge + MaxElementStacks].IceDamageMultiplier;

	public int AllowedElementStacks
	{
		get
		{
			return SoulCrystalLevel switch
			{
				>= 35 => 3,
				>= 20 => 2,
				>= 10 => 1,
				_     => 0,
			};
		}
	}

	public bool CanHaveUmbralHearts => SoulCrystalLevel >= 60;

	public int AllowedPolyglots
	{
		get
		{
			return SoulCrystalLevel switch
			{
				>= 80 => 2,
				>= 70 => 1,
				_     => 0,
			};
		}
	}

	public bool CanUseParadox => SoulCrystalLevel >= 90;

	public int SoulCrystalLevel { get; set; }

	public int MP
	{
		get => _mp;
		set => _mp = Math.Max(Math.Min(value, MaxMP), 0);
	}

	public int ElementalCharge
	{
		get => _elementalCharge;
		set => _elementalCharge = Math.Max(Math.Min(value, AllowedElementStacks), -AllowedElementStacks);
	}

	public int AstralFire
	{
		get => Math.Max(ElementalCharge, 0);
		set => ElementalCharge = Math.Max(Math.Min(value, AllowedElementStacks), 0);
	}

	public int UmbralIce
	{
		get => Math.Max(-ElementalCharge, 0);
		set => ElementalCharge = -Math.Max(Math.Min(value, AllowedElementStacks), 0);
	}

	public int UmbralHearts
	{
		get => _umbralHearts;
		set => _umbralHearts = CanHaveUmbralHearts ? Math.Max(Math.Min(value, MaxUmbralHearts), 0) : 0;
	}

	public int Polyglots
	{
		get => _polyglots;
		set => _polyglots = Math.Max(Math.Min(value, AllowedPolyglots), 0);
	}

	public bool ParadoxReady
	{
		get => _paradoxReady;
		set => _paradoxReady = value && CanUseParadox;
	}

	public bool Firestarter { get; set; }

	public string CurrentSpell { get; set; } = "";

	public uint CastTimer            { get; set; } = 0;
	public uint MaxCastTimer         { get; set; } = 0;
	public uint GlobalCooldownTimer  { get; set; } = GlobalCooldownMaxTime;
	public uint MPTickTimer          { get; set; } = MPTickMaxTime;
	public uint ElementalChargeTimer { get; set; } = ElementalChargeMaxTime;
	public uint PolyglotTimer        { get; set; } = PolyglotMaxTime;

	private int  _mp;
	private int  _elementalCharge;
	private int  _umbralHearts;
	private int  _polyglots;
	private bool _paradoxReady;

	public override void PostUpdateMiscEffects()
	{
		if (GlobalCooldownTimer > 0)
			GlobalCooldownTimer--;

		if (CastTimer > 0)
			CastTimer--;

		if (CastTimer == 0 && CurrentSpell != "")
		{
			CastSpell(CurrentSpell);
			CurrentSpell = "";
		}

		foreach (string spellId in from spellDataPair in Spell.Data
		                           let spellId = spellDataPair.Key
		                           let spellData = spellDataPair.Value
		                           where spellData.Cooldown > 0
		                           select spellId)
		{
			if (SpellCooldowns.ContainsKey(spellId))
			{
				if (SpellCooldowns[spellId] > 0)
					SpellCooldowns[spellId]--;
			}
			else
			{
				SpellCooldowns.Add(spellId, 0);
			}
		}

		if (--MPTickTimer == 0)
		{
			MPTickTimer =  MPTickMaxTime;
			MP          += ElementChargeValues[ElementalCharge + MaxElementStacks].MPRegenRate;
		}

		if (ElementalCharge == 0) return;

		if (--ElementalChargeTimer == 0)
			ResetElementalStack();

		if (--PolyglotTimer != 0) return;

		Polyglots     = Math.Min(Polyglots + 1, MaxPolyglots);
		PolyglotTimer = PolyglotMaxTime;
	}

	public override void ResetEffects()
	{
		SoulCrystalLevel = 0;
	}

	public bool IsSpellLearned(string spellId) => SoulCrystalLevel >= Spell.Data[spellId].LevelLearned;

	public int GetSpellCost(string spellId)
	{
		Spell.SpellData spellData = Spell.Data[spellId];

		return spellData.MPCost switch
		{
			-2 => Math.Max(
				(int)(MP / (UmbralHearts > 0 ? 1.5f : 1f)),
				MinAllMP
			),
			-1 => Math.Max(MP, MinAllMP),
			_ => spellData.Element switch
			{
				Constants.Elements.FireElement                       => (int)(spellData.MPCost * FireMPMultiplier),
				Constants.Elements.IceElement                        => (int)(spellData.MPCost * IceMPMultiplier),
				Constants.Elements.ParadoxElement when UmbralIce > 0 => 0,
				_                                                    => spellData.MPCost,
			},
		};
	}

	public bool CanCastSpell(string spellId)
	{
		if (!Player.HasMinionAttackTargetNPC)
			return false;

		if (GetSpellCost(spellId) > MP)
			return false;

		if (!SpellCooldowns.ContainsKey(spellId))
			SpellCooldowns.Add(spellId, 0);

		Spell.SpellData spellData = Spell.Data[spellId];
		/*if ((GlobalCooldownTimer > 0 && spellData.GlobalCooldown) || SpellCooldowns[spellId] > 0)
			return false;*/

		switch (spellData.Element)
		{
			case Constants.Elements.FireElement:
				if (spellData.StackRequired && AstralFire == 0)
					return false;
				break;
			case Constants.Elements.IceElement:
				if (spellData.StackRequired && UmbralIce == 0)
					return false;
				break;
			case Constants.Elements.ParadoxElement:
				if (!ParadoxReady)
					return false;
				break;
			case Constants.Elements.PolyglotElement:
				if (Polyglots == 0)
					return false;
				break;

			case Constants.Elements.NoElement:
			case Constants.Elements.TransposeElement:
			default:
				break;
		}

		return true;
	}

	public void SetElementalStack(int elementStack)
	{
		if (Math.Abs(ElementalCharge) == MaxElementStacks && (AstralFire > 0 || UmbralHearts == MaxUmbralHearts))
			ParadoxReady = true;

		ElementalCharge      = elementStack;
		ElementalChargeTimer = ElementalChargeMaxTime;
	}

	public void AddElementalStack(int elementStack)
	{
		if (ElementalCharge * elementStack < 0)
		{
			ResetElementalStack();
		}
		else
		{
			ElementalCharge      += elementStack;
			ElementalChargeTimer =  ElementalChargeMaxTime;
		}
	}

	public void ResetElementalStack()
	{
		ElementalCharge      = 0;
		UmbralHearts         = 0;
		ElementalChargeTimer = ElementalChargeTimer;
		PolyglotTimer        = PolyglotMaxTime;
	}

	public void BeginSpellCast(string spellId)
	{
		if (!CanCastSpell(spellId) || (GlobalCooldownTimer > 0 && Spell.Data[spellId].GlobalCooldown) ||
		    SpellCooldowns[spellId] > 0)
			return;

		CurrentSpell = spellId;

		Spell.SpellData spellData = Spell.Data[spellId];

		MaxCastTimer = spellData.CastTime;
		if ((spellData.Element == Constants.Elements.FireElement && UmbralIce == MaxElementStacks) ||
		    (spellData.Element == Constants.Elements.IceElement && AstralFire == MaxElementStacks))
			MaxCastTimer /= 2;

		if (spellData.Element == Constants.Elements.ParadoxElement && UmbralIce > 0)
			MaxCastTimer = 0;

		CastTimer = MaxCastTimer;
		if (Spell.Data[spellId].GlobalCooldown)
			GlobalCooldownTimer = GlobalCooldownMaxTime;
	}

	public bool CastSpell(string spellId)
	{
		if (!CanCastSpell(spellId))
			return false;

		int damage = Spell.Data[spellId].Potency;

		MP -= GetSpellCost(spellId);

		Spell.SpellData spellData = Spell.Data[spellId];
		switch (spellData.Element)
		{
			case Constants.Elements.FireElement:
				if (AstralFire > 0 && UmbralHearts > 0 && spellData.MPCost > 0)
					UmbralHearts--;
				damage = (int)(damage * FireDamageMultiplier);
				break;
			case Constants.Elements.IceElement:
				damage = (int)(damage * IceDamageMultiplier);
				break;
			case Constants.Elements.ParadoxElement:
				ParadoxReady = false;
				break;
			case Constants.Elements.PolyglotElement:
				Polyglots--;
				break;

			case Constants.Elements.NoElement:
			case Constants.Elements.TransposeElement:
			default: break;
		}

		spellData.OnCastEffect(Player);

		SpellCooldowns[spellId] = spellData.Cooldown;

		if (damage > 0)
			Projectile.NewProjectile(Player.GetSource_Misc("spell"),
			                         Player.position,
			                         Vector2.Zero,
			                         spellData.ProjectileID,
			                         damage,
			                         0f,
			                         Player.whoAmI);

		return true;
	}
}
