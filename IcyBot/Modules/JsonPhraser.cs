using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Newtonsoft.Json;

namespace IcyBot.Modules
{
	public class JsonPhraser : IrcPlugin
	{
		public static CharacterStatBase CharacterStatBase;
		public static CharacterClassBase CharacterClassBase;
		public static PassiveBase PassiveBase;
		public static TextLocale TextLocale;
		public static SkillBase SkillBase;
		public static CharacterBase CharacterBase;
		public static WeaponBase WeaponBase;
		public static WeaponCategoryBase WeaponCategoryBase;
		public static WeaponConvertCostBase WeaponConvertCostBase;
		public static WeaponConvertExclusiveOptionBase WeaponConvertExclusiveOptionBase;
		public static WeaponConvertListBase WeaponConvertListBase;
		public static WeaponDismantlePropBase WeaponDismantlePropBase;
		public static WeaponGradeupCostBase WeaponGradeupCostBase;
		public static WeaponSellCostBase WeaponSellCostBase;
		public static GoddessBase GoddessBase;
		public static BreadBase BreadBase;
		public static StageBase StageBase;
		public static DialogueBase DialogueBase;
		public static PvPRewardBase PvPRewardBase;
		public static PvPWeekRewardBase PvPWeekRewardBase;
		public static List<string> Animals = new List<string>();

		public override void Initialize()
		{
			using (StreamReader r = new StreamReader(IcyBot.Config.SavePath + "get_character_stat.txt"))
			{
				string json = r.ReadToEnd();
				CharacterStatBase = JsonConvert.DeserializeObject<CharacterStatBase>(json);
			}
			using (StreamReader r = new StreamReader(IcyBot.Config.SavePath + "get_characterclass.txt"))
			{
				string json = r.ReadToEnd();
				CharacterClassBase = JsonConvert.DeserializeObject<CharacterClassBase>(json);
			}
			using (StreamReader r = new StreamReader(IcyBot.Config.SavePath + "get_passive.txt"))
			{
				string json = r.ReadToEnd();
				PassiveBase = JsonConvert.DeserializeObject<PassiveBase>(json);
			}
			using (StreamReader r = new StreamReader(IcyBot.Config.SavePath + "get_textlocale.txt"))
			{
				string json = r.ReadToEnd();
				TextLocale = JsonConvert.DeserializeObject<TextLocale>(json);
			}
			using (StreamReader r = new StreamReader(IcyBot.Config.SavePath + "get_skill.txt"))
			{
				string json = r.ReadToEnd();
				SkillBase = JsonConvert.DeserializeObject<SkillBase>(json);
			}
			using (StreamReader r = new StreamReader(IcyBot.Config.SavePath + "get_character_visual.txt"))
			{
				string json = r.ReadToEnd();
				CharacterBase = JsonConvert.DeserializeObject<CharacterBase>(json);
			}
			using (StreamReader r = new StreamReader(IcyBot.Config.SavePath + "get_weapon.txt"))
			{
				string json = r.ReadToEnd();
				WeaponBase = JsonConvert.DeserializeObject<WeaponBase>(json);
			}
			using (StreamReader r = new StreamReader(IcyBot.Config.SavePath + "get_weapon_convert_cost.txt"))
			{
				string json = r.ReadToEnd();
				WeaponConvertCostBase = JsonConvert.DeserializeObject<WeaponConvertCostBase>(json);
			}
			using (StreamReader r = new StreamReader(IcyBot.Config.SavePath + "get_weapon_convert_exclusive_option.txt"))
			{
				string json = r.ReadToEnd();
				WeaponConvertExclusiveOptionBase = JsonConvert.DeserializeObject<WeaponConvertExclusiveOptionBase>(json);
			}
			using (StreamReader r = new StreamReader(IcyBot.Config.SavePath + "get_weapon_convert_list.txt"))
			{
				string json = r.ReadToEnd();
				WeaponConvertListBase = JsonConvert.DeserializeObject<WeaponConvertListBase>(json);
			}
			using (StreamReader r = new StreamReader(IcyBot.Config.SavePath + "get_weapon_dismantle_props.txt"))
			{
				string json = r.ReadToEnd();
				WeaponDismantlePropBase = JsonConvert.DeserializeObject<WeaponDismantlePropBase>(json);
			}
			using (StreamReader r = new StreamReader(IcyBot.Config.SavePath + "get_weapon_gradeup_cost.txt"))
			{
				string json = r.ReadToEnd();
				WeaponGradeupCostBase = JsonConvert.DeserializeObject<WeaponGradeupCostBase>(json);
			}
			using (StreamReader r = new StreamReader(IcyBot.Config.SavePath + "get_weapon_sell_cost.txt"))
			{
				string json = r.ReadToEnd();
				WeaponSellCostBase = JsonConvert.DeserializeObject<WeaponSellCostBase>(json);
			}
			using (StreamReader r = new StreamReader(IcyBot.Config.SavePath + "get_weaponcategory.txt"))
			{
				string json = r.ReadToEnd();
				WeaponCategoryBase = JsonConvert.DeserializeObject<WeaponCategoryBase>(json);
			}
			using (StreamReader r = new StreamReader(IcyBot.Config.SavePath + "get_sister.txt"))
			{
				string json = r.ReadToEnd();
				GoddessBase = JsonConvert.DeserializeObject<GoddessBase>(json);
			}
			using (StreamReader r = new StreamReader(IcyBot.Config.SavePath + "get_bread.txt"))
			{
				string json = r.ReadToEnd();
				BreadBase = JsonConvert.DeserializeObject<BreadBase>(json);
			}
			using (StreamReader r = new StreamReader(IcyBot.Config.SavePath + "get_stage.txt"))
			{
				string json = r.ReadToEnd();
				StageBase = JsonConvert.DeserializeObject<StageBase>(json);
			}
			using (StreamReader r = new StreamReader(IcyBot.Config.SavePath + "get_pvp_roundreward.txt"))
			{
				string json = r.ReadToEnd();
				PvPRewardBase = JsonConvert.DeserializeObject<PvPRewardBase>(json);
			}
			using (StreamReader r = new StreamReader(IcyBot.Config.SavePath + "get_pvp_weekreward.txt"))
			{
				string json = r.ReadToEnd();
				PvPWeekRewardBase = JsonConvert.DeserializeObject<PvPWeekRewardBase>(json);
			}
			using (StreamReader r = new StreamReader(IcyBot.Config.SavePath + "get_dialogues.txt"))
			{
				string json = r.ReadToEnd();
				DialogueBase = JsonConvert.DeserializeObject<DialogueBase>(json);
			}
			Animals = File.ReadAllLines("D:\\animals.txt").ToList();
			/*
			using (StreamReader r = new StreamReader(@"D:\Streamers.txt"))
			{
				string json = r.ReadToEnd();
				Streamers = JsonConvert.DeserializeObject<List<string>>(json);
				if (Streamers == null)
				{
					Streamers = new List<string>();
				}
			}
		
			*/
			//using (FileStream fs = new FileStream(fileName, FileMode.Append, FileAccess.Write))
			//{
			//	using (StreamWriter sw = new StreamWriter(fs))
			//	{
			//		sw.WriteLine(string.Format("_Weaps.ConvertTarget, _Weaps.ConvertType, _Weaps.ConvertValueMin, _Weaps.ConvertValueMax, _Weaps.Huge"));
			//		foreach (WeaponConvertList _Weaps in WeaponConvertListBase.WeaponConvertList)
			//		{
			//			sw.WriteLine(string.Format("{0}, {1}, {2}, {3}, {4}", _Weaps.ConvertTarget, _Weaps.ConvertType, _Weaps.ConvertValueMin, _Weaps.ConvertValueMax, _Weaps.Huge));
			//		}
			//	}
			//}
			//CreateWeaponDump();
			//CreateHeroDump();
		}
	}
}
