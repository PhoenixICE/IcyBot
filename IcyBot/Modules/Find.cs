using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Meebey.SmartIrc4net;

namespace IcyBot.Modules
{
	public class Find : IrcPlugin
	{
		public override void Initialize() 
		{
			Commands.ChatCommands.Add(new Command(FindMain, "find"));
		}

		private void FindMain(CommandArgs args)
		{
			if (args.Parameters.Count == 0)
			{
				args.Args.Data.SendErrorText("find <hero/bread/weapon/skill/passive> <search string>");
				return;
			}
			args.Args.Data.SendText(FindMethod(string.Join(" ", args.Parameters)));
			//SendChat(Color + Find(e.PrivateMessage.Message.Substring(6)), e, client, Whisper);
		}

		private string FindMethod(string searchstr)
		{
			searchstr = searchstr.TrimStart(' ');
			List<string> param = searchstr.Split(' ').ToList();
			if (param.Count() <= 1)
			{
				return "/find <hero,bread,skill,passive> <search string>";
			}

			if (param.Count() == 2)
			{
				if (param[1].Length == 1)
				{
					return "Search string must be greater then 1 character!";
				}
			}
			string SearchCase = param[0];
			param.RemoveAt(0);
			searchstr = string.Join(" ", param);

			List<string> strings = new List<string>();

			switch (SearchCase.ToLower())
			{
				case "hero":
					foreach (Locales _Locale in JsonPhraser.TextLocale.locale)
					{
						if (_Locale.EnUs != null)
						{
							if (_Locale.EnUs.ToLower().Contains(searchstr.ToLower()))
							{
								if (JsonPhraser.CharacterBase.Character.Count(x => x.Name == _Locale.Id) != 0)
								{
									strings.Add(_Locale.EnUs);
								}
							}
						}
					}
					break;
				case "bread":
					foreach (Locales _Locale in JsonPhraser.TextLocale.locale)
					{
						if (_Locale.EnUs != null)
						{
							if (_Locale.EnUs.ToLower().Contains(searchstr.ToLower()))
							{
								if (JsonPhraser.BreadBase.Bread.Count(x => x.Name == _Locale.Id) != 0)
								{
									strings.Add(_Locale.EnUs);
								}
							}
						}
					}
					break;
				case "skill":
					foreach (Locales _Locale in JsonPhraser.TextLocale.locale)
					{
						if (_Locale.EnUs != null)
						{
							if (_Locale.EnUs.ToLower().Contains(searchstr.ToLower()))
							{
								if (JsonPhraser.SkillBase.Skill.Count(x => x.Name == _Locale.Id) != 0)
								{
									strings.Add(_Locale.EnUs);
								}
							}
						}
					}
					break;
				case "passive":
					foreach (Locales _Locale in JsonPhraser.TextLocale.locale)
					{
						if (_Locale.EnUs != null)
						{
							if (_Locale.EnUs.ToLower().Contains(searchstr.ToLower()))
							{
								if (JsonPhraser.PassiveBase.Passive.Count(x => x.Name == _Locale.Id) != 0)
								{
									strings.Add(_Locale.EnUs);
								}
							}
						}
					}
					break;
				case "stage":
					foreach (Locales _Locale in JsonPhraser.TextLocale.locale)
					{
						if (_Locale.EnUs != null)
						{
							if (_Locale.EnUs.ToLower().Contains(searchstr.ToLower()))
							{
								if (JsonPhraser.StageBase.Stage.Count(x => x.Name == _Locale.Id) != 0)
								{
									strings.Add(_Locale.EnUs);
								}
							}
						}
					}
					break;
				case "weapon":
					foreach (Locales _Locale in JsonPhraser.TextLocale.locale)
					{
						if (_Locale.EnUs != null)
						{
							if (_Locale.EnUs.ToLower().Contains(searchstr.ToLower()))
							{
								if (JsonPhraser.CharacterBase.Character.Count(x => x.Name == _Locale.Id) != 0)
								{
									//_Locale.Id = "TEXT_CHA_AR_4_1_NAME"
									List<string> _heroName = new List<string>();
									StringBuilder sb = new StringBuilder(_Locale.Id);
									sb[12] = '1';
									_heroName.Add(sb.ToString());
									sb[12] = '2';
									_heroName.Add(sb.ToString());
									sb[12] = '3';
									_heroName.Add(sb.ToString());
									sb[12] = '4';
									_heroName.Add(sb.ToString());
									sb[12] = '5';
									_heroName.Add(sb.ToString());
									sb[12] = '6';
									_heroName.Add(sb.ToString());
									foreach (WeaponConvertExclusiveOption _Weap in JsonPhraser.WeaponConvertExclusiveOptionBase.WeaponConvertExclusiveOption)
									{
										if (_heroName.Contains(_Weap.Reqheroname))
										{
											var _Wea = JsonPhraser.WeaponBase.Weapon.FirstOrDefault(x => x != null && x.Id == _Weap.WeaponId);
											if (_Wea != null)
											{
												Locales _loc = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x != null && x.Id == _Wea.Name);
												strings.Add(_loc.EnUs);
											}
										}
									}
									break;
								}
							}
						}
					}
					break;
				default:
					return string.Format("Invalid switch: {0}", SearchCase);
			}

			string returnStr = "";
			if (strings.Count > 0)
			{
				returnStr = string.Join(", ", strings);
			}
			else
			{
				returnStr = string.Format("No matching {0} found!", SearchCase.ToLower());
			}
			return returnStr;
			//string.Join(", ", TextLocale.Textlocale.Where(x => x.EnUs != null && x.EnUs.Length < 35 && x.EnUs.ToLower().Contains(searchstr)).Select(x => x.EnUs));
		}
	}
}
