using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
namespace IcyBot
{
    public class Information
    {
        public override string ToString()
        {
			return string.Format("{0} | Gender: {4} | {1} Stars - {2} | How to get: {3}", Name, Stats.Star, Description, HowToGet, Gender).Replace("\n", " ");
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Passive { get; set; }
        public string PassiveDescription { get; set; }
        public string Move { get; set; }
        public string MoveDescription { get; set; }
        public bool ContractOnly { get; set; }
        public string Gender { get; set; }
        public Stats Stats { get; set; }
        public Skill Skill { get; set; }
		public Weapons Weapons { get; set; }
        public string HowToGet { get; set; }
        public Information()
        {
            Stats = new Stats();
            Skill = new Skill();
			Weapons = new Weapons();
        }
    }

    public class Skill
    {
        public override string ToString()
        {
			return string.Format("{0} - {1} - Range: {2} Cast Time: {3} | {5} - {4}", Name, Type, Range, CastTime, SubDescription, Description).Replace("\n", " ");
        }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Range { get; set; }
        public double CastTime { get; set; }
        public string Description { get; set; }
        public string SubDescription { get; set; }
    }

    public class Stats
    {
        public override string ToString()
        {
			return string.Format("BaseHP: {0} BaseAttack: {1} BaseDefence: {2} BaseResist: {3} | GrowthHP: {4} GrowthAttack: {5} GrowthDefence: {6} GrowthResist: {7} | KnockBackResist: {8} CritPower: {9}% CritChance: {10}% LifeSteal: {11}%", HP, AttackDamage, Defence, Resist, GrowthHP, GrowthDamage, GrowthDefence, GrowthResist, KnockBackResist, (CritPower * 100) + 100, CritChance * 100, LifeStael * 100).Replace("\n", " ");
        }

        public double GrowthResist { get; set; }
        public double GrowthHP { get; set; }
        public double GrowthDefence { get; set; }
        public double GrowthDamage { get; set; }

        public double KnockBackResist { get; set; }

        public double AttackDamage { get; set; }
        public double HP { get; set; }
        public double Resist { get; set; }
        public double Defence { get; set; }

        public int PassiveSlots { get; set; }

        public int Star { get; set; }
        public double CritPower { get; set; }
        public double CritChance { get; set; }
        public double LifeStael { get; set; }

		public double GetHA(int level, int train)
		{
			return CalcStat(level, AttackDamage, GrowthDamage) * (1 + (train / 10.0));
		}

		public double GetHP(int level, int train)
		{
			return CalcStat(level, HP, GrowthHP) * (1 + (train / 10.0));
		}

		public double GetDefence(int level, int train)
		{
			return CalcStat(level, Defence, GrowthDefence) * (1 + (train / 10.0));
		}

		public double GetResist(int level, int train)
		{
			return CalcStat(level, Resist, GrowthResist) * (1 + (train / 10.0));
		}

		private double CalcStat(int level, double baseStat, double growthStat)
		{
			return baseStat + (growthStat * level);
		}
    }

	public class Weapons
	{
		public override string ToString()
		{
			return string.Format("{0} {9}* - {1} | Slots: {2} {3} {4} | Damage: {5} AttackSpeed: {6} SellAmount: {7} Range: {8} Rarity: {10}", Name, (Description == null ? "No Description" : Description), Slot1, Slot2, Slot3, AttackDamage, AttackSpeed, SellAmount, Range, Stars, Rarity).Replace("\n", " ");
		}

		public string ToString2()
		{
			return string.Format("Type: {0} - {1} | Dismantel - Iron: {2} Crystal: {3}% Piece: {4}% Powder: {5}%", CategoryName, (CategoryDescription == null ? "No Description" : CategoryDescription), RewardIron, CrystalChance * 100, PieceChance * 100, PowderChance * 100).Replace("\n", " ");
		}

		public string ToString3()
		{
			if (RequiredHero != "")
			{
				return string.Format("{0} - {1} | Upgrade Materials - Iron: {2} Crystal: {3} Piece: {4} Powder: {5} Gold: {6}", RequiredHero, SkillDesciption, RequiredIron, RequiredCrystal, RequiredPiece, RequiredPowder, RequiredGold).Replace("\n", " ");
			}
			else
			{
				return "";
			}
		}

		//Base Weapon
		public string Rarity { get; set; }
		public int Stars { get; set; }
		public double Range { get; set; }
		public string Slot1 { get; set; }
		public string Slot2 { get; set; }
		public string Slot3 { get; set; }
		public int AttackDamage { get; set; }
		public double AttackSpeed { get; set; }
		public string Description { get; set; }
		public string Name { get; set; }
		public int SellAmount { get; set; }

		//Category Weapon
		public string CategoryName { get; set; }
		public string CategoryDescription { get; set; }

		//Legendary stats
		public string RequiredHero { get; set; }
		public string SkillDesciption { get; set; }
		public int RequiredPowder { get; set; }
		public int RequiredIron { get; set; }
		public int RequiredPiece { get; set; }
		public int RequiredCrystal { get; set; }
		public int RequiredGold { get; set; }

		//Dismantel Awards
		public int RewardIron { get; set; }
		public double CrystalChance { get; set; }
		public double PieceChance { get; set; }
		public double PowderChance { get; set; }
	}

	public class Passives
	{
		public override string ToString()
		{
			if (HeroRequired == null)
			{
				return string.Format("{0} Lvl.{6} - {1} | Honor: {2} Gold: {3} Great: {4}% DamageType: {8} Class: {5} Type: {7}", Name, Description, HonorCost, GoldCost, GreatChance * 100, Class, Level, Type, DamageType).Replace("\n", " ");
			}
			else
			{
				return string.Format("{0} Lvl.{6} - {1} | Honor: {2} Gold: {3} Great: {4}% DamageType: {9} Class: {5} Type: {7} Requirement: {8}", Name, Description, HonorCost, GoldCost, (GreatChance > 0 ? GreatChance * 100 : 0), Class, Level, Type, HeroRequired + " " + HeroStars + "*", DamageType).Replace("\n", " ");
			}
		}
		public double GreatChance { get; set; }
		public int HonorCost { get; set; }
		public int GoldCost { get; set; }
		public string Class { get; set; }
		public string Description { get; set; }
		public string Name { get; set; }
		public int Level { get; set; }
		public string Type { get; set; }
		public string HeroRequired { get; set; }
		public int HeroStars { get; set; }
		public string UpgradeType { get; set; }
		public string DamageType { get; set; }
	}

	public class Goddesses
	{
		public override string ToString()
		{
			return string.Format("{0} | {1} - {2} Duration: {3}s", Name, SkillName, SkillDescription, SkillDuration).Replace("\n", " ");
		}
		public string Name { get; set; }
		public string SkillName { get; set; }
		public string SkillDescription { get; set; }
		public double SkillDuration { get; set; }
	}

	public class Breads
	{
		public override string ToString()
		{
			return string.Format("{0} {5}* | Train: {1} Great: {2}% Sell: {3} Class?: {4}", Name, TrainPoint, GreatChance * 100, SellPrice, string.Join(", ", Class), Stars);
		}
		public string Name { get; set; }
		public int TrainPoint { get; set; }
		public double GreatChance { get; set; }
		public int SellPrice { get; set; }
		public int Stars { get; set; }
		public string[] Class { get; set; }
	}

	public class Stages
	{
		public override string ToString()
		{
			return string.Format("{0} | {1}-{2}* Bread | {3}-{4}* Weapon Types: {10} | Popo: {5} | FirstReward: {6}{7} Reward: {8}{9}", Name, (MinBreadStars != -1 ? MinBreadStars.ToString() : "None"), (MaxBreadStars != -1 ? MaxBreadStars.ToString() : "None"), (MinWeaponStars != -1 ? MinWeaponStars.ToString() : "None"), (MaxWeaponStars != -1 ? MaxWeaponStars.ToString() : "None"), (Popo == 1 ? "True" : "False"), (FirstRewardValue != null ? FirstRewardValue + " " : string.Empty), FirstRewardType, (RewardValue != null ? RewardValue + " " : string.Empty), RewardType, (WeaponTypes != string.Empty ? WeaponTypes : "None"));
		}
		public string ToString2()
		{
			return string.Join(" | ", Bosses);
		}
		public string Name { get; set; }
		public int MinBreadStars { get; set; }
		public int MaxBreadStars { get; set; }
		public int MeatNeeded { get; set; }
		public string Condition { get; set; }
		public double Popo { get; set; }
		public int RewardValue { get; set; }
		public string RewardType { get; set; }
		public int FirstRewardValue { get; set; }
		public string FirstRewardType { get; set; }
		public List<Bosses> Bosses { get; set; }
		public int MinWeaponStars { get; set; }
		public int MaxWeaponStars { get; set; }
		public string WeaponTypes { get; set; }
	}

	public class Bosses
	{
		public override string ToString()
		{
			return string.Format("{0} Lvl {1}", BossName, BossInfo[0].level);
		}
		public BossInfo[] BossInfo { get; set; }
		public string BossName { get; set; }
	}

	public class WeaponUpgrades
	{
		public double Chance { get; set; }
		public double Max { get; set; }
		public double Min { get; set; }
		public string Type { get; set; }
		public string SlotType { get; set; }
	}
}
