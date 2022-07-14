using System;
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

		public const uint GlobalCooldownMaxTime  = (uint)(GlobalCooldownSeconds * 60);  // 150
		public const uint MPTickMaxTime          = (uint)(MPTickSeconds * 60);          // 150
		public const uint ElementalChargeMaxTime = (uint)(ElementalChargeSeconds * 60); // 900
		public const uint PolyglotMaxTime        = (uint)(PolyglotChargeSeconds * 60);  // 1800
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

		public bool Firestarter   { get; set; }

		public int? CurrentSpell { get; set; } = null;

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

			if (CastTimer == 0 && CurrentSpell != null)
			{
				CastSpell(CurrentSpell.Value);
				CurrentSpell = null;
			}

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
			switch (spellData.Element)
			{
				case Constants.Elements.FireElement when spellData.MPCost == -1 && UmbralHearts > 0:
					return Math.Max((int)(MP / 1.5f), 800);
				case Constants.Elements.FireElement when spellData.MPCost == -1:
					return Math.Max(MP, 800);
				case Constants.Elements.FireElement:
					return (int)(spellData.MPCost * FireMPMult);
				case Constants.Elements.IceElement:
					return (int)(spellData.MPCost * IceMPMult);
				case Constants.Elements.ParadoxElement when UmbralIce > 0:
					return 0;

				case Constants.Elements.NoElement:
				case Constants.Elements.PolyglotElement:
				case Constants.Elements.TransposeElement:
				default:
					return spellData.MPCost;
			}
		}

		public bool CanCastSpell(int spellId)
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

		public void AddElementalStack(int elementStack, bool swapElement)
		{
			if (elementStack == 0)
			{
				if (AstralFire > 0)
					elementStack = -1;
				if (UmbralIce > 0)
					elementStack = 1;

				ElementalCharge = 0;
			}

			if (elementStack > 0)
			{
				if (UmbralIce == MaxElementStacks && UmbralHearts == MaxUmbralHearts && !swapElement)
					ParadoxReady = true;

				if (UmbralIce > 0 && !swapElement)
					RemoveElementalStack();
				else
					AstralFire += elementStack;

				ElementalChargeTimer = ElementalChargeMaxTime;
			}
			else if (elementStack < 0)
			{
				if (AstralFire == MaxElementStacks && !swapElement)
					ParadoxReady = true;

				if (AstralFire > 0 && !swapElement)
					RemoveElementalStack();
				else
					UmbralIce += elementStack;

				ElementalChargeTimer = ElementalChargeMaxTime;
			}
		}

		public void RemoveElementalStack()
		{
			ElementalCharge      = 0;
			UmbralHearts         = 0;
			ElementalChargeTimer = ElementalChargeTimer;
			PolyglotTimer        = PolyglotMaxTime;
		}

		public void BeginSpellCast(int spellId)
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

			CastTimer = MaxCastTimer;
			if (Spell.Data[spellId].GlobalCooldown)
				GlobalCooldownTimer = GlobalCooldownMaxTime;
		}

		public bool CastSpell(int spellId)
		{
			if (!CanCastSpell(spellId))
				return false;

			Spell.SpellData spellData = Spell.Data[spellId];
			
			int damage  = Spell.Data[spellId].Potency;

			MP -= GetSpellCost(spellId);

			switch (spellData.Element)
			{
				case Constants.Elements.FireElement:
					if (AstralFire > 0 && UmbralHearts > 0)
						UmbralHearts--;
					damage = (int)(damage * FireDamageMult);
					break;
				case Constants.Elements.IceElement:
					damage = (int)(damage * IceDamageMult);
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
				Projectile.NewProjectile(Player.position, Vector2.Zero, spellId, damage, 0f, Player.whoAmI);

			return true;
		}
	}
}
