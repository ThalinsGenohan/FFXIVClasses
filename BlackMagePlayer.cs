﻿using System;
using System.Collections.Generic;
using System.Linq;
using BlackMage.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BlackMage
{
	internal class BlackMagePlayer : ModPlayer
	{
		public const float GlobalCooldownSeconds  = 2.5f;
		public const float MPTickSeconds          = 2.5f;
		public const float ElementalChargeSeconds = 15f;
		public const float PolyglotChargeSeconds  = 30f;

		public const uint GlobalCooldownMaxTime  = (int)(GlobalCooldownSeconds * 60);  // 150
		public const uint MPTickMaxTime          = (int)(MPTickSeconds * 60);          // 150
		public const uint ElementalChargeMaxTime = (int)(ElementalChargeSeconds * 60); // 900
		public const uint PolyglotMaxTime        = (int)(PolyglotChargeSeconds * 60);  // 1800
		public const int  MaxMP                  = 10000;

		public const int MaxElementStacks = 3;
		public const int MaxUmbralHearts  = 3;
		public const int MaxPolyglots     = 2;

		public static int[]   MPRegenRate        { get; } = { 6200, 4700, 3200, 200, 0, 0, 0 };
		public static float[] FireMPMultList     { get; } = { 0f, 0f, 0f, 1f, 2f, 2f, 2f };
		public static float[] IceMPMultList      { get; } = { 0f, 0.5f, 0.75f, 1f, 0f, 0f, 0f };
		public static float[] FireDamageMultList { get; } = { 0.7f, 0.8f, 0.9f, 1f, 1.4f, 1.6f, 1.8f };
		public static float[] IceDamageMultList  { get; } = { 1f, 1f, 1f, 1f, 0.9f, 0.8f, 0.7f };

		public Dictionary<int, uint> SpellCooldowns { get; } = new Dictionary<int, uint>();

		public float FireMPMult     => FireMPMultList[(UmbralHearts > 0 && AstralFire > 0 ? 0 : ElementalCharge) + MaxElementStacks];
		public float IceMPMult      => IceMPMultList[ElementalCharge + MaxElementStacks];
		public float FireDamageMult => FireDamageMultList[ElementalCharge + MaxElementStacks];
		public float IceDamageMult  => IceDamageMultList[ElementalCharge + MaxElementStacks];

		public int AllowedElementStacks
		{
			get
			{
				if (SoulCrystalLevel >= 35) return 3;
				if (SoulCrystalLevel >= 20) return 2;
				if (SoulCrystalLevel >= 10) return 1;
				return 0;
			}
		}

		public bool CanHaveUmbralHearts => SoulCrystalLevel >= 60;

		public int AllowedPolyglots
		{
			get
			{
				if (SoulCrystalLevel >= 80) return 2;
				if (SoulCrystalLevel >= 70) return 1;
				return 0;
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

			foreach (int spellId in from spellDataPair in Spell.Data
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
				else SpellCooldowns.Add(spellId, 0);
			}

			if (--MPTickTimer == 0)
			{
				MPTickTimer =  MPTickMaxTime;
				MP          += MPRegenRate[ElementalCharge + MaxElementStacks];
			}

			if (ElementalCharge == 0) return;

			if (--ElementalChargeTimer == 0)
				RemoveElementalStack();

			if (--PolyglotTimer != 0) return;

			Polyglots     = Math.Min(Polyglots + 1, MaxPolyglots);
			PolyglotTimer = PolyglotMaxTime;
		}

		public override void ResetEffects()
		{
			SoulCrystalLevel = 0;
		}

		public bool IsSpellLearned(int spellId) => SoulCrystalLevel >= Spell.Data[spellId].LevelLearned;

		public int GetSpellCost(int spellId)
		{
			Spell.SpellData spellData = Spell.Data[spellId];
			switch (spellData.ElementStack & Elements.ElementMask)
			{
				case Elements.FireElement when (spellData.ElementStack & Elements.StackMask) == Elements.HeartStack &&
				                               UmbralHearts > 0:
					return Math.Max((int)(MP / 1.5f), 800);
				case Elements.FireElement when spellData.MPCost == -1:
					return Math.Max(MP, 800);
				case Elements.FireElement:
					return (int)(spellData.MPCost * FireMPMult);

				case Elements.IceElement:
					return (int)(spellData.MPCost * IceMPMult);
				case Elements.ParadoxElement when UmbralIce > 0:
					return 0;

				default:
					return spellData.MPCost;
			}
		}

		public bool CanCastSpell(int spellId)
		{
			if (GetSpellCost(spellId) > MP)
				return false;

			if (!SpellCooldowns.ContainsKey(spellId))
				SpellCooldowns.Add(spellId, 0);

			Spell.SpellData spellData = Spell.Data[spellId];
			if ((GlobalCooldownTimer > 0 && spellData.GlobalCooldown) || SpellCooldowns[spellId] > 0)
				return false;

			switch (spellData.ElementStack & Elements.ElementMask)
			{
				case Elements.FireElement:
					if (spellData.StackRequired && AstralFire == 0)
						return false;
					break;
				case Elements.IceElement:
					if (spellData.StackRequired && UmbralIce == 0)
						return false;
					break;
				case Elements.ParadoxElement:
					if (!ParadoxReady)
						return false;
					break;
				case Elements.PolyglotElement:
					if (Polyglots == 0)
						return false;
					break;
			}

			return true;
		}

		public void AddElementalStack(int elementStack)
		{
			int element = elementStack & Elements.ElementMask;
			int stack   = elementStack & Elements.StackMask;

			switch (element)
			{
				case Elements.FireElement:
					switch (stack)
					{
						case Elements.FullStack:
						case Elements.HeartStack:
							if (UmbralIce == 3 && UmbralHearts == 3)
								ParadoxReady = true;
							AstralFire           = MaxElementStacks;
							ElementalChargeTimer = ElementalChargeMaxTime;
							break;
						case Elements.OneStack when UmbralIce > 0:
							RemoveElementalStack();
							break;
						case Elements.OneStack:
							AstralFire++;
							ElementalChargeTimer = ElementalChargeMaxTime;
							break;
					}
					break;
				case Elements.IceElement:
					switch (stack)
					{
						case Elements.FullStack:
							if (AstralFire == 3)
								ParadoxReady = true;
							UmbralIce            = MaxElementStacks;
							ElementalChargeTimer = ElementalChargeMaxTime;
							break;
						case Elements.OneStack when AstralFire > 0:
							RemoveElementalStack();
							break;
						case Elements.OneStack:
							UmbralIce++;
							ElementalChargeTimer = ElementalChargeMaxTime;
							break;
					}
					break;
				case Elements.TransposeElement:
					if (AstralFire > 0)
					{
						UmbralIce            = 1;
						ElementalChargeTimer = ElementalChargeMaxTime;
					}
					else if (UmbralIce > 0)
					{
						AstralFire            = 1;
						ElementalChargeTimer = ElementalChargeMaxTime;
					}
					break;
			}
		}

		public void RemoveElementalStack()
		{
			ElementalCharge      = 0;
			UmbralHearts         = 0;
			ElementalChargeTimer = ElementalChargeTimer;
			PolyglotTimer        = PolyglotMaxTime;
		}

		public bool CastSpell(int spellId)
		{
			if (!CanCastSpell(spellId))
				return false;

			Spell.SpellData spellData = Spell.Data[spellId];

			int element = spellData.ElementStack & Elements.ElementMask;
			int stack   = spellData.ElementStack & Elements.StackMask;
			int damage  = Spell.Data[spellId].Potency;

			MP -= GetSpellCost(spellId);

			switch (element)
			{
				case Elements.FireElement:
					if (AstralFire > 0)
						UmbralHearts -= stack == Elements.HeartStack ? UmbralHearts : 1;
					AddElementalStack(spellData.ElementStack);
					damage = (int)(damage * FireDamageMult);
					break;
				case Elements.IceElement:
					AddElementalStack(spellData.ElementStack);
					if (stack == Elements.HeartStack)
						UmbralHearts = MaxUmbralHearts;
					if (spellData.SpellName == "Umbral Soul")
						UmbralHearts++;
					damage = (int)(damage * IceDamageMult);
					break;
				case Elements.ParadoxElement:
					ParadoxReady         =  false;
					ElementalChargeTimer =  ElementalChargeMaxTime;
					break;
				case Elements.PolyglotElement:
					Polyglots--;
					break;
				case Elements.TransposeElement:
					AddElementalStack(spellData.ElementStack);
					break;
			}

			SpellCooldowns[spellId] = spellData.Cooldown;

			if (spellData.GlobalCooldown)
				GlobalCooldownTimer = GlobalCooldownMaxTime;

			if (damage > 0)
				Projectile.NewProjectile(player.position, Vector2.Zero, spellId, damage, 0f, player.whoAmI);

			return true;
		}
	}
}
