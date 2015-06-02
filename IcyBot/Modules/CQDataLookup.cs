using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Meebey.SmartIrc4net;

namespace IcyBot.Modules
{
	public class CQDataLookup : IrcPlugin
	{
		public override void Initialize()
		{
			Commands.ChatCommands.Add(new Command(Hero, "hero"));
			Commands.ChatCommands.Add(new Command(Bread, "bread"));
			Commands.ChatCommands.Add(new Command(Passive, "passive"));
			Commands.ChatCommands.Add(new Command(Skill, "skill"));
			Commands.ChatCommands.Add(new Command(Weapon, "weapon"));
			Commands.ChatCommands.Add(new Command(Goddess, "goddess"));
			Commands.ChatCommands.Add(new Command(Random, "random"));
			Commands.ChatCommands.Add(new Command(Stage, "stage"));
		}

		private void Stage(CommandArgs args)
		{
			if (args.Parameters.Count == 0)
			{
				args.Args.Data.SendErrorText("stage <name>");
				return;
			}
			Stages _info;
			string ReturnString = GetStage(string.Join(" ", args.Parameters), out _info);
			if (ReturnString == "")
			{
				args.Args.Data.SendText(_info.ToString());
				args.Args.Data.SendText(_info.ToString2());
			}
			else
			{
				args.Args.Data.SendErrorText(ReturnString);
			}
		}

		private Locales GetStageLocales(string Name)
		{
			Locales _StageNameTxt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.EnUs != null && (x.EnUs.ToLower() == Name.ToLower()));
			if (_StageNameTxt == null)
			{
				_StageNameTxt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.Id != null && (x.Id.ToLower() == Name.ToLower()));
			}
			if (_StageNameTxt == null)
			{
				List<Locales> _CheckList = JsonPhraser.TextLocale.locale.Where(x => x.EnUs != null && x.EnUs.ToLower().Contains(Name.ToLower())).ToList();
				bool found = false;
				foreach (Locales _loc in _CheckList)
				{
					if (JsonPhraser.StageBase.Stage.Count(x => x.Name == _loc.Id) >= 1)
					{
						_StageNameTxt = _loc;
						found = true;
						break;
					}
				}
				if (!found)
					return null;
			}
			return _StageNameTxt;
		}

		private string GetStage(string Name, out Stages _Info)
		{
			Name = Name.TrimStart(' ');
			Locales _StageNameTxt = GetStageLocales(Name);
			_Info = null;

			if (_StageNameTxt == null)
			{
				return string.Format("Could not find Stage named: {0}!", Name);
			}

			Stage _Stage = JsonPhraser.StageBase.Stage.FirstOrDefault(x => x.Name == _StageNameTxt.Id);
			if (_Stage == null)
			{
				return string.Format("Could not find Stage ID: {0}!", _StageNameTxt.Id);
			}

			List<string> _ConditionTxt = new List<string>();
			if (_Stage.Restrictcondition != null)
			{
				if (_Stage.Restrictcondition.ClassCondition != null)
				{
					_ConditionTxt.AddRange(_Stage.Restrictcondition.ClassCondition);
				}
				if (_Stage.Restrictcondition.GradeCondition != null)
				{
					_ConditionTxt.AddRange(_Stage.Restrictcondition.GradeCondition.Select(x => x.Type + " " + x.Value.ToString()));
				}
			}

			List<string> _WeaponTxt = new List<string>();

			foreach (string _Weapon in _Stage.DropableWeaponType.Classids)
			{
				WeaponCategory _WeaponCat = JsonPhraser.WeaponCategoryBase.WeaponCategory.FirstOrDefault(x => x.Id == _Weapon);
				if (_WeaponCat == null)
				{
					return string.Format("Could not find Weapon Cat: {0}!", _Weapon);
				}

				Locales _WeaponCatName = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.Id == _WeaponCat.Name);
				if (_WeaponCat == null)
				{
					return string.Format("Could not find Weapon Cat ID: {0}!", _WeaponCat.Name);
				}

				_WeaponTxt.Add(_WeaponCatName.EnUs);
			}

			_Info = new Stages();
			_Info.Bosses = new List<Bosses>();
			if (_Stage.Bosses != null)
			{
				string _String = _Stage.Bosses.ToString();
				IEnumerable<object> _Bosses = (IEnumerable<object>)JsonConvert.DeserializeObject(_String);
				foreach (object _obj in _Bosses)
				{
					Bosses _bosses = new Bosses();
					_String = _obj.ToString();
					string _key = _String.Substring(0, _String.IndexOf(":")).Trim('"');
					_String = _String.Substring(_String.IndexOf("["));
					_bosses.BossInfo = JsonConvert.DeserializeObject<BossInfo[]>(_String);

					var _Stat = JsonPhraser.CharacterBase.Character.FirstOrDefault(x => x.Id == _key);

					if (_Stat == null)
					{
						continue;
					}

					var _name = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.Id == _Stat.Name);

					if (_name == null)
					{
						continue;
					}

					_bosses.BossName = _name.EnUs;
					_Info.Bosses.Add(_bosses);
				}
			}

			_Info.Condition = string.Join(", ", _ConditionTxt);
			_Info.MaxBreadStars = _Stage.DropableBreadType.MaxGrade;
			_Info.MinBreadStars = _Stage.DropableBreadType.MinGrade;
			_Info.MeatNeeded = _Stage.Heartneed;
			_Info.MaxWeaponStars = _Stage.DropableWeaponType.MaxGrade;
			_Info.MinWeaponStars = _Stage.DropableWeaponType.MinGrade;
			_Info.Name = _StageNameTxt.EnUs;
			_Info.Popo = _Stage.Gypsiappear;
			_Info.RewardType = _Stage.Clearrewardtype;
			_Info.RewardValue = _Stage.Clearrewardvalue;
			_Info.FirstRewardType = _Stage.Firstclearrewardtype;
			_Info.FirstRewardValue = _Stage.Firstclearrewardvalue;
			_Info.WeaponTypes = string.Join(", ", _WeaponTxt);

			return string.Empty;
		}

		private void Random(CommandArgs args)
		{
			string argString = string.Empty;
			if (args.Parameters.Count > 0)
			{
				argString = string.Join(" ", args.Parameters);
			}
			args.Args.Data.SendText(GetRandom(argString));
		}

		private string GetRandom(string argString)
		{
			argString = argString.TrimEnd(' ');
			List<string> param = argString.Split(' ').ToList();
			int argIndex = 0;
			bool textSearch = false;
			if (param.Count() > 0)
			{
				if (!int.TryParse(param[0], out argIndex))
				{
					textSearch = true;
				}
			}

			List<string> str = JsonPhraser.TextLocale.locale.Where(x => !string.IsNullOrEmpty(x.EnUs) && x.EnUs.Count(y => y == ' ') > 2).Select(x => x.EnUs).ToList();
			if (param.Count() == 0)
			{
				return str[IcyBot.Rand.Next(0, str.Count)];
			}
			else if (!textSearch)
			{
				if (argIndex > str.Count - 1 || argIndex < 0)
				{
					return string.Format("Text line number {0}, does not exist!", argIndex);
				}
				return str[argIndex].Replace(@"\n", "");
			}
			else
			{
				str = JsonPhraser.TextLocale.locale.Where(x => !string.IsNullOrEmpty(x.EnUs) && x.EnUs.ToLower().Contains(argString.ToLower())).Select(x => x.EnUs).ToList();
				if (str == null || str.Count == 0)
				{
					return string.Format("No Matches for {0} was found!", argString);
				}
				string currStr = str[IcyBot.Rand.Next(0, str.Count)];
				int start = currStr.IndexOf(argString, StringComparison.CurrentCultureIgnoreCase);
				string value = currStr.Substring(0, start) + ControlCode.Bold + currStr.Substring(start, argString.Length) + ControlCode.Bold + currStr.Substring(start + argString.Length);
				return value;
			}
		}

		private void Goddess(CommandArgs args)
		{
			if (args.Parameters.Count == 0)
			{
				args.Args.Data.SendErrorText("goddess <name>");
				return;
			}
			Goddesses _info;
			string ReturnString = GetGoddess(string.Join(" ", args.Parameters), out _info);
			if (ReturnString == "")
			{
				args.Args.Data.SendText(_info.ToString());
			}
			else
			{
				args.Args.Data.SendErrorText(ReturnString);
			}
		}

		private void Weapon(CommandArgs args)
		{
			if (args.Parameters.Count == 0)
			{
				args.Args.Data.SendErrorText("weapon <name>");
				return;
			}
			Weapons _Weapons;
			string ReturnString = "";
			if (args.Parameters[0] != "-k")
			{
				ReturnString = GetWeapons(string.Join(" ", args.Parameters), out _Weapons);
			}
			else
			{
				ReturnString = GetWeapons(string.Join(" ", args.Parameters.Skip(1)), out _Weapons, true);
			}
			if (ReturnString == "")
			{
				args.Args.Data.SendText(_Weapons.ToString());
				args.Args.Data.SendText(_Weapons.ToString2());

				if (_Weapons.RequiredHero != "")
				{
					args.Args.Data.SendText(_Weapons.ToString3());
				}
			}
			else
			{
				args.Args.Data.SendErrorText(ReturnString);
			}
		}

		private void Skill(CommandArgs args)
		{
			if (args.Parameters.Count == 0)
			{
				args.Args.Data.SendErrorText("skill <name>");
				return;
			}
			Passives _info;
			string ReturnString = GetPassive(string.Join(" ", args.Parameters), out _info);
			if (ReturnString == "")
			{
				args.Args.Data.SendText(_info.ToString());
			}
			else
			{
				args.Args.Data.SendErrorText(ReturnString);
			}
		}

		private void Passive(CommandArgs args)
		{
			if (args.Parameters.Count == 0)
			{
				args.Args.Data.SendErrorText("passive <name>");
				return;
			}
			Information _info;
			string ReturnString = GetSkill(string.Join(" ", args.Parameters), out _info);
			if (ReturnString == "")
			{
				args.Args.Data.SendText(_info.Skill.ToString());
			}
			else
			{
				args.Args.Data.SendErrorText(ReturnString);
			}
		}

		private void Bread(CommandArgs args)
		{
			if (args.Parameters.Count == 0)
			{
				args.Args.Data.SendErrorText("bread <name>");
				return;
			}

			Breads _info;
			string ReturnString = GetBread(string.Join(" ", args.Parameters), out _info);
			if (ReturnString == "")
			{
				args.Args.Data.SendText(_info.ToString());
			}
			else
			{
				args.Args.Data.SendErrorText(ReturnString);
			}
		}

		private void Hero(CommandArgs args)
		{
			if (args.Parameters.Count == 0)
			{
				args.Args.Data.SendErrorText("hero <name>");
				return;
			}
			Information _info;
			string ReturnString = GetCharacter(string.Join(" ", args.Parameters), out _info);
			if (ReturnString == "")
			{
				args.Args.Data.SendText(_info.ToString());
				args.Args.Data.SendText(_info.Stats.ToString());
			}
			else
			{
				args.Args.Data.SendErrorText(ReturnString);
			}
		}

		private Locales GetBreadLocales(string Name)
		{
			Locales _BreadNameTxt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.EnUs != null && (x.EnUs.ToLower() == Name.ToLower()));
			if (_BreadNameTxt == null)
			{
				List<Locales> _CheckList = JsonPhraser.TextLocale.locale.Where(x => x.EnUs != null && x.EnUs.ToLower().Contains(Name.ToLower())).ToList();
				bool found = false;
				foreach (Locales _loc in _CheckList)
				{
					if (JsonPhraser.BreadBase.Bread.Count(x => x.Name == _loc.Id) >= 1)
					{
						_BreadNameTxt = _loc;
						found = true;
						break;
					}
				}
				if (!found)
					return null;
			}
			return _BreadNameTxt;
		}

		private string GetBread(string Name, out Breads _Info)
		{
			Name = Name.TrimStart(' ');
			Locales _BreadNameTxt = GetBreadLocales(Name);
			_Info = null;

			if (_BreadNameTxt == null)
			{
				return string.Format("Could not find Bread named: {0}!", Name);
			}

			Bread _Bread = JsonPhraser.BreadBase.Bread.FirstOrDefault(x => x.Name == _BreadNameTxt.Id);
			if (_Bread == null)
			{
				return string.Format("Could not find Bread ID: {0}!", _BreadNameTxt.Id);
			}

			_Info = new Breads();
			_Info.Stars = _Bread.Grade;
			_Info.Name = _BreadNameTxt.EnUs;
			_Info.SellPrice = _Bread.Sellprice;
			_Info.TrainPoint = _Bread.Trainpoint;
			_Info.Class = _Bread.Class;
			_Info.GreatChance = _Bread.Critprob;

			return string.Empty;
		}

		private Locales GetGoddessLocales(string Name)
		{
			Locales _GoddessNameTxt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.EnUs != null && (x.EnUs.ToLower() == Name.ToLower()));
			if (_GoddessNameTxt == null)
			{
				List<Locales> _CheckList = JsonPhraser.TextLocale.locale.Where(x => x.EnUs != null && x.EnUs.ToLower().Contains(Name.ToLower())).ToList();
				bool found = false;
				foreach (Locales _loc in _CheckList)
				{
					if (JsonPhraser.GoddessBase.Sister.Count(x => x.Name == _loc.Id) >= 1)
					{
						_GoddessNameTxt = _loc;
						found = true;
						break;
					}
				}
				if (!found)
					return null;
			}
			return _GoddessNameTxt;
		}

		private string GetGoddess(string Name, out Goddesses _Info)
		{
			Name = Name.TrimStart(' ');
			Locales _GoddessNameTxt = GetGoddessLocales(Name);
			_Info = null;

			if (_GoddessNameTxt == null)
			{
				return string.Format("Could not find goddess named: {0}!", Name);
			}

			Sister _Goddess = JsonPhraser.GoddessBase.Sister.FirstOrDefault(x => x.Name == _GoddessNameTxt.Id);
			if (_Goddess == null)
			{
				return string.Format("No Goddess found for Goddess ID: {0}", _GoddessNameTxt.Id);
			}

			Locales _GoddessSkillTxt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.Id == _Goddess.Skillname);
			if (_GoddessSkillTxt == null)
			{
				return string.Format("No Goddess Skill found for Goddess Skill ID: {0}", _Goddess.Skillname);
			}

			Locales _GoddessSkillDescTxt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.Id == _Goddess.Skilldesc);
			if (_GoddessSkillDescTxt == null)
			{
				return string.Format("No Goddess Skill Description found for Goddess Skill Description ID: {0}", _Goddess.Skilldesc);
			}

			_Info = new Goddesses();

			_Info.Name = _GoddessNameTxt.EnUs;
			_Info.SkillName = _GoddessSkillTxt.EnUs;
			_Info.SkillDescription = _GoddessSkillDescTxt.EnUs;
			_Info.SkillDuration = _Goddess.Bgduration;

			return string.Empty;
		}

		//this is actually skills
		private Locales GetPassiveLocales(string Name)
		{
			Locales _PassiveNameTxt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.EnUs != null && (x.EnUs.ToLower() == Name.ToLower()));
			if (_PassiveNameTxt == null)
			{
				List<Locales> _CheckList = JsonPhraser.TextLocale.locale.Where(x => x.EnUs != null && x.EnUs.ToLower().Contains(Name.ToLower())).ToList();
				bool found = false;
				foreach (Locales _loc in _CheckList)
				{
					if (JsonPhraser.PassiveBase.Passive.Count(x => x.Name == _loc.Id) >= 1)
					{
						_PassiveNameTxt = _loc;
						found = true;
						break;
					}
				}
				if (!found)
					return null;
			}
			return _PassiveNameTxt;
		}

		private string GetPassive(string Name, out Passives _Info)
		{
			Name = Name.TrimStart(' ');
			string[] Params = Name.Split(' ');
			int level = 1;
			int.TryParse(Params[Params.Count() - 1], out level);
			if (level > 5 || level < 1)
			{
				level = 1;
			}
			else
			{
				Name = Name.Substring(0, Name.LastIndexOf(' ')).Trim();
			}
			Locales _PassiveNameTxt = GetPassiveLocales(Name);
			_Info = null;

			if (_PassiveNameTxt == null)
			{
				return string.Format("Could not find Skill named: {0}!", Name);
			}

			Passive _Passive = JsonPhraser.PassiveBase.Passive.FirstOrDefault(x => x.Name == _PassiveNameTxt.Id && x.Level == level);
			if (_Passive == null)
			{
				return string.Format("Could not find Skill ID: {0} lvl.{1}!", _PassiveNameTxt.Id, level);
			}

			Locales _PassiveDescTxt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.Id == _Passive.Desc);
			if (_PassiveDescTxt == null)
			{
				return string.Format("Could not find Skill Description ID: {0}!", _Passive.Desc);
			}

			Character _Character = null;
			CharacterStat _CharacterStat = null;
			Locales _PassiveUnlockHeroTxt = null;

			if (_Passive.Unlockcond != null)
			{
				switch (_Passive.Unlockcond.Type)
				{
					case "SPECIFIC":
						_Character = JsonPhraser.CharacterBase.Character.FirstOrDefault(x => x.Id == _Passive.Unlockcond.TypeTarget);
						_CharacterStat = JsonPhraser.CharacterStatBase.CharacterStat.FirstOrDefault(x => x.ID == _Character.DefaultStatId);
						_PassiveUnlockHeroTxt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.Id == _Character.Name);
						break;
				}
			}
			_Info = new Passives();
			_Info.Class = _Passive.Class;
			_Info.GreatChance = _Passive.Huge;
			_Info.HonorCost = _Passive.Cost[0].Value;
			_Info.GoldCost = _Passive.Cost[1].Value;
			_Info.Description = _PassiveDescTxt.EnUs;
			_Info.Name = _PassiveNameTxt.EnUs;
			if (_CharacterStat != null)
			{
				_Info.HeroStars = _CharacterStat.Grade;
				_Info.HeroRequired = _PassiveUnlockHeroTxt.EnUs;
			}
			_Info.Level = _Passive.Level;
			_Info.UpgradeType = _Passive.Unlockcond.Type;
			_Info.Type = _Passive.Type;
			_Info.DamageType = _Passive.Property;

			return string.Empty;
		}

		private Locales GetCharacterLocales(string Name)
		{
			Locales _CharNameTxt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.EnUs != null && (x.EnUs.ToLower() == Name.ToLower()));
			if (_CharNameTxt == null)
			{
				List<Locales> _CheckList = JsonPhraser.TextLocale.locale.Where(x => x.EnUs != null && x.EnUs.ToLower().Contains(Name.ToLower())).ToList();
				bool found = false;
				foreach (Locales _loc in _CheckList)
				{
					if (JsonPhraser.CharacterBase.Character.Count(x => x.Name == _loc.Id) >= 1)
					{
						_CharNameTxt = _loc;
						found = true;
						break;
					}
				}
				if (!found)
					return null;
			}
			return _CharNameTxt;
		}

		private string GetSkill(string Name, out Information _Info)
		{
			Name = Name.TrimStart(' ');
			Locales _CharNameTxt = GetCharacterLocales(Name);
			_Info = null;

			if (_CharNameTxt == null)
			{
				return string.Format("Could not find character named: {0}!", Name);
			}

			//gets character id data
			Character _Char = JsonPhraser.CharacterBase.Character.FirstOrDefault(x => x.Name == _CharNameTxt.Id);
			if (_Char == null)
			{
				return string.Format("No Characters found for Character ID: {0}", _CharNameTxt.Id);
			}

			//gets character stats
			CharacterStat _CharStat = JsonPhraser.CharacterStatBase.CharacterStat.FirstOrDefault(x => x.ID == _Char.DefaultStatId);
			if (_CharStat == null)
			{
				return string.Format("No Characters found for Character Stat ID: {0}", _Char.DefaultStatId);
			}
			SkillCondition _SkillID = _CharStat.Skillconditionjson.FirstOrDefault(x => x.ABHEROSKILL != null);
			if (_CharStat.Skillconditionjson == null || _SkillID == null)
			{
				return string.Format("This character does not have any skills!");
			}

			SkillDef _CharSkill = JsonPhraser.SkillBase.Skill.FirstOrDefault(x => x.Id == _SkillID.ABHEROSKILL);
			if (_CharSkill == null)
			{
				return string.Format("No Skills found for Skill ID: {0}", _SkillID.ABHEROSKILL);
			}

			Locales _SkillNameTxt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.Id == _CharSkill.Name);
			if (_SkillNameTxt == null)
			{
				return string.Format("No Skill Name found for Skill Name ID: {0}", _CharSkill.Name);
			}
			Locales _SkillDescTxt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.Id == _CharSkill.Desc);
			if (_SkillDescTxt == null)
			{
				return string.Format("No Skill Description found for Skill Description ID: {0}", _CharSkill.Desc);
			}
			Locales _SkillSubDescTxt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.Id == _CharSkill.Subdesc);
			if (_SkillDescTxt == null)
			{
				return string.Format("No Skill Description found for Skill Description ID: {0}", _CharSkill.Subdesc);
			}

			_Info = new Information();
			_Info.Skill.Name = _SkillNameTxt.EnUs;
			_Info.Skill.Description = _SkillDescTxt.EnUs;
			_Info.Skill.SubDescription = _SkillSubDescTxt.EnUs;
			_Info.Skill.CastTime = _CharSkill.Anidurationlimitsec;
			_Info.Skill.Type = _CharSkill.Type;
			return string.Empty;
		}

		private string GetCharacter(string Name, out Information _Info)
		{
			Name = Name.TrimStart(' ');
			_Info = null;

			//gets character name
			Locales _CharNameTxt = GetCharacterLocales(Name);
			if (_CharNameTxt == null)
			{
				return string.Format("Could not find character named: {0}!", Name);
			}

			//gets character id data
			Character _Char = JsonPhraser.CharacterBase.Character.FirstOrDefault(x => x.Name == _CharNameTxt.Id);
			if (_Char == null)
			{
				return string.Format("No Characters found for Character ID: {0}", _CharNameTxt.Id);
			}

			//gets character description
			Locales _CharDescTxt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.Id == _Char.Desc);
			if (_CharDescTxt == null)
			{
				return string.Format("No Characters found for Character Desc ID: {0}", _Char.Desc);
			}

			//gets character stats
			CharacterStat _CharStat = JsonPhraser.CharacterStatBase.CharacterStat.FirstOrDefault(x => x.ID == _Char.DefaultStatId);
			if (_CharStat == null)
			{
				return string.Format("No Characters found for Character Stat ID: {0}", _Char.DefaultStatId);
			}

			_Info = new Information();

			//setting gender
			_Info.Gender = _Char.Gender;
			//setting name
			_Info.Name = _CharNameTxt.EnUs;
			//setting description
			_Info.Description = _CharDescTxt.EnUs;
			//setting character information stats
			_Info.Stats.AttackDamage = _CharStat.Initialattdmg;
			_Info.Stats.Defence = _CharStat.Defense;
			_Info.Stats.Resist = _CharStat.Resist;
			_Info.Stats.HP = _CharStat.Initialhp;

			_Info.Stats.CritPower = _CharStat.Critpower;
			_Info.Stats.CritChance = _CharStat.Critprob;
			_Info.Stats.KnockBackResist = _CharStat.KnockbackResistancerate;
			_Info.Stats.LifeStael = _CharStat.Vamp;

			_Info.Stats.PassiveSlots = _CharStat.Passiveslot;
			_Info.Stats.Star = _CharStat.Grade;

			_Info.Stats.GrowthDamage = _CharStat.Growthattdmg;
			_Info.Stats.GrowthDefence = _CharStat.Growthdefense;
			_Info.Stats.GrowthHP = _CharStat.Growthhp;
			_Info.Stats.GrowthResist = _CharStat.Growthresist;

			_Info.HowToGet = (_Char.Howtoget != null ? string.Join(", ", _Char.Howtoget) : "");

			return string.Empty;
		}

		private Locales GetWeaponLocales(string Name)
		{
			Locales _WeapNameTxt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.EnUs != null && (x.EnUs.ToLower() == Name.ToLower()));
			if (_WeapNameTxt == null)
			{
				List<Locales> _CheckList = JsonPhraser.TextLocale.locale.Where(x => x.EnUs != null && x.EnUs.ToLower().Contains(Name.ToLower())).ToList();
				foreach (Locales _loc in _CheckList)
				{
					if (JsonPhraser.WeaponBase.Weapon.Count(x => x.Name == _loc.Id) >= 1)
					{
						_WeapNameTxt = _loc;
						break;
					}
				}
			}
			return _WeapNameTxt;
		}

		private string GetWeapons(string Name, out Weapons weapons, bool korean = false)
		{
			weapons = null;
			Name = Name.TrimStart(' ');
			Locales _WeaponNameTxt = GetWeaponLocales(Name);
			Weapon _Weapon = null;
			if (_WeaponNameTxt == null)
			{
				_Weapon = JsonPhraser.WeaponBase.Weapon.FirstOrDefault(x => x.Id != null && (x.Id.ToLower() == Name.ToLower()));
				if (_Weapon != null)
					_WeaponNameTxt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.EnUs != null && (x.EnUs == _Weapon.Name));
			}
			else
			{
				_Weapon = JsonPhraser.WeaponBase.Weapon.FirstOrDefault(x => x.Name == _WeaponNameTxt.Id);
			}

			if (_Weapon == null)
			{
				if (_WeaponNameTxt != null)
					return string.Format("Could not find Weapon ID: {0}!", _WeaponNameTxt.EnUs);
				else
					return string.Format("Could not find Weapon: {0}!", Name);
			}

			Locales _WeaponDescTxt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.Id == _Weapon.Desc);
			//if (_WeaponDescTxt == null)
			//{
			//	return string.Format("Could not find Weapon Desc ID: {0}!", _Weapon.Desc);
			//}

			WeaponConvertCost _WeaponUpgradeCost = JsonPhraser.WeaponConvertCostBase.WeaponConvertCost.FirstOrDefault(x => x.Grade == _Weapon.Grade);
			if (_WeaponUpgradeCost == null)
			{
				return string.Format("Could not find Weapon Upgrade Grade: {0}!", _Weapon.Grade);
			}

			//check if exclusive weapon - aka legendary - doesn't matter if this is null
			WeaponConvertExclusiveOption _WeaponLegendary = null;
			Locales _WeaponLegendarySkillTxt = null;
			Locales _WeaponRequiredHeroNameTxt = null;
			WeaponGradeupCost _WeaponLegendaryUpgrade = null;
			_WeaponLegendary = JsonPhraser.WeaponConvertExclusiveOptionBase.WeaponConvertExclusiveOption.FirstOrDefault(x => x.WeaponId == _Weapon.Id);
			if (_WeaponLegendary != null)
			{
				_WeaponLegendarySkillTxt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.Id == _WeaponLegendary.DspFormat);
				_WeaponLegendaryUpgrade = JsonPhraser.WeaponGradeupCostBase.WeaponGradeupCost.FirstOrDefault(x => x.Grade == _Weapon.Grade);
				_WeaponRequiredHeroNameTxt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.Id == _WeaponLegendary.Reqheroname);
			}

			WeaponDismantleProp _WeaponIron = JsonPhraser.WeaponDismantlePropBase.WeaponDismantleProps.FirstOrDefault(x => x.Grade == _Weapon.Grade);
			if (_WeaponIron == null)
			{
				return string.Format("Could not find Weapon Dismantel Grade: {0}!", _Weapon.Grade);
			}

			WeaponSellCost _WeaponSell = JsonPhraser.WeaponSellCostBase.WeaponSellCost.FirstOrDefault(x => x.Grade == _Weapon.Grade);
			if (_WeaponSell == null)
			{
				return string.Format("Could not find Weapon Sell Grade: {0}!", _Weapon.Grade);
			}

			WeaponCategory _WeaponCategory = JsonPhraser.WeaponCategoryBase.WeaponCategory.FirstOrDefault(x => x.Id == _Weapon.Categoryid);
			if (_WeaponCategory == null)
			{
				return string.Format("Could not find Weapon Category ID: {0}!", _Weapon.Categoryid);
			}

			Locales _WeaponCategoryNameTxt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.Id == _WeaponCategory.Name);
			if (_WeaponCategoryNameTxt == null)
			{
				return string.Format("Could not find Weapon Category Name ID: {0}!", _WeaponCategory.Desc);
			}

			Locales _WeaponCategoryDescTxt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.Id == _WeaponCategory.Desc);
			if (_WeaponCategoryDescTxt == null)
			{
				return string.Format("Could not find Weapon Category Descripition ID: {0}!", _WeaponCategory.Desc);
			}

			weapons = new Weapons();

			weapons.Name = (_WeaponNameTxt == null ? "Fill me!" : _WeaponNameTxt.EnUs);
			weapons.Description = (_WeaponDescTxt != null ? _WeaponDescTxt.EnUs : "");
			weapons.AttackSpeed = _Weapon.Attspd;
			weapons.AttackDamage = _Weapon.Attdmg;
			weapons.CategoryDescription = _WeaponCategoryDescTxt.EnUs;
			weapons.CategoryName = _WeaponCategoryNameTxt.EnUs;
			weapons.Range = _Weapon.Range;
			weapons.Rarity = _Weapon.Rarity;
			weapons.SellAmount = _WeaponSell.Goldcost;
			weapons.Slot1 = _Weapon.ConvertSlot1;
			weapons.Slot2 = _Weapon.ConvertSlot2;
			weapons.Slot3 = _Weapon.ConvertSlot3;
			weapons.Stars = _Weapon.Grade;

			if (_WeaponLegendary != null)
			{
				weapons.RequiredHero = _WeaponRequiredHeroNameTxt.EnUs;
				if (_WeaponLegendarySkillTxt != null)
					if (!korean)
						weapons.SkillDesciption = (string.IsNullOrWhiteSpace(_WeaponLegendarySkillTxt.EnUs) || _WeaponLegendarySkillTxt.EnUs == "Fill me!" ? _WeaponLegendarySkillTxt.KoKr : _WeaponLegendarySkillTxt.EnUs);
					else
						weapons.SkillDesciption = (string.IsNullOrWhiteSpace(_WeaponLegendarySkillTxt.KoKr) || _WeaponLegendarySkillTxt.KoKr == "Fill me!" ? _WeaponLegendarySkillTxt.KoKr : _WeaponLegendarySkillTxt.KoKr);
				if (_WeaponLegendaryUpgrade != null)
				{
					weapons.RequiredPowder = _WeaponLegendaryUpgrade.Reqpowder;
					weapons.RequiredCrystal = _WeaponLegendaryUpgrade.Reqcrystal;
					weapons.RequiredGold = _WeaponLegendaryUpgrade.Reqgold;
					weapons.RequiredIron = _WeaponLegendaryUpgrade.Reqiron;
				}
			}

			weapons.RewardIron = _WeaponIron.Rewardiron;
			weapons.CrystalChance = _WeaponIron.RewardCrystalProb;
			weapons.PieceChance = _WeaponIron.RewardPieceProb;
			weapons.PowderChance = _WeaponIron.RewardPowderProb;

			return string.Empty;
		}
	}
}
