using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlackMage.Projectiles
{
	public abstract class Spell : ModProjectile
	{
		public static Dictionary<int, SpellData> Data { get; } = new Dictionary<int, SpellData>();

		public class SpellData
		{
			public string SpellName      { get; set; } = "";
			public int    Potency        { get; set; } = 0;
			public int    MPCost         { get; set; } = 0;
			public uint   CastTime       { get; set; } = 0;
			public uint   Cooldown       { get; set; } = 0;
			public bool   GlobalCooldown { get; set; } = true;
			public byte   ElementStack   { get; set; } = Elements.NoElement | Elements.NoStack;
			public bool   StackRequired  { get; set; } = false;
			public string Description    { get; set; } = "PLACEHOLDER\nYou shouldn't be seeing this.";
			public int    LevelLearned   { get; set; } = 0;
		}

		public const int SingleTargetSize = 5;
		public const int AoESize          = 80;

		protected NPC Target;

		protected virtual bool AoE => false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault(Data[projectile.type].SpellName);
			ProjectileID.Sets.Homing[projectile.type]                  = true;
			ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
		}

		public override void SetDefaults()
		{
			projectile.width                = AoE ? AoESize : SingleTargetSize;
			projectile.height               = AoE ? AoESize : SingleTargetSize;
			projectile.friendly             = true;
			projectile.magic                = true;
			projectile.knockBack            = 0f;
			projectile.tileCollide          = false;
			projectile.penetrate            = AoE ? -1 : 1;
			projectile.timeLeft             = 120;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown  = -1;
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
			{
				return;
			}

			Player  player       = Main.player[projectile.owner];
			Vector2 targetCenter = projectile.position;
			var     foundTarget  = false;

			if (player.HasMinionAttackTargetNPC)
			{
				NPC   npc     = Main.npc[player.MinionAttackTargetNPC];
				float between = Vector2.Distance(npc.Center, projectile.Center);
				if (between < 2000f)
				{
					targetCenter = npc.Center;
					foundTarget  = true;
					Target       = npc;
				}
			}

			if (!foundTarget)
			{
				projectile.Kill();
				return;
			}

			projectile.Center = targetCenter;
		}
	}

	internal class Blizzard : Spell
	{
		protected override bool AoE => false;

		public override void SetStaticDefaults()
		{
			Data[projectile.type] = new SpellData
			{
				SpellName      = "Blizzard",
				Potency        = 180,
				MPCost         = 400,
				CastTime       = 150,
				Cooldown       = 0,
				GlobalCooldown = true,
				ElementStack   = Elements.IceElement | Elements.OneStack,
				StackRequired  = false,
				LevelLearned   = 1,
			};
			Data[projectile.type].Description =
				$"Deals ice damage with a potency of {Data[projectile.type].Potency}.\n" +
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
			Data[projectile.type] = new SpellData
			{
				SpellName      = "Fire",
				Potency        = 180,
				MPCost         = 800,
				CastTime       = 150,
				Cooldown       = 0,
				GlobalCooldown = true,
				ElementStack   = Elements.FireElement | Elements.OneStack,
				StackRequired  = false,
				LevelLearned   = 2,
			};
			Data[projectile.type].Description =
				$"Deals fire damage with a potency of {Data[projectile.type].Potency}.\n" +
				"Additional Effect: Grants [Astral Fire] or removes [Umbral Ice]\n" +
				$"Duration: {BlackMagePlayer.ElementalChargeSeconds}s\n" +
				"Additional Effect: 40% chance the next [Firaga] will cost no MP and have no cast time\n" +
				"Duration: 30s";
			base.SetStaticDefaults();
		}

		public override void AI()
		{
			Player  player       = Main.player[projectile.owner];
			Vector2 targetCenter = projectile.position;
			var     foundTarget  = false;

			if (player.HasMinionAttackTargetNPC)
			{
				NPC   npc     = Main.npc[player.MinionAttackTargetNPC];
				float between = Vector2.Distance(npc.Center, projectile.Center);
				if (between < 2000f)
				{
					targetCenter = npc.Center;
					foundTarget  = true;
					Target       = npc;
				}
			}

			if (!foundTarget)
			{
				for (var i = 0; i < Main.maxNPCs; i++)
				{
					NPC npc = Main.npc[i];
					if (!npc.CanBeChasedBy()) continue;

					float npcDistance = Vector2.Distance(npc.Center, projectile.Center);
					bool  closest     = Vector2.Distance(projectile.Center, targetCenter) > npcDistance;

					if (!closest && foundTarget) continue;

					targetCenter = npc.Center;
					foundTarget  = true;
					Target       = npc;
				}
			}

			if (!foundTarget)
			{
				projectile.Kill();
				return;
			}

			const float speed   = 24f;
			const float inertia = 8f;

			if (!(Vector2.Distance(projectile.Center, targetCenter) > 40f)) return;

			Vector2 direction = targetCenter - projectile.Center;
			direction.Normalize();
			direction           *= speed;
			projectile.velocity =  (projectile.velocity * (inertia - 1) + direction) / inertia;
		}
	}

	internal class Transpose : Spell
	{
		protected override bool AoE => false;

		public override void SetStaticDefaults()
		{
			Data[projectile.type] = new SpellData
			{
				SpellName      = "Transpose",
				Potency        = 0,
				MPCost         = 0,
				CastTime       = 0,
				Cooldown       = 300,
				GlobalCooldown = false,
				ElementStack   = Elements.TransposeElement | Elements.OneStack,
				StackRequired  = false,
				Description =
					"Swaps [Astral Fire] with a single [Umbral Ice], or [Umbral Ice] with a single [Astral Fire].",
				LevelLearned = 4,
			};
			base.SetStaticDefaults();
		}

		public override void AI()
		{
			projectile.Kill();
		}
	}

	internal class Blizzard2 : Spell
	{
		protected override bool AoE => true;

		public override void SetStaticDefaults()
		{
			Data[projectile.type] = new SpellData
			{
				SpellName      = "Blizzara",
				Potency        = 100,
				MPCost         = 800,
				CastTime       = 180,
				Cooldown       = 0,
				GlobalCooldown = true,
				ElementStack   = Elements.IceElement | Elements.FullStack,
				StackRequired  = false,
				LevelLearned   = 12,
			};
			Data[projectile.type].Description =
				$"Deals ice damage with a potency of {Data[projectile.type].Potency} to target and all enemies nearby it.\n" +
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
			Data[projectile.type] = new SpellData
			{
				SpellName      = "Scathe",
				Potency        = 100,
				MPCost         = 800,
				CastTime       = 0,
				Cooldown       = 0,
				GlobalCooldown = true,
				ElementStack   = Elements.NoElement | Elements.NoStack,
				StackRequired  = false,
				LevelLearned   = 0,
			};
			Data[projectile.type].Description =
				$"Deals unaspected damage with a potency of {Data[projectile.type].Potency}.\n" +
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
			Data[projectile.type] = new SpellData
			{
				SpellName      = "Fira",
				Potency        = 100,
				MPCost         = 1500,
				CastTime       = 180,
				Cooldown       = 0,
				GlobalCooldown = true,
				ElementStack   = Elements.FireElement | Elements.FullStack,
				StackRequired  = false,
				LevelLearned   = 18,
			};
			Data[projectile.type].Description =
				$"Deals fire damage with a potency of {Data[projectile.type].Potency} to target and all enemies nearby it.\n" +
				"Additional Effect: Grants [Astral Fire III] and removes [Umbral Ice]\n" +
				$"Duration: {BlackMagePlayer.ElementalChargeSeconds}s\n" +
				"[Astral Fire] Bonus: Grants [Enhanced Flare]\n" +
				"Effect is canceled if [Astral Fire] ends.";
			base.SetStaticDefaults();
		}
	}

	internal class Fire3 : Spell
	{
		protected override bool AoE => false;

		public override void SetStaticDefaults()
		{
			Data[projectile.type] = new SpellData
			{
				SpellName      = "Firaga",
				Potency        = 260,
				MPCost         = 2000,
				CastTime       = 210,
				Cooldown       = 0,
				GlobalCooldown = true,
				ElementStack   = Elements.FireElement | Elements.FullStack,
				StackRequired  = false,
				LevelLearned   = 35,
			};
			Data[projectile.type].Description =
				$"Deals fire damage with a potency of {Data[projectile.type].Potency}.\n" +
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
			Data[projectile.type] = new SpellData
			{
				SpellName      = "Blizzaga",
				Potency        = 260,
				MPCost         = 800,
				CastTime       = 210,
				Cooldown       = 0,
				GlobalCooldown = true,
				ElementStack   = Elements.IceElement | Elements.FullStack,
				StackRequired  = false,
				LevelLearned   = 35,
			};
			Data[projectile.type].Description =
				$"Deals ice damage with a potency of {Data[projectile.type].Potency}.\n" +
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
			Data[projectile.type] = new SpellData
			{
				SpellName      = "Freeze",
				Potency        = 120,
				MPCost         = 1000,
				CastTime       = 168,
				Cooldown       = 0,
				GlobalCooldown = true,
				ElementStack   = Elements.IceElement | Elements.HeartStack,
				StackRequired  = true,
				LevelLearned   = 40,
			};
			Data[projectile.type].Description =
				$"Deals ice damage with a potency of {Data[projectile.type].Potency} to target and all enemies nearby it.\n" +
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
			Data[projectile.type] = new SpellData
			{
				SpellName      = "Flare",
				Potency        = 220,
				MPCost         = -1,
				CastTime       = 240,
				Cooldown       = 0,
				GlobalCooldown = true,
				ElementStack   = Elements.FireElement | Elements.HeartStack,
				StackRequired  = true,
				LevelLearned   = 50,
			};
			Data[projectile.type].Description =
				$"Deals fire damage with a potency of {Data[projectile.type].Potency} to target and all enemies nearby it.\n" +
				"[Enhanced Flare] Potency: 280\n" +
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
			Data[projectile.type] = new SpellData
			{
				SpellName      = "Blizzaja",
				Potency        = 310,
				MPCost         = 800,
				CastTime       = 150,
				Cooldown       = 0,
				GlobalCooldown = true,
				ElementStack   = Elements.IceElement | Elements.HeartStack,
				StackRequired  = true,
				LevelLearned   = 58,
			};
			Data[projectile.type].Description =
				$"Deals ice damage with a potency of {Data[projectile.type].Potency}.\n" +
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
			Data[projectile.type] = new SpellData
			{
				SpellName      = "Firaja",
				Potency        = 310,
				MPCost         = 800,
				CastTime       = 168,
				Cooldown       = 0,
				GlobalCooldown = true,
				ElementStack   = Elements.FireElement | Elements.NoStack,
				StackRequired  = true,
				LevelLearned   = 60,
			};
			Data[projectile.type].Description =
				$"Deals fire damage with a potency of {Data[projectile.type].Potency}.\n" +
				"Can only be executed while under the effect of [Astral Fire].";
			base.SetStaticDefaults();
		}
	}

	internal class Foul : Spell
	{
		protected override bool AoE => true;

		public override void SetStaticDefaults()
		{
			Data[projectile.type] = new SpellData
			{
				SpellName      = "Foul",
				Potency        = 560,
				MPCost         = 0,
				CastTime       = 0,
				Cooldown       = 0,
				GlobalCooldown = true,
				ElementStack   = Elements.PolyglotElement | Elements.NoStack,
				StackRequired  = false,
				LevelLearned   = 70,
			};
			Data[projectile.type].Description =
				$"Deals unaspected damage with a potency of {Data[projectile.type].Potency} to target and all enemies nearby it.\n" +
				"[Polyglot] Cost: 1";
			base.SetStaticDefaults();
		}
	}

	internal class Despair : Spell
	{
		protected override bool AoE => false;

		public override void SetStaticDefaults()
		{
			Data[projectile.type] = new SpellData
			{
				SpellName      = "Despair",
				Potency        = 340,
				MPCost         = -1,
				CastTime       = 180,
				Cooldown       = 0,
				GlobalCooldown = true,
				ElementStack   = Elements.FireElement | Elements.FullStack,
				StackRequired  = true,
				LevelLearned   = 72,
			};
			Data[projectile.type].Description =
				$"Deals fire damage with a potency of {Data[projectile.type].Potency}.\n" +
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
			Data[projectile.type] = new SpellData
			{
				SpellName      = "Umbral Soul",
				Potency        = 0,
				MPCost         = 0,
				CastTime       = 0,
				Cooldown       = 150,
				GlobalCooldown = false,
				ElementStack   = Elements.IceElement | Elements.OneStack,
				StackRequired  = true,
				LevelLearned   = 76,
				Description = "Grants [Umbral Ice] and 1 [Umbral Heart].\n" +
				              "[Umbral Heart] Bonus: Nullifies [Astral Fire]'s [MP] cost increase for [Fire] spells and reduces [MP] cost for [Flare] by one-third\n" +
				              "Can only be executed while under the effect of [Umbral Ice].",
			};
			base.SetStaticDefaults();
		}

		public override void AI()
		{
			projectile.Kill();
		}
	}

	internal class Xenoglossy : Spell
	{
		protected override bool AoE => false;

		public override void SetStaticDefaults()
		{
			Data[projectile.type] = new SpellData
			{
				SpellName      = "Xenoglossy",
				Potency        = 760,
				MPCost         = 0,
				CastTime       = 0,
				Cooldown       = 0,
				GlobalCooldown = true,
				ElementStack   = Elements.PolyglotElement | Elements.NoStack,
				StackRequired  = false,
				LevelLearned   = 80,
			};
			Data[projectile.type].Description =
				$"Deals unaspected damage with a potency of {Data[projectile.type].Potency}.\n" +
				"[Polyglot] Cost: 1";
			base.SetStaticDefaults();
		}
	}

	internal class Paradox : Spell
	{
		protected override bool AoE => false;

		public override void SetStaticDefaults()
		{
			Data[projectile.type] = new SpellData
			{
				SpellName      = "Paradox",
				Potency        = 500,
				MPCost         = 1600,
				CastTime       = 150,
				Cooldown       = 0,
				GlobalCooldown = true,
				ElementStack   = Elements.ParadoxElement | Elements.NoStack,
				StackRequired  = false,
				LevelLearned   = 90,
			};
			Data[projectile.type].Description =
				$"Deals unaspected damage with a potency of {Data[projectile.type].Potency}.\n" +
				"[Astral Fire] Bonus: Refreshes the duration of [Astral Fire]\n" +
				$"Duration: {BlackMagePlayer.ElementalChargeSeconds}s\n" +
				"[Umbral Ice] Bonus: Spell is cast immediately, requires no [MP] to cast, and refreshes the duration of [Umbral Ice]\n" +
				$"Duration: {BlackMagePlayer.ElementalChargeSeconds}s\n" +
				"Can only be executed while under the effect of [Paradox].";
			base.SetStaticDefaults();
		}
	}
}
