﻿using System;
using System.Collections.Generic;
using BlackMage.Buffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlackMage.Projectiles;

public abstract class Spell : ModProjectile
{
	public static Dictionary<string, SpellData> Data { get; } = new();

	public const int SingleTargetSize = 5;
	public const int AoESize          = 80;

	protected virtual bool AoE => false;

	protected NPC Target;

	public override void SetStaticDefaults()
	{
		ProjectileID.Sets.CultistIsResistantTo[Projectile.type]    = true;
		ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
	}

	public override void SetDefaults()
	{
		Projectile.width                = AoE ? AoESize : SingleTargetSize;
		Projectile.height               = AoE ? AoESize : SingleTargetSize;
		Projectile.friendly             = true;
		Projectile.DamageType           = DamageClass.Magic;
		Projectile.knockBack            = 0f;
		Projectile.tileCollide          = false;
		Projectile.penetrate            = AoE ? -1 : 1;
		Projectile.timeLeft             = 120;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown  = -1;
	}

	public override bool? CanHitNPC(NPC target)
	{
		if (AoE)
			return true;

		return Target?.whoAmI == target.whoAmI;
	}

	public override void ModifyHitNPC(NPC target,
	                                  ref int damage,
	                                  ref float knockback,
	                                  ref bool crit,
	                                  ref int hitDirection)
	{
		crit = false;
	}

	public override void AI()
	{
		if (AoE && Target != null)
			return;

		Player  player       = Main.player[Projectile.owner];
		Vector2 targetCenter = Projectile.position;
		var     foundTarget  = false;

		if (player.HasMinionAttackTargetNPC)
		{
			NPC   npc     = Main.npc[player.MinionAttackTargetNPC];
			float between = Vector2.Distance(npc.Center, Projectile.Center);
			if (between < 2000f)
			{
				targetCenter = npc.Center;
				foundTarget  = true;
				Target       = npc;
			}
		}

		if (!foundTarget)
		{
			Projectile.Kill();
			return;
		}

		Projectile.Center = targetCenter;
	}

	public class SpellData
	{
		public string             SpellName        { get; set; } = "";
		public int                ProjectileID     { get; set; } = 0;
		public int                Potency          { get; set; } = 0;
		public int                MPCost           { get; set; } = 0;
		public uint               CastTime         { get; set; } = 0;
		public uint               Cooldown         { get; set; } = 0;
		public bool               GlobalCooldown   { get; set; } = true;
		public Constants.Elements Element          { get; set; } = Constants.Elements.NoElement;
		public bool               StackRequired    { get; set; } = false;
		public string             Description      { get; set; } = "PLACEHOLDER\nYou shouldn't be seeing this.";
		public int                LevelLearned     { get; set; } = 0;
		public Action<Player>     OnCastEffect     { get; set; } = _ => throw new NotImplementedException();
		public Func<Player, bool> ShouldButtonGlow { get; set; } = _ => false;
	}
}

internal class Blizzard : Spell
{
	protected override bool AoE => false;

	public override void SetStaticDefaults()
	{
		Data["blizzard"] = new SpellData
		{
			SpellName      = "Blizzard",
			ProjectileID   = Projectile.type,
			Potency        = 180,
			MPCost         = 400,
			CastTime       = 150,
			Cooldown       = 0,
			GlobalCooldown = true,
			Element        = Constants.Elements.IceElement,
			StackRequired  = false,
			LevelLearned   = 1,
			OnCastEffect = player =>
			{
				var blm = player.GetModPlayer<BlackMagePlayer>();
				blm.AddElementalStack(-1);
			},
		};
		DisplayName.SetDefault(Data["blizzard"].SpellName);
		Data["blizzard"].Description =
			$"Deals ice damage with a potency of {Data["blizzard"].Potency}.\n" +
			"Additional Effect: Grants [Umbral Ice] or removes [Astral Fire]\n" +
			$"Duration: {BlackMagePlayer.ElementalChargeSeconds}s";
		base.SetStaticDefaults();
	}
}

internal class Fire : Spell
{
	protected override bool AoE => false;

	public override void SetStaticDefaults()
	{
		Data["fire"] = new SpellData
		{
			SpellName      = "Fire",
			ProjectileID   = Projectile.type,
			Potency        = 180,
			MPCost         = 800,
			CastTime       = 150,
			Cooldown       = 0,
			GlobalCooldown = true,
			Element        = Constants.Elements.FireElement,
			StackRequired  = false,
			LevelLearned   = 2,
			OnCastEffect = player =>
			{
				var blm = player.GetModPlayer<BlackMagePlayer>();
				blm.AddElementalStack(1);

				if (blm.SoulCrystalLevel < 42 || Main.rand.Next(0, 9) >= 4)
					return;

				player.AddBuff(ModContent.BuffType<Firestarter>(), 1800);
			},
		};
		DisplayName.SetDefault(Data["fire"].SpellName);
		Data["fire"].Description =
			$"Deals fire damage with a potency of {Data["fire"].Potency}.\n" +
			"Additional Effect: Grants [Astral Fire] or removes [Umbral Ice]\n" +
			$"Duration: {BlackMagePlayer.ElementalChargeSeconds}s\n" +
			"Additional Effect: 40% chance the next [Firaga] will cost no [MP] and have no cast time\n" +
			"Duration: 30s";
		base.SetStaticDefaults();
	}

	public override void AI()
	{
		Player  player       = Main.player[Projectile.owner];
		Vector2 targetCenter = Projectile.position;
		var     foundTarget  = false;

		if (player.HasMinionAttackTargetNPC)
		{
			NPC   npc     = Main.npc[player.MinionAttackTargetNPC];
			float between = Vector2.Distance(npc.Center, Projectile.Center);
			if (between < 2000f)
			{
				targetCenter = npc.Center;
				foundTarget  = true;
				Target       = npc;
			}
		}

		if (!foundTarget)
			for (var i = 0; i < Main.maxNPCs; i++)
			{
				NPC npc = Main.npc[i];
				if (!npc.CanBeChasedBy()) continue;

				float npcDistance = Vector2.Distance(npc.Center, Projectile.Center);
				bool  closest     = Vector2.Distance(Projectile.Center, targetCenter) > npcDistance;

				if (!closest && foundTarget) continue;

				targetCenter = npc.Center;
				foundTarget  = true;
				Target       = npc;
			}

		if (!foundTarget)
		{
			Projectile.Kill();
			return;
		}

		const float speed   = 24f;
		const float inertia = 8f;

		if (!(Vector2.Distance(Projectile.Center, targetCenter) > 40f)) return;

		Vector2 direction = targetCenter - Projectile.Center;
		direction.Normalize();
		direction           *= speed;
		Projectile.velocity =  (Projectile.velocity * (inertia - 1) + direction) / inertia;
	}
}

internal class Transpose : Spell
{
	protected override bool AoE => false;

	public override void SetStaticDefaults()
	{
		Data["transpose"] = new SpellData
		{
			SpellName      = "Transpose",
			ProjectileID   = Projectile.type,
			Potency        = 0,
			MPCost         = 0,
			CastTime       = 0,
			Cooldown       = 300,
			GlobalCooldown = false,
			Element        = Constants.Elements.TransposeElement,
			StackRequired  = false,
			Description =
				"Swaps [Astral Fire] with a single [Umbral Ice], or [Umbral Ice] with a single [Astral Fire].",
			LevelLearned = 4,
			OnCastEffect = player =>
			{
				var blm = player.GetModPlayer<BlackMagePlayer>();
				
				if (blm.AstralFire > 0)
					blm.SetElementalStack(-1);
				else if (blm.UmbralIce > 0)
					blm.SetElementalStack(1);
			},
		};
		DisplayName.SetDefault(Data["transpose"].SpellName);
		base.SetStaticDefaults();
	}

	public override void AI()
	{
		Projectile.Kill();
	}
}

internal class Blizzard2 : Spell
{
	protected override bool AoE => true;

	public override void SetStaticDefaults()
	{
		Data["blizzard2"] = new SpellData
		{
			SpellName      = "Blizzara",
			ProjectileID   = Projectile.type,
			Potency        = 100,
			MPCost         = 800,
			CastTime       = 180,
			Cooldown       = 0,
			GlobalCooldown = true,
			Element        = Constants.Elements.IceElement,
			StackRequired  = false,
			LevelLearned   = 12,
			OnCastEffect = player =>
			{
				var blm = player.GetModPlayer<BlackMagePlayer>();
				blm.SetElementalStack(-BlackMagePlayer.MaxElementStacks);
			},
		};
		DisplayName.SetDefault(Data["blizzard2"].SpellName);
		Data["blizzard2"].Description =
			$"Deals ice damage with a potency of {Data["blizzard2"].Potency} to target and all enemies nearby it.\n" +
			"Additional Effect: Grants [Umbral Ice] III and removes [Astral Fire]\n" +
			$"Duration: {BlackMagePlayer.ElementalChargeSeconds}s";
		base.SetStaticDefaults();
	}
}

internal class Scathe : Spell
{
	protected override bool AoE => false;

	public override void SetStaticDefaults()
	{
		Data["scathe"] = new SpellData
		{
			SpellName      = "Scathe",
			ProjectileID   = Projectile.type,
			Potency        = 100,
			MPCost         = 800,
			CastTime       = 0,
			Cooldown       = 0,
			GlobalCooldown = true,
			Element        = Constants.Elements.NoElement,
			StackRequired  = false,
			LevelLearned   = 0,
			OnCastEffect = player =>
			{
				var blm = player.GetModPlayer<BlackMagePlayer>();
			},
		};
		DisplayName.SetDefault(Data["scathe"].SpellName);
		Data["scathe"].Description =
			$"Deals unaspected damage with a potency of {Data["scathe"].Potency}.\n" +
			"Additional Effect: 20% chance potency will double";
		base.SetStaticDefaults();
	}

	public override void ModifyHitNPC(NPC target,
	                                  ref int damage,
	                                  ref float knockback,
	                                  ref bool crit,
	                                  ref int hitDirection)
	{
		crit = Main.rand.NextBool(5);
		if (crit)
			damage *= 2;
	}
}

internal class Fire2 : Spell
{
	protected override bool AoE => true;

	public override void SetStaticDefaults()
	{
		Data["fire2"] = new SpellData
		{
			SpellName      = "Fira",
			ProjectileID   = Projectile.type,
			Potency        = 100,
			MPCost         = 1500,
			CastTime       = 180,
			Cooldown       = 0,
			GlobalCooldown = true,
			Element        = Constants.Elements.FireElement,
			StackRequired  = false,
			LevelLearned   = 18,
			OnCastEffect = player =>
			{
				var blm = player.GetModPlayer<BlackMagePlayer>();
				blm.SetElementalStack(BlackMagePlayer.MaxElementStacks);
			},
		};
		DisplayName.SetDefault(Data["fire2"].SpellName);
		Data["fire2"].Description =
			$"Deals fire damage with a potency of {Data["fire2"].Potency} to target and all enemies nearby it.\n" +
			"Additional Effect: Grants [Astral Fire III] and removes [Umbral Ice]\n" +
			$"Duration: {BlackMagePlayer.ElementalChargeSeconds}s";
		base.SetStaticDefaults();
	}
}

internal class Fire3 : Spell
{
	protected override bool AoE => false;

	public override void SetStaticDefaults()
	{
		Data["fire3"] = new SpellData
		{
			SpellName      = "Firaga",
			ProjectileID   = Projectile.type,
			Potency        = 260,
			MPCost         = 2000,
			CastTime       = 210,
			Cooldown       = 0,
			GlobalCooldown = true,
			Element        = Constants.Elements.FireElement,
			StackRequired  = false,
			LevelLearned   = 35,
			OnCastEffect = player =>
			{
				var blm = player.GetModPlayer<BlackMagePlayer>();
				if (blm.Firestarter)
					player.ClearBuff(ModContent.BuffType<Firestarter>());
				blm.SetElementalStack(BlackMagePlayer.MaxElementStacks);
			},
		};
		DisplayName.SetDefault(Data["fire3"].SpellName);
		Data["fire3"].Description =
			$"Deals fire damage with a potency of {Data["fire3"].Potency}.\n" +
			"Additional Effect: Grants [Astral Fire III] and removes [Umbral Ice]\n" +
			$"Duration: {BlackMagePlayer.ElementalChargeSeconds}s";
		base.SetStaticDefaults();
	}
}

internal class Blizzard3 : Spell
{
	protected override bool AoE => false;

	public override void SetStaticDefaults()
	{
		Data["blizzard3"] = new SpellData
		{
			SpellName      = "Blizzaga",
			ProjectileID   = Projectile.type,
			Potency        = 260,
			MPCost         = 800,
			CastTime       = 210,
			Cooldown       = 0,
			GlobalCooldown = true,
			Element        = Constants.Elements.IceElement,
			StackRequired  = false,
			LevelLearned   = 35,
			OnCastEffect = player =>
			{
				var blm = player.GetModPlayer<BlackMagePlayer>();
				blm.SetElementalStack(-BlackMagePlayer.MaxElementStacks);
			},
		};
		DisplayName.SetDefault(Data["blizzard3"].SpellName);
		Data["blizzard3"].Description =
			$"Deals ice damage with a potency of {Data["blizzard3"].Potency}.\n" +
			"Additional Effect: Grants [Umbral Ice III] and removes [Astral Fire]\n" +
			$"Duration: {BlackMagePlayer.ElementalChargeSeconds}s";
		base.SetStaticDefaults();
	}
}

internal class Freeze : Spell
{
	protected override bool AoE => true;

	public override void SetStaticDefaults()
	{
		Data["freeze"] = new SpellData
		{
			SpellName      = "Freeze",
			ProjectileID   = Projectile.type,
			Potency        = 120,
			MPCost         = 1000,
			CastTime       = 168,
			Cooldown       = 0,
			GlobalCooldown = true,
			Element        = Constants.Elements.IceElement,
			StackRequired  = true,
			LevelLearned   = 40,
			OnCastEffect = player =>
			{
				var blm = player.GetModPlayer<BlackMagePlayer>();
				blm.UmbralHearts = BlackMagePlayer.MaxUmbralHearts;
			},
		};
		DisplayName.SetDefault(Data["freeze"].SpellName);
		Data["freeze"].Description =
			$"Deals ice damage with a potency of {Data["freeze"].Potency} to target and all enemies nearby it.\n" +
			$"Additional Effect: Grants {BlackMagePlayer.MaxUmbralHearts} [Umbral Hearts]\n" +
			"[Umbral Heart] Bonus: Nullifies [Astral Fire]'s [MP] cost increase for [Fire] spells and reduces [MP] cost for [Flare] by one-third\n" +
			"Can only be executed while under the effect of [Umbral Ice].";
		base.SetStaticDefaults();
	}
}

internal class Flare : Spell
{
	protected override bool AoE => true;

	public override void SetStaticDefaults()
	{
		Data["flare"] = new SpellData
		{
			SpellName      = "Flare",
			ProjectileID   = Projectile.type,
			Potency        = 280,
			MPCost         = -2,
			CastTime       = 240,
			Cooldown       = 0,
			GlobalCooldown = true,
			Element        = Constants.Elements.FireElement,
			StackRequired  = true,
			LevelLearned   = 50,
			OnCastEffect = player =>
			{
				var blm = player.GetModPlayer<BlackMagePlayer>();
				blm.SetElementalStack(BlackMagePlayer.MaxElementStacks);
				blm.UmbralHearts = 0;
			},
		};
		DisplayName.SetDefault(Data["flare"].SpellName);
		Data["flare"].Description =
			$"Deals fire damage with a potency of {Data["flare"].Potency} to target and all enemies nearby it.\n" +
			"Additional Effect: Grants [Astral Fire III]\n" +
			$"Duration: {BlackMagePlayer.ElementalChargeSeconds}s\n" +
			"Can only be executed while under the effect of [Astral Fire].";
		base.SetStaticDefaults();
	}
}

internal class Blizzard4 : Spell
{
	protected override bool AoE => false;

	public override void SetStaticDefaults()
	{
		Data["blizzard4"] = new SpellData
		{
			SpellName      = "Blizzaja",
			ProjectileID   = Projectile.type,
			Potency        = 310,
			MPCost         = 800,
			CastTime       = 150,
			Cooldown       = 0,
			GlobalCooldown = true,
			Element        = Constants.Elements.IceElement,
			StackRequired  = true,
			LevelLearned   = 58,
			OnCastEffect = player =>
			{
				var blm = player.GetModPlayer<BlackMagePlayer>();
				blm.UmbralHearts = BlackMagePlayer.MaxUmbralHearts;
			},
		};
		DisplayName.SetDefault(Data["blizzard4"].SpellName);
		Data["blizzard4"].Description =
			$"Deals ice damage with a potency of {Data["blizzard4"].Potency}.\n" +
			$"Additional Effect: Grants {BlackMagePlayer.MaxUmbralHearts} [Umbral Hearts]\n" +
			"[Umbral Heart] Bonus: Nullifies [Astral Fire]'s [MP] cost increase for [Fire] spells and reduces [MP] cost for [Flare] by one-third\n" +
			"Can only be executed while under the effect of [Umbral Ice].";
		base.SetStaticDefaults();
	}
}

internal class Fire4 : Spell
{
	protected override bool AoE => false;

	public override void SetStaticDefaults()
	{
		Data["fire4"] = new SpellData
		{
			SpellName      = "Firaja",
			ProjectileID   = Projectile.type,
			Potency        = 310,
			MPCost         = 800,
			CastTime       = 168,
			Cooldown       = 0,
			GlobalCooldown = true,
			Element        = Constants.Elements.FireElement,
			StackRequired  = true,
			LevelLearned   = 60,
			OnCastEffect = player =>
			{
				var blm = player.GetModPlayer<BlackMagePlayer>();
			},
		};
		DisplayName.SetDefault(Data["fire4"].SpellName);
		Data["fire4"].Description =
			$"Deals fire damage with a potency of {Data["fire4"].Potency}.\n" +
			"Can only be executed while under the effect of [Astral Fire].";
		base.SetStaticDefaults();
	}
}

internal class Foul : Spell
{
	protected override bool AoE => true;

	public override void SetStaticDefaults()
	{
		Data["foul"] = new SpellData
		{
			SpellName      = "Foul",
			ProjectileID   = Projectile.type,
			Potency        = 560,
			MPCost         = 0,
			CastTime       = 0,
			Cooldown       = 0,
			GlobalCooldown = true,
			Element        = Constants.Elements.PolyglotElement,
			StackRequired  = false,
			LevelLearned   = 70,
			OnCastEffect = player =>
			{
				var blm = player.GetModPlayer<BlackMagePlayer>();
			},
		};
		DisplayName.SetDefault(Data["foul"].SpellName);
		Data["foul"].Description =
			$"Deals unaspected damage with a potency of {Data["foul"].Potency} to target and all enemies nearby it.\n" +
			"[Polyglot] Cost: 1";
		base.SetStaticDefaults();
	}
}

internal class Despair : Spell
{
	protected override bool AoE => false;

	public override void SetStaticDefaults()
	{
		Data["despair"] = new SpellData
		{
			SpellName      = "Despair",
			ProjectileID   = Projectile.type,
			Potency        = 340,
			MPCost         = -1,
			CastTime       = 180,
			Cooldown       = 0,
			GlobalCooldown = true,
			Element        = Constants.Elements.FireElement,
			StackRequired  = true,
			LevelLearned   = 72,
			OnCastEffect = player =>
			{
				var blm = player.GetModPlayer<BlackMagePlayer>();
				blm.SetElementalStack(BlackMagePlayer.MaxElementStacks);
			},
		};
		DisplayName.SetDefault(Data["despair"].SpellName);
		Data["despair"].Description =
			$"Deals fire damage with a potency of {Data["despair"].Potency}.\n" +
			"Additional Effect: Grants [Astral Fire III]\n" +
			$"Duration: {BlackMagePlayer.ElementalChargeSeconds}s\n" +
			"Can only be executed while under the effect of [Astral Fire].";
		base.SetStaticDefaults();
	}
}

internal class UmbralSoul : Spell
{
	protected override bool AoE => false;

	public override void SetStaticDefaults()
	{
		Data["umbral_soul"] = new SpellData
		{
			SpellName      = "Umbral Soul",
			ProjectileID   = Projectile.type,
			Potency        = 0,
			MPCost         = 0,
			CastTime       = 0,
			Cooldown       = 150,
			GlobalCooldown = false,
			Element        = Constants.Elements.IceElement,
			StackRequired  = true,
			LevelLearned   = 76,
			Description = "Grants [Umbral Ice] and 1 [Umbral Heart].\n" +
			              "[Umbral Heart] Bonus: Nullifies [Astral Fire]'s [MP] cost increase for [Fire] spells and reduces [MP] cost for [Flare] by one-third\n" +
			              "Can only be executed while under the effect of [Umbral Ice].",
			OnCastEffect = player =>
			{
				var blm = player.GetModPlayer<BlackMagePlayer>();
				blm.AddElementalStack(-1);
				blm.UmbralHearts++;
			},
		};
		DisplayName.SetDefault(Data["umbral_soul"].SpellName);
		base.SetStaticDefaults();
	}

	public override void AI()
	{
		Projectile.Kill();
	}
}

internal class Xenoglossy : Spell
{
	protected override bool AoE => false;

	public override void SetStaticDefaults()
	{
		Data["xenoglossy"] = new SpellData
		{
			SpellName      = "Xenoglossy",
			ProjectileID   = Projectile.type,
			Potency        = 760,
			MPCost         = 0,
			CastTime       = 0,
			Cooldown       = 0,
			GlobalCooldown = true,
			Element        = Constants.Elements.PolyglotElement,
			StackRequired  = false,
			LevelLearned   = 80,
			OnCastEffect = player =>
			{
				var blm = player.GetModPlayer<BlackMagePlayer>();
			},
		};
		DisplayName.SetDefault(Data["xenoglossy"].SpellName);
		Data["xenoglossy"].Description =
			$"Deals unaspected damage with a potency of {Data["xenoglossy"].Potency}.\n" +
			"[Polyglot] Cost: 1";
		base.SetStaticDefaults();
	}
}

internal class Paradox : Spell
{
	protected override bool AoE => false;

	public override void SetStaticDefaults()
	{
		Data["paradox"] = new SpellData
		{
			SpellName      = "Paradox",
			ProjectileID   = Projectile.type,
			Potency        = 500,
			MPCost         = 1600,
			CastTime       = 150,
			Cooldown       = 0,
			GlobalCooldown = true,
			Element        = Constants.Elements.ParadoxElement,
			StackRequired  = false,
			LevelLearned   = 90,
			OnCastEffect = player =>
			{
				var blm = player.GetModPlayer<BlackMagePlayer>();
				blm.ElementalChargeTimer = BlackMagePlayer.ElementalChargeMaxTime;

				if (blm.AstralFire == 0 || blm.SoulCrystalLevel < 42 || Main.rand.Next(0, 9) >= 4)
					return;

				player.AddBuff(ModContent.BuffType<Firestarter>(), 1800);
			},
		};
		DisplayName.SetDefault(Data["paradox"].SpellName);
		Data["paradox"].Description =
			$"Deals unaspected damage with a potency of {Data["paradox"].Potency}.\n" +
			"[Astral Fire] Bonus: Refreshes the duration of [Astral Fire]\n" +
			$"Duration: {BlackMagePlayer.ElementalChargeSeconds}s\n" +
			"[Umbral Ice] Bonus: Spell is cast immediately, requires no [MP] to cast, and refreshes the duration of [Umbral Ice]\n" +
			$"Duration: {BlackMagePlayer.ElementalChargeSeconds}s\n" +
			"Can only be executed while under the effect of [Paradox].";
		base.SetStaticDefaults();
	}
}
