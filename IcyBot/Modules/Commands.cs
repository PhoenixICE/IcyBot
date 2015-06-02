/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

using ChatterBotAPI;
using System.Text.RegularExpressions;
using Meebey.SmartIrc4net;

namespace IcyBot
{
	public static class Temp
	{
		public static Dictionary<string, DateTime> RequestTime = new Dictionary<string, DateTime>();
		public static DateTime LastRequest = DateTime.MinValue;
		public static int Internval = 3;


		public static Timer TwitchCheck = new Timer(30000);
		public static List<string> CurrentStreamers = new List<string>();
		public static List<string> Streamers = new List<string>();

		public static readonly int ColoRoundsMax = 10;

		public static List<string> Animals = new List<string>();

		public static Dictionary<string, List<Information>> UserTeams = new Dictionary<string, List<Information>>();
		


		static void HandleCommands(IrcClient client, IrcEventArgs e, bool Whisper = false)
		{

			if ((Whisper || (DateTime.Now - LastRequest).TotalSeconds >= Internval) && e.PrivateMessage.Source != "#quiz")
			{
				else if (e.PrivateMessage.Message.StartsWith("$twitch "))
				{
					string var = e.PrivateMessage.Message.Substring(8).ToLower();
					if (Streamers.Contains(var))
					{
						SendChat(ErrorColor + string.Format("Already monitoring {0}!", var), e, client, Whisper);
					}
					else
					{
						Streamers.Add(var);
						string json = JsonConvert.SerializeObject(Streamers);
						System.IO.File.WriteAllText(@"D:\streamers.txt", json);
						SendChat(Color + string.Format("Succesfully added monitoring for twitch user {0}!", var), e, client, Whisper);
					}
				}
				else if (e.PrivateMessage.Message.StartsWith("$weather "))
				{
					SendChat(Color + weather.get_weather(e.PrivateMessage.Message.Substring(9)), e, client, Whisper);
				}
				else if (e.PrivateMessage.Message.StartsWith("$forecast "))
				{
					foreach (string str in weather.get_forecast(e.PrivateMessage.Message.Substring(10), 5))
					{
						SendChat(Color + str, e, client, Whisper);
					}
				}
				
				//else if (e.PrivateMessage.Message.StartsWith("$team "))
				//{
				//	MyTeam(e.PrivateMessage.Message, e.PrivateMessage.User.Nick);
				//	//SendChat(Color + MyTeam(), e, client, Whisper);
				//}
				//else if (e.PrivateMessage.Message.StartsWith(".stage ") || e.PrivateMessage.Message.StartsWith("$stage "))
				//{
				//	Stages _info;
				//	string ReturnString = GetStage(e.PrivateMessage.Message.Substring(7), out _info);
				//	if (ReturnString == "")
				//	{
				//		if (e.PrivateMessage.Message.StartsWith("."))
				//		{
				//			client.SendMessage(Color + _info.ToString(), e.PrivateMessage.User.Nick);
				//		}
				//		else
				//		{
				//			var channel = client.Channels[e.PrivateMessage.Source];
				//			channel.SendMessage(Color + _info.ToString());
				//		}
				//	}
				//	else
				//	{
				//		if (e.PrivateMessage.Message.StartsWith("."))
				//		{
				//			client.SendMessage(ErrorColor + ReturnString, e.PrivateMessage.User.Nick);
				//		}
				//		else
				//		{
				//			var channel = client.Channels[e.PrivateMessage.Source];
				//			channel.SendMessage(ErrorColor + ReturnString);
				//		}
				//	}
				//}
				LastRequest = DateTime.Now;
			}
		}

		//hero add <name>
		private static void MyTeam(string message, string nick)
		{
			List<string> param = message.Split(' ').ToList();
			param.RemoveAt(0);
			if (param.Count < 3)
			{
				return;
			}
			string caseString = param[0];

			if (caseString.ToLower().Contains("hero"))
			{
				if (param[1].ToLower() == "add")
				{
					Information _info = null;
					var hero = GetCharacter(param[2], out _info);
					if (_info == null)
					{
						return;
					}
					List<Information> _infos;
					if (!UserTeams.TryGetValue(nick, out _infos))
					{
						_infos = new List<Information>();
						_infos.Add(_info);
						Database.Query("INSERT INTO CQDB(NickName, Heros) VALUES (@0,@1)", nick, JsonConvert.SerializeObject(_infos.Select(x => x.Name)));
					}
					else
					{
						if (_infos.Count == 3)
						{
							return;
						}

						_infos.Add(_info);

						Database.Query("UPDATE CQDB SET Heros = @0 WHERE Nick = @1", JsonConvert.SerializeObject(_infos.Select(x => x.Name)), nick);
					}
				}
				else if (param[1].ToLower() == "del")
				{
					Information _info = null;
					var hero = GetCharacter(param[2], out _info);
					if (_info == null)
					{
						return;
					}
					List<Information> _infos;
					if (!UserTeams.TryGetValue(nick, out _infos))
					{
						return;
					}
					if (_infos.Count == 0)
					{
						return;
					}
					var list = _infos.Where(x => x.Name == _info.Name);
					foreach (var obj in list)
					{
						_infos.Remove(obj);
					}
				}
				else
				{
					return;
				}
			}
		}

		static void SendChat(string Message, IrcEventArgs e, IrcClient client, bool Whisper)
		{
			if (Whisper)
			{
				client.SendMessage(Message, e.PrivateMessage.User.Nick);
			}
			else
			{
				var channel = client.Channels[e.PrivateMessage.Source];
				channel.SendMessage(Message);
			}
		}

		static void OnInitialize()
		{
			//load all json files
			LoadCQConfigs();
			//Setup Quiz
			//Setup Twitch
			TwitchCheck.Elapsed += TwitchCheck_Elapsed;
			TwitchCheck.Start();
			SetupDB();
		}

		static void TwitchCheck_Elapsed(object sender, ElapsedEventArgs e)
		{
			foreach (string str in Streamers)
			{
				bool isStreaming = Twitch.Check(str);

				if (CurrentStreamers.Contains(str) && !isStreaming)
				{
					CurrentStreamers.Remove(str);
					var channel = clients[0].Channels["#general"];
					channel.SendMessage(string.Format("DEBUG [Twitch] - {0} has just stopped streaming! {1}", str, "www.twitch.tv/" + str));
				}
				else if (isStreaming && !CurrentStreamers.Contains(str))
				{
					CurrentStreamers.Add(str);

					var channel = clients[0].Channels["#general"];
					channel.SendMessage(string.Format("[Twitch] - {0} has just started streaming! {1}", str, "www.twitch.tv/" + str));
				}
			}
		}		

		


		

		private static Locales GetStageLocales(string Name)
		{
			Locales _StageNameTxt = TextLocale.locale.FirstOrDefault(x => x.EnUs != null && (x.EnUs.ToLower() == Name.ToLower()));
			if (_StageNameTxt == null)
			{
				List<Locales> _CheckList = TextLocale.locale.Where(x => x.EnUs != null && x.EnUs.ToLower().Contains(Name.ToLower())).ToList();
				bool found = false;
				foreach (Locales _loc in _CheckList)
				{
					if (StageBase.Stage.Count(x => x.Name == _loc.Id) >= 1)
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

		private static string GetStage(string Name, out Stages _Info)
		{
			Name = Name.TrimStart(' ');
			Locales _StageNameTxt = GetStageLocales(Name);
			_Info = null;

			if (_StageNameTxt == null)
			{
				return string.Format("Could not find Stage named: {0}!", Name);
			}

			Stage _Stage = StageBase.Stage.FirstOrDefault(x => x.Name == _StageNameTxt.Id);
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

			List<string> _BossTxt = new List<string>();
			//if (_Stage.Bosses != null)
			//{
			string _String = _Stage.Bosses.ToString();
			Boss _Boss = JsonConvert.DeserializeObject<Boss>(_String);
			foreach (List<BossInfo> _BossInfoList in _Boss.bosses.Values)
			{
				_BossTxt.AddRange(_BossInfoList.Select(x => x.desc));
			}
			//foreach (object _BossObj in _Bosses)
			//{

			//	Locales _BossNameTxt = TextLocale.locale.FirstOrDefault(x => x.Id == _Boss.desc);
			//	if (_BossNameTxt == null)
			//	{
			//		return string.Format("Could not find Boss Description ID: {0}!", _Boss.desc);
			//	}
			//	_BossTxt.Add(_BossNameTxt.EnUs + " Lvl." + _Boss.level.ToString());
			//}
			//}

			List<string> _WeaponTxt = new List<string>();

			foreach (string _Weapon in _Stage.DropableWeaponType.Classids)
			{
				WeaponCategory _WeaponCat = WeaponCategoryBase.WeaponCategory.FirstOrDefault(x => x.Id == _Weapon);
				if (_WeaponCat == null)
				{
					return string.Format("Could not find Weapon Cat: {0}!", _Weapon);
				}

				Locales _WeaponCatName = TextLocale.locale.FirstOrDefault(x => x.Id == _WeaponCat.Name);
				if (_WeaponCat == null)
				{
					return string.Format("Could not find Weapon Cat ID: {0}!", _WeaponCat.Name);
				}

				_WeaponTxt.Add(_WeaponCatName.EnUs);
			}

			_Info = new Stages();
			_Info.Bosses = string.Join(", ", _BossTxt);
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

		

		

		static void CreateWeaponDump()
		{
			using (FileStream fs = new FileStream(fileName, FileMode.Append, FileAccess.Write))
			{
				using (StreamWriter sw = new StreamWriter(fs))
				{
					sw.WriteLine(string.Format("weapons.Name, weapons.AttackDamage, weapons.AttackSpeed, weapons.CategoryName, weapons.Rarity, weapons.Slot1, weapons.Slot2, weapons.Slot3"));
				}
			}
			using (FileStream fs = new FileStream(fileName, FileMode.Append, FileAccess.Write))
			{
				using (StreamWriter sw = new StreamWriter(fs))
				{
					foreach (Weapon _Weaps in WeaponBase.Weapon)
					{
						Locales _WeaponNameTxt = TextLocale.locale.FirstOrDefault(x => x.Id == _Weaps.Name);

						if (_WeaponNameTxt == null)
						{
							continue;
						}

						Weapon _Weapon = WeaponBase.Weapon.FirstOrDefault(x => x.Name == _WeaponNameTxt.Id);
						if (_Weapon == null)
						{
							continue;
						}

						Locales _WeaponDescTxt = TextLocale.locale.FirstOrDefault(x => x.Id == _Weapon.Desc);
						if (_WeaponDescTxt == null)
						{
							continue;
						}

						WeaponConvertCost _WeaponUpgradeCost = WeaponConvertCostBase.WeaponConvertCost.FirstOrDefault(x => x.Grade == _Weapon.Grade);
						if (_WeaponUpgradeCost == null)
						{
							continue;
						}

						WeaponSellCost _WeaponSell = WeaponSellCostBase.WeaponSellCost.FirstOrDefault(x => x.Grade == _Weapon.Grade);
						if (_WeaponSell == null)
						{
							continue;
						}

						WeaponCategory _WeaponCategory = WeaponCategoryBase.WeaponCategory.FirstOrDefault(x => x.Id == _Weapon.Categoryid);
						if (_WeaponCategory == null)
						{
							continue;
						}

						Locales _WeaponCategoryNameTxt = TextLocale.locale.FirstOrDefault(x => x.Id == _WeaponCategory.Name);
						if (_WeaponCategoryNameTxt == null)
						{
							continue;
						}

						Weapons weapons = new Weapons();

						weapons.Name = _WeaponNameTxt.EnUs;
						weapons.Description = _WeaponDescTxt.EnUs;
						weapons.AttackSpeed = _Weapon.Attspd;
						weapons.AttackDamage = _Weapon.Attdmg;
						weapons.CategoryName = _WeaponCategoryNameTxt.EnUs;
						weapons.Range = _Weapon.Range;
						weapons.Rarity = _Weapon.Rarity;
						weapons.SellAmount = _WeaponSell.Goldcost;
						weapons.Slot1 = _Weapon.ConvertSlot1;
						weapons.Slot2 = _Weapon.ConvertSlot2;
						weapons.Slot3 = _Weapon.ConvertSlot3;
						weapons.Stars = _Weapon.Grade;

						sw.WriteLine(string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", weapons.Name, weapons.AttackDamage, weapons.AttackSpeed, weapons.CategoryName, weapons.Rarity, weapons.Slot1, weapons.Slot2, weapons.Slot3));
					}
				}
			}
		}

		static void CreateHeroDump()
		{
			using (FileStream fs = new FileStream(fileName, FileMode.Append, FileAccess.Write))
			{
				using (StreamWriter sw = new StreamWriter(fs))
				{
					sw.WriteLine(string.Format("_Info.Name, _Info.Gender, _Info.Stats.AttackDamage, _Info.Stats.Defence, _Info.Stats.Resist, _Info.Stats.HP, _Info.Stats.GrowthDamage, _Info.Stats.GrowthDamage, _Info.Stats.GrowthResist, _Info.Stats.GrowthHP, _Info.Stats.CritChance, _Info.Stats.CritPower, _Info.Stats.Star"));
				}
			}
			using (FileStream fs = new FileStream(fileName, FileMode.Append, FileAccess.Write))
			{
				using (StreamWriter sw = new StreamWriter(fs))
				{
					foreach (Character _Chars in CharacterBase.Character)
					{
						Information _Info = new Information();
						//gets character name
						Locales _CharNameTxt = TextLocale.locale.FirstOrDefault(x => x.Id != null && x.Id == _Chars.Name);
						if (_CharNameTxt == null)
						{
							continue;
						}

						//gets character id data
						Character _Char = CharacterBase.Character.FirstOrDefault(x => x.Name == _CharNameTxt.Id);
						if (_Char == null)
						{
							continue;
						}

						//gets character description
						Locales _CharDescTxt = TextLocale.locale.FirstOrDefault(x => x.Id == _Char.Desc);
						if (_CharDescTxt == null)
						{
							continue;
						}

						//gets character stats
						CharacterStat _CharStat = CharacterStatBase.CharacterStat.FirstOrDefault(x => x.ID == _Char.DefaultStatId);
						if (_CharStat == null)
						{
							continue;
						}

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

						_Info.HowToGet = (_Char.Howtoget == null ? "" : string.Join(", ", _Char.Howtoget));
						sw.WriteLine(string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}", _Info.Name, _Info.Gender, _Info.Stats.AttackDamage, _Info.Stats.Defence, _Info.Stats.Resist, _Info.Stats.HP, _Info.Stats.GrowthDamage, _Info.Stats.GrowthDefence, _Info.Stats.GrowthResist, _Info.Stats.GrowthHP, _Info.Stats.CritChance, _Info.Stats.CritPower, _Info.Stats.Star));
					}
				}
			}
		}

		static void SetupDB()
		{
			if (!System.IO.Directory.Exists("D:\\"))
			{
				System.IO.Directory.CreateDirectory(SavePath);
			}
			string sql = Path.Combine(SavePath, "irc.sqlite");
			Database = new SqliteConnection(string.Format("uri=file://{0},Version=3", sql));
			SqlTableCreator sqlcreator = new SqlTableCreator(Database, (IQueryBuilder)new SqliteQueryCreator());
			sqlcreator.EnsureExists(new SqlTable("CQDB",
				new SqlColumn("NickName", MySqlDbType.VarChar) { Primary = true, Length = 99 },
				new SqlColumn("Heros", MySqlDbType.Text),
				new SqlColumn("Weapons", MySqlDbType.Text),
				new SqlColumn("Skills", MySqlDbType.Text)
				));
		}
	}
}
*/
