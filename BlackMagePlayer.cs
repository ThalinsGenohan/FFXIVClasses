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
		public const uint GlobalCooldownMaxTime  = 150;
		public const uint MPTickMaxTime          = 150;
		public const uint ElementalChargeMaxTime = 900;
		public const uint PolyglotMaxTime        = 1800;
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

		public float FireMPMult     => FireMPMultList[ElementalCharge + MaxElementStacks];
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

		public List<int> CastableSpells
		{
			get
			{
				var list = new List<int>();

				if (SoulCrystalLevel >= 10)
				{
					list.Add(ModContent.ProjectileType<Fire>());
					list.Add(ModContent.ProjectileType<Blizzard>());
					list.Add(ModContent.ProjectileType<Transpose>());
				}
				if (SoulCrystalLevel >= 20)
				{
					list.Add(ModContent.ProjectileType<Fire2>());
					list.Add(ModContent.ProjectileType<Blizzard2>());
				}
				if (SoulCrystalLevel >= 35)
				{
					list.Add(ModContent.ProjectileType<Fire3>());
					list.Add(ModContent.ProjectileType<Blizzard3>());
				}
				if (SoulCrystalLevel >= 50)
				{
					list.Add(ModContent.ProjectileType<Flare>());
					list.Add(ModContent.ProjectileType<Freeze>());
				}
				if (SoulCrystalLevel >= 60)
				{
					list.Add(ModContent.ProjectileType<Fire4>());
					list.Add(ModContent.ProjectileType<Blizzard4>());
				}
				if (SoulCrystalLevel >= 70)
					list.Add(ModContent.ProjectileType<Foul>());
				if (SoulCrystalLevel >= 75)
					list.Add(ModContent.ProjectileType<Despair>());
				if (SoulCrystalLevel >= 80)
				{
					list.Add(ModContent.ProjectileType<UmbralSoul>());
					list.Add(ModContent.ProjectileType<Xenoglossy>());
				}
				if (SoulCrystalLevel >= 90)
					list.Add(ModContent.ProjectileType<Paradox>());
				
				return list;
			}
		}

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
			if (!SpellCooldowns.ContainsKey(spellId))
				SpellCooldowns.Add(spellId, 0);

			if (GlobalCooldownTimer > 0 || SpellCooldowns[spellId] > 0) return false;

			Spell.SpellData spellData = Spell.Data[spellId];

			int element = spellData.ElementStack & Elements.ElementMask;
			int stack   = spellData.ElementStack & Elements.StackMask;
			int mp;
			int damage = Spell.Data[spellId].Potency;

			switch (element)
			{
				case Elements.FireElement:
					if (spellData.StackRequired && AstralFire == 0) return false;

					if (spellData.MPCost == -1)
					{
						if (MP < 800) return false;
						mp = MP / (stack == Elements.HeartStack && UmbralHearts > 0 ? 3 : 1);
					}
					else
					{
						mp = (int)(spellData.MPCost * FireMPMult);
						if (MP < mp) return false;
					}

					MP -= mp;
					if (AstralFire > 0)
						UmbralHearts -= stack == Elements.HeartStack ? UmbralHearts : 1;
					AddElementalStack(spellData.ElementStack);

					damage = (int)(damage * FireDamageMult);
					break;
				case Elements.IceElement:
					if (spellData.StackRequired && UmbralIce == 0) return false;

					mp = (int)(spellData.MPCost * IceMPMult);
					if (MP < mp) return false;

					MP -= mp;
					AddElementalStack(spellData.ElementStack);
					if (stack == Elements.HeartStack)
						UmbralHearts = MaxUmbralHearts;
					if (spellData.SpellName == "Umbral Soul")
						UmbralHearts++;

					damage = (int)(damage * IceDamageMult);
					break;
				case Elements.ParadoxElement:
					if (!ParadoxReady) return false;

					mp = UmbralIce > 0 ? 0 : spellData.MPCost;

					if (MP < mp) return false;

					MP                   -= mp;
					ParadoxReady         =  false;
					ElementalChargeTimer =  ElementalChargeMaxTime;
					break;
				case Elements.PolyglotElement:
					if (Polyglots == 0) return false;

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
