using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BlackMage
{
	internal class BlackMagePlayer : ModPlayer
	{
		public const uint GlobalCooldownMaxTime  = 150;
		public const uint MPTickMaxTime          = 150;
		public const uint ElementalChargeMaxTime = 900;
		public const uint PolyglotMaxTime        = 1800;
		public const int  MaxMP                  = 10000;

		public const int MaxElementStacks = 3;
		public const int MaxUmbralHearts  = 3;
		public const int MaxPolyglots     = 2;

		private static readonly int[]   MPRegenRate        = { 6200, 4700, 3200, 200, 0, 0, 0 };
		private static readonly float[] FireMPMultList     = { 0f, 0f, 0f, 1f, 2f, 2f, 2f };
		private static readonly float[] IceMPMultList      = { 0f, 0.5f, 0.75f, 1f, 0f, 0f, 0f };
		private static readonly float[] FireDamageMultList = { 0.7f, 0.8f, 0.9f, 1f, 1.4f, 1.6f, 1.8f };
		private static readonly float[] IceDamageMultList  = { 1f, 1f, 1f, 1f, 0.9f, 0.8f, 0.7f };

		public Dictionary<Spell, uint> SpellCooldowns { get; } = new();

		public float FireMPMult     => FireMPMultList[MaxElementStacks + (ElementalCharge > 0 ? 0 : ElementalCharge)];
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

			foreach (Spell spell in SpellCooldowns.Keys.Where(spell => SpellCooldowns[spell] > 0))
				SpellCooldowns[spell]--;

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
			}
		}

		public void RemoveElementalStack()
		{
			ElementalCharge      = 0;
			UmbralHearts         = 0;
			ElementalChargeTimer = ElementalChargeMaxTime;
			PolyglotTimer        = PolyglotMaxTime;
		}

		public bool CastSpell(Spell spell)
		{
			if (!SpellCooldowns.ContainsKey(spell))
				SpellCooldowns.Add(spell, 0);

			if (GlobalCooldownTimer > 0 || SpellCooldowns[spell] > 0) return false;

			int element = spell.ElementStack & Elements.ElementMask;
			int stack   = spell.ElementStack & Elements.StackMask;
			int mp;

			switch (element)
			{
				case Elements.FireElement:
					if (spell.StackRequired && AstralFire == 0) return false;

					if (spell.MPCost == -1)
					{
						if (MP < 800) return false;
						mp = MP / (stack == Elements.HeartStack && UmbralHearts > 0 ? 3 : 1);
					}
					else
					{
						mp = (int)(spell.MPCost * FireMPMult);
						if (MP < mp) return false;
					}

					MP -= mp;
					AddElementalStack(spell.ElementStack);
					if (AstralFire > 0)
						UmbralHearts -= stack == Elements.HeartStack ? UmbralHearts : 1;

					break;
				case Elements.IceElement:
					if (spell.StackRequired && UmbralIce == 0) return false;

					mp = (int)(spell.MPCost * IceMPMult);
					if (MP < mp) return false;

					MP -= mp;
					AddElementalStack(spell.ElementStack);
					if (stack == Elements.HeartStack)
						UmbralHearts = MaxUmbralHearts;

					break;
				case Elements.ParadoxElement:
					if (!ParadoxReady) return false;

					mp = UmbralIce > 0 ? 0 : spell.MPCost;

					if (MP < mp) return false;

					MP                   -= mp;
					ParadoxReady         =  false;
					ElementalChargeTimer =  ElementalChargeMaxTime;
					break;
				case Elements.PolyglotElement:
					if (Polyglots == 0) return false;

					Polyglots--;
					break;
			}

			SpellCooldowns[spell] = spell.Cooldown;

			if (spell.GlobalCooldown)
				GlobalCooldownTimer = GlobalCooldownMaxTime;

			Projectile.NewProjectile(Player.GetSource_Misc("spell"), Player.position, Vector2.Zero, spell.Projectile, spell.Potency, 0f);

			return true;
		}
	}
}
