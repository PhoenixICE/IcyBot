using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Meebey.SmartIrc4net;

namespace IcyBot.Modules
{
	public class PvPCalculator : IrcPlugin
	{
		public readonly Dictionary<string, int> PvPLeagueRequirements = new Dictionary<string, int>()
		{
			{ "bronze", 300 }, { "silver", 1000 }, { "gold", 2500 }
		};

		public List<string> LeagueIDs = new List<string>();

		public override void Initialize()
		{
			Commands.ChatCommands.Add(new Command(PvPCalculactorMain, "pvp"));
			foreach (PvpRoundreward obj in JsonPhraser.PvPRewardBase.PvpRoundreward)
			{
				if (!LeagueIDs.Contains(obj.Leagueid))
				{
					LeagueIDs.Add(obj.Leagueid);
				}
			}
		}

		private void PvPCalculactorMain(CommandArgs args)
		{
			if (args.Parameters.Count() < 2)
			{
				args.Args.Data.SendText("pvp <bronze/silver/gold> <current score>");
				args.Args.Data.SendText("pvp master <current score> <target score>");
				return;
			}

			int Score;
			int OriginalScore = 0;
			int TargetScore = 0;
			string League;

			if (JsonPhraser.PvPRewardBase.PvpRoundreward.Count(x => x.Leagueid.ToLower() == args.Parameters[0].ToLower()) == 0)
			{
				args.Args.Data.SendText("Error: {0} is not a league rank!", args.Parameters[0]);
				return;
			}
			else
			{
				League = args.Parameters[0].ToLower();
			}

			if (!int.TryParse(args.Parameters[1], out Score))
			{
				args.Args.Data.SendErrorText( string.Format("Error: Score must be numerical, {0}", args.Parameters[1]));
				return;
			}
			OriginalScore = Score;
			if (League == "master")
			{
				if (args.Parameters.Count < 3)
				{
					args.Args.Data.SendErrorText("pvp master <current score> <target score>");
					return;
				}
				if (!int.TryParse(args.Parameters[2], out TargetScore))
				{
					args.Args.Data.SendErrorText( string.Format("Error: Rank must be numerical or less then 100000, {0}", args.Parameters[2]));
					return;
				}
				if (TargetScore > 100000)
				{
					args.Args.Data.SendErrorText( string.Format("Error: Rank must be numerical or less then 100000, {0}", args.Parameters[2]));
					return;
				}
			}

			bool _foundLeague = false;
			int TicketsRequired = 0;
			int CurrentStreak = 1;
			bool CurrentStreakLock = false;

			if (League != "master")
			{
				foreach (string str in LeagueIDs)
				{
					if (str.ToLower() != League && !_foundLeague)
					{
						continue;
					}
					if (str.ToLower() == "master")
					{
						break;
					}
					_foundLeague = true;
					bool _nextLeague = false;
					while (!_nextLeague)
					{
						if (CurrentStreak == 1)
							TicketsRequired++;
						for (int i = CurrentStreak; i < 11; i++)
						{
							PvpRoundreward RoundReward = JsonPhraser.PvPRewardBase.PvpRoundreward.FirstOrDefault(x => x.Leagueid.ToLower() == League.ToLower() && x.Streak == i);
							Score += RoundReward.Ratingvariation;
							CurrentStreakLock = false;
							if (PvPLeagueRequirements[str.ToLower()] <= Score)
							{
								for (int j = 0; j < LeagueIDs.Count - 1; j++)
								{
									if (LeagueIDs[j].ToLower() == League.ToLower() && j != LeagueIDs.Count - 1)
									{
										CurrentStreakLock = true;
										CurrentStreak = i + 1;
										League = LeagueIDs[j + 1];
										_nextLeague = true;
										break;
									}
								}
								Score = Score - PvPLeagueRequirements[str.ToLower()];
								break;
							}
						}
						if (CurrentStreak > 1 && !CurrentStreakLock)
						{
							CurrentStreak = 1;
						}
					}
					if (CurrentStreak > 10)
					{
						CurrentStreak = 1;
					}
				}
				args.Args.Data.SendText("Tickets Required to get into Masters: {0}", TicketsRequired);
				return;
			}
			else
			{
				bool _nextLeague = false;
				while (!_nextLeague)
				{
					TicketsRequired++;
					for (int i = 1; i < 11; i++)
					{
						PvpRoundreward RoundReward = JsonPhraser.PvPRewardBase.PvpRoundreward.FirstOrDefault(x => x.Leagueid.ToLower() == League.ToLower() && x.Streak == i);
						Score += RoundReward.Ratingvariation;
						if (TargetScore <= Score)
						{
							args.Args.Data.SendText("Tickets Required to get from Score {0} to Score {1} : {2} Tickets", OriginalScore, TargetScore, TicketsRequired);
							return;
						}
					}
				}
			}
		}
	}
}
