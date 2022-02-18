using System;
using Terraria.ModLoader;

namespace BlackMage
{
	internal class BlackMagePlayer : ModPlayer
	{
		public const uint MPTickMaxTime          = 150;
		public const  uint ElementalChargeMaxTime = 900;
		public const  uint PolyglotMaxTime        = 1800;
		public const  int  MaxMP                  = 10000;

		public const int MaxElementStacks   = 3;
		public const int MaxUmbralHearts    = 3;
		public const int MaxPolyglotCharges = 2;

		private static readonly int[]   MPRegenRate        = { 6200, 4700, 3200, 200, 0, 0, 0 };
		private static readonly float[] FireMPMultList     = { 0f, 0f, 0f, 1f, 2f, 2f, 2f };
		private static readonly float[] IceMPMultList      = { 0f, 0.5f, 0.75f, 1f, 0f, 0f, 0f };
		private static readonly float[] FireDamageMultList = { 0.7f, 0.8f, 0.9f, 1f, 1.4f, 1.6f, 1.8f };
		private static readonly float[] IceDamageMultList  = { 1f, 1f, 1f, 1f, 0.9f, 0.8f, 0.7f };

		public float FireMPMult => FireMPMultList[ElementalCharge + MaxElementStacks];
		public float IceMPMult  => IceMPMultList[ElementalCharge + MaxElementStacks];
		public float FireDamageMult => FireDamageMultList[ElementalCharge + MaxElementStacks];
		public float IceDamageMult  => IceDamageMultList[ElementalCharge + MaxElementStacks];

		public bool SoulCrystal { get; set; }

		public int MP
		{
			get => _mp;
			set => _mp = Math.Max(Math.Min(value, MaxMP), 0);
		}

		public int ElementalCharge
		{
			get => _elementalCharge;
			set => _elementalCharge = Math.Max(Math.Min(value, MaxElementStacks), -MaxElementStacks);
		}
		public int AstralFire
		{
			get => Math.Max(ElementalCharge, 0);
			set => ElementalCharge = Math.Max(Math.Min(value, MaxElementStacks), 0);
		}
		public int UmbralIce
		{
			get => Math.Max(-ElementalCharge, 0);
			set => ElementalCharge = -Math.Max(Math.Min(value, MaxElementStacks), 0);
		}

		public int UmbralHearts
		{
			get => _umbralHearts;
			set => _umbralHearts = Math.Max(Math.Min(value, MaxUmbralHearts), 0);
		}

		public int  PolyglotCharges
		{
			get => _polyglotCharges;
			set => _polyglotCharges = Math.Max(Math.Min(value, MaxPolyglotCharges), 0);
		}

		public bool ParadoxReady    { get; set; }

		public uint MPTickTimer          { get; set; } = MPTickMaxTime;
		public uint ElementalChargeTimer { get; set; } = ElementalChargeMaxTime;
		public uint PolyglotTimer        { get; set; } = PolyglotMaxTime;

		private int _mp;
		private int _elementalCharge;
		private int _umbralHearts;
		private int _polyglotCharges;

		public override void PostUpdateMiscEffects()
		{
			if (--MPTickTimer == 0)
			{
				MPTickTimer =  MPTickMaxTime;
				MP          += MPRegenRate[ElementalCharge + MaxElementStacks];
			}

			if (ElementalCharge == 0) return;

			if (--ElementalChargeTimer == 0)
			{
				RemoveElementalStack();
			}

			if (--PolyglotTimer != 0) return;

			PolyglotCharges = Math.Min(PolyglotCharges + 1, MaxPolyglotCharges);
			PolyglotTimer   = PolyglotMaxTime;
		}

		public override void ResetEffects()
		{
			SoulCrystal = false;
		}

		public void AddElementalStack(int stack)
		{
			if (stack == MaxElementStacks) // Fire III
			{
				AstralFire           = MaxElementStacks;
				ElementalChargeTimer = ElementalChargeMaxTime;
			}
			else if (stack > 0) // Fire
			{
				if (UmbralIce > 0)
					RemoveElementalStack();
				else
				{
					AstralFire++;
					ElementalChargeTimer = ElementalChargeMaxTime;
				}
			}
			else if (stack == -MaxElementStacks) // Blizzard III
			{
				UmbralIce            = MaxElementStacks;
				ElementalChargeTimer = ElementalChargeMaxTime;
			}
			else if (stack < 0) // Blizzard
			{
				if (AstralFire > 0)
					RemoveElementalStack();
				else
				{
					UmbralIce++;
					ElementalChargeTimer = ElementalChargeMaxTime;
				}
			}
		}

		public void RemoveElementalStack()
		{
			ElementalCharge      = 0;
			UmbralHearts         = 0;
			ElementalChargeTimer = ElementalChargeTimer;
			PolyglotTimer        = PolyglotMaxTime;
		}
	}
}
