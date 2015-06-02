using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Meebey.SmartIrc4net;
using System.Text.RegularExpressions;

using System.IO;
using Newtonsoft.Json;

namespace IcyBot.Modules
{
	public class Quiz : IrcPlugin
	{
		public static bool QuizEnabled 
		{
			get 
			{
				return (QuizChannel.Count() > 0);
			}
		}
		public static string QuizQuestion { get; set; }
		public static string QuizAnswer { get; set; }
		public static string QuizHint { get; set; }
		public static Dictionary<string, int> QuizScores = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
		private readonly static object WriteLock = new object();
		public static string QuizLastAnswered { get; set; }
		public static int QuizStreak { get; set; }
		public static Timer QuizHintTimer = new Timer(15000);
		public static Timer QuizTimer = new Timer(60000);
		public static Timer QuizWaitTimer = new Timer(10000);
		public static int HintLevel = 0;
		public static DateTime QuizTimeCompare { get; set; }
		private static Dictionary<IrcClient, List<string>> QuizChannel = new Dictionary<IrcClient, List<string>>();

		public override void Initialize()
		{
			QuizTimer.Elapsed += QuizTimer_Elapsed;
			QuizWaitTimer.Elapsed += QuizWaitTimer_Elapsed;
			QuizHintTimer.Elapsed += QuizHintTimer_Elapsed;
			LoadQuizScores();
			QuizAnswer = string.Empty;
			QuizHint = string.Empty;
			QuizQuestion = string.Empty;
			QuizLastAnswered = string.Empty;
			Commands.ChatCommands.Add(new Command(QuizMain, "quiz"));
			Commands.ChatCommands.Add(new Command(ScoreMain, "score"));
		}

		private void ScoreMain(CommandArgs args)
		{
			string user = args.Args.Data.Nick;
			if (args.Parameters.Count() > 0)
			{
				user = string.Join(" ", args.Parameters);
			}

			int score;

			if (QuizScores.TryGetValue(user, out score))
			{
				args.Args.Data.SendText("{0} Score is: {1}", user, score);
			}
			else
			{
				args.Args.Data.SendText("{0} Score is: {1}", user, 0);
			}
		}

		private void QuizMain(CommandArgs args)
		{
			List<string> _channelList;
			if (QuizChannel.TryGetValue(args.Client, out _channelList) && _channelList.Contains(args.Args.Data.Channel))
			{
				_channelList.Remove(args.Args.Data.Channel);
				if (_channelList.Count == 0)
				{
					QuizChannel.Remove(args.Client);
				}
				args.Args.Data.SendText("Quiz is now: Disabled");
			}
			else
			{
				bool _start = false;
				if (!QuizEnabled)
				{
					_start = true;
				}
				if (!QuizChannel.TryGetValue(args.Client, out _channelList))
				{
					QuizChannel.Add(args.Client, new List<string>() { args.Args.Data.Channel });
				}
				args.Client.OnRawMessage += new IrcEventHandler(QuizAnswerMethod);
				args.Args.Data.SendText("Quiz is now: Enabled");
				if (_start)
					QuizWaitTimer.Start();
				else
					args.Args.Data.SendText(QuizQuestion);		
			}
		}

		public void LoadQuizScores()
		{
			using (StreamReader r = new StreamReader(@"D:\trivia.txt"))
			{
				string json = r.ReadToEnd();
				QuizScores = JsonConvert.DeserializeObject<Dictionary<string, int>>(json);
				if (QuizScores == null)
				{
					QuizScores = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
				}
			}
		}

		public void QuizAnswerMethod(object sender, IrcEventArgs e)
		{
			if (QuizChannel[e.Data.Irc].Contains(e.Data.Channel, StringComparer.CurrentCultureIgnoreCase))
			{
				lock (WriteLock)
				{
					string answer = e.Data.Message;
					string answerer = e.Data.Nick;
					if (answer.ToLower().Trim() == QuizAnswer.ToLower().Trim())
					{
						int score = 1;
						bool comboBreak = false;
						bool newUser = false;
						if (QuizScores.ContainsKey(answerer))
						{
							QuizScores[answerer]++;
							score = QuizScores[answerer];
						}
						else
						{
							QuizScores.Add(answerer, 1);
							newUser = true;
						}

						if (QuizLastAnswered == answerer)
						{
							QuizStreak++;
						}
						else
						{
							if (!string.IsNullOrEmpty(QuizLastAnswered) && QuizStreak > 2)
							{
								comboBreak = true;
							}
							else
							{
								QuizStreak = 1;
							}
							QuizLastAnswered = answerer;
						}
						if (newUser)
						{
							e.Data.SendText("A New Challenger Approaches!");
						}
						if (comboBreak)
						{
							int bonusPoints = ((int)Math.Ceiling(QuizStreak / 2.0) > 10 ? 10 : (int)Math.Ceiling(QuizStreak / 2.0));
							e.Data.SendText("C-C-C-Combo Breaker! Bonus Score +{0}", bonusPoints);
							QuizScores[answerer] += bonusPoints;
							score = QuizScores[answerer];
							QuizStreak = 1;
						}
						foreach (KeyValuePair<IrcClient, List<string>> channel in QuizChannel)
							foreach (string _chan in channel.Value)
								channel.Key.SendMessage(SendType.Message, _chan, IcyBot.Config.Color + string.Format("Correct: {0}, Player {1} now has {2} Points{4}, with a Streak of {3} - Time Taken {5}s", QuizAnswer, answerer, score, QuizStreak, (QuizStreak >= 3 ? " +1 Streak Bonus" : ""), (DateTime.Now - QuizTimeCompare).TotalSeconds));
						if (QuizStreak >= 3)
							QuizScores[answerer]++;
						QuizAnswer = string.Empty;
						QuizQuestion = string.Empty;
						HintLevel = 0;
						QuizHintTimer.Stop();
						QuizTimer.Stop();
						QuizWaitTimer.Start();
						string json = JsonConvert.SerializeObject(QuizScores);

						//write string to file
						System.IO.File.WriteAllText(@"D:\trivia.txt", json);
					}
				}
			}
		}

		private string RandQuiz()
		{
			string[] types = { "Hero", "Bread", "Skill", "Passive", "Goddess", "Speaker" };
			string randtype = types[IcyBot.Rand.Next(0, types.Length)];
			string question = "";
			Locales _txt;
			while (string.IsNullOrEmpty(question))
			{
				switch (randtype)
				{
					case "Hero":
						Character _char = JsonPhraser.CharacterBase.Character[IcyBot.Rand.Next(0, JsonPhraser.CharacterBase.Character.Length)];
						if (_char == null)
							break;
						_txt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.Id != null && x.Id == _char.Name);
						if (_txt == null)
							break;
						QuizAnswer = _txt.EnUs;
						_txt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.Id != null && x.Id == _char.Desc);
						if (_txt == null)
							break;
						question = _txt.EnUs;
						break;
					case "Bread":
						Bread _bread = JsonPhraser.BreadBase.Bread[IcyBot.Rand.Next(0, JsonPhraser.BreadBase.Bread.Length)];
						if (_bread == null)
							break;
						_txt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.Id != null && x.Id == _bread.Name);
						if (_txt == null)
							break;
						QuizAnswer = _txt.EnUs;
						question = string.Format("{0} Train {1}% Great Chance", _bread.Trainpoint, (_bread.Critprob * 100));
						break;
					case "Skill":
						SkillDef _skill = JsonPhraser.SkillBase.Skill[IcyBot.Rand.Next(0, JsonPhraser.SkillBase.Skill.Length)];
						if (_skill == null)
							break;
						_txt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.Id != null && x.Id == _skill.Name);
						if (_txt == null)
							break;
						QuizAnswer = _txt.EnUs;
						_txt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.Id != null && x.Id == _skill.Desc);
						if (_txt == null)
							break;
						question = _txt.EnUs;
						break;
					case "Passive":
						Passive _passive = JsonPhraser.PassiveBase.Passive[IcyBot.Rand.Next(0, JsonPhraser.PassiveBase.Passive.Length)];
						if (_passive == null)
							break;
						_txt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.Id != null && x.Id == _passive.Name);
						if (_txt == null)
							break;
						QuizAnswer = _txt.EnUs;
						_txt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.Id != null && x.Id == _passive.Desc);
						if (_txt == null)
							break;
						question = _txt.EnUs;
						break;
					case "Goddess":
						Sister _goddess = JsonPhraser.GoddessBase.Sister[IcyBot.Rand.Next(0, JsonPhraser.GoddessBase.Sister.Length)];
						if (_goddess == null)
							break;
						_txt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.Id != null && x.Id == _goddess.Name);
						if (_txt == null)
							break;
						QuizAnswer = _txt.EnUs;
						_txt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.Id != null && x.Id == _goddess.Skilldesc);
						if (_txt == null)
							break;
						question = _txt.EnUs;
						break;
					case "Speaker":
						Dialogue _dialogue = JsonPhraser.DialogueBase.Dialogues[IcyBot.Rand.Next(0, JsonPhraser.DialogueBase.Dialogues.Length)];
						if (_dialogue == null)
							break;
						Talkjson _talkjson = _dialogue.Talkjson[IcyBot.Rand.Next(0, _dialogue.Talkjson.Length)];
						if (_talkjson == null)
							break;
						_txt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.Id != null && x.Id == _talkjson.TalkerName);
						if (_txt == null)
							break;
						if (_txt.EnUs == null || _txt.EnUs.Length < 9)
						{
							break;
						}
						QuizAnswer = _txt.EnUs;
						_txt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.Id != null && x.Id == _talkjson.TalkerDesc);
						if (_txt == null)
							break;
						question = _txt.EnUs;
						break;
					case "Weapon":
						Weapon _weapon = JsonPhraser.WeaponBase.Weapon[IcyBot.Rand.Next(0, JsonPhraser.WeaponBase.Weapon.Length)];
						if (_weapon == null)
							break;
						_txt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.Id != null && x.Id == _weapon.Name);
						if (_txt == null)
							break;
						QuizAnswer = _txt.EnUs;
						_txt = JsonPhraser.TextLocale.locale.FirstOrDefault(x => x.Id != null && x.Id == _weapon.Categoryid);
						if (_txt == null)
							break;
						question = string.Format("Slots {0} {1} {2} Type {3} Damage {4}", _weapon.ConvertSlot1, _weapon.ConvertSlot2, _weapon.ConvertSlot3, _weapon.Categoryid, _weapon.Attdmg);
						break;
				}
			}
			if (randtype == "Skill" || randtype == "Passive")
			{
				randtype = "Skill|Passive";
			}
			QuizAnswer = QuizAnswer.Replace("\n", " ");
			foreach (string str in QuizAnswer.Split(' '))
			{
				if (str.Length > 3)
				{
					question = Regex.Replace(question, str, "_____", RegexOptions.IgnoreCase);
				}
			}
			return "Question: Name the " + randtype + " - " + question;
		}

		void QuizHintTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (HintLevel < 3)
			{
				HintLevel++;
				int _noLetters = (int)(Math.Ceiling((HintLevel / 6.0 * QuizAnswer.Length)));
				for (int i = 0; i < _noLetters; i++)
				{
					int _pos = IcyBot.Rand.Next(0, QuizHint.Length);
					StringBuilder sb = new StringBuilder(QuizHint);
					sb[_pos] = QuizAnswer[_pos];
					QuizHint = sb.ToString();
				}
				foreach(KeyValuePair<IrcClient, List<string>> channel in QuizChannel)
					foreach(string _chan in channel.Value)
						channel.Key.SendMessage(SendType.Message, _chan, IcyBot.Config.Color + "Hint: " + QuizHint);
			}
			else
			{
				QuizHintTimer.Enabled = false;
			}
		}

		void QuizWaitTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			QuizQuestion = RandQuiz();
			QuizWaitTimer.Enabled = false;
			foreach (KeyValuePair<IrcClient, List<string>> channel in QuizChannel)
				foreach (string _chan in channel.Value)
					channel.Key.SendMessage(SendType.Message, _chan, IcyBot.Config.Color + QuizQuestion);

			Regex rgx = new Regex(@"[a-zA-Z0-9]");
			QuizHint = rgx.Replace(QuizAnswer, "-");
			QuizTimer.Enabled = true;
			QuizHintTimer.Enabled = true;
			QuizTimeCompare = DateTime.Now;
		}

		void QuizTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			foreach (KeyValuePair<IrcClient, List<string>> channel in QuizChannel)
				foreach (string _chan in channel.Value)
					channel.Key.SendMessage(SendType.Message, _chan, IcyBot.Config.ErrorColor + string.Format("Times Up! Answer was: {0}", QuizAnswer));
			QuizAnswer = "";
			QuizLastAnswered = "";
			QuizStreak = 1;
			HintLevel = 0;
			QuizHintTimer.Enabled = false;
			QuizWaitTimer.Enabled = true;
			QuizTimer.Enabled = false;
			return;
		}
	}
}
