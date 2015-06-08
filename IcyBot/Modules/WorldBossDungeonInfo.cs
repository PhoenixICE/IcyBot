using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Meebey.SmartIrc4net;

namespace IcyBot.Modules
{
	public class WorldBossDungeonInfo : IrcPlugin
	{
		public static readonly List<string> WorldBosses = new List<string>()
		{
			"Obelisk - Golem", "Fenriruth - Worm", "Kranus - Squid", "Magnax - Dragon"
		};

		public static readonly List<string> WorldBossWeapons = new List<string>()
		{
			"Hammer, Bow, Gun", "Sword, Hammer, Staff", "Sword, Bow, Relics (healer)", "Gun, Staff, Relics (healer)"
		};

		public static readonly List<string> Dungeons = new List<string>()
		{
			"Black and White", "Disarm", "Gravity"
		};

		public static readonly List<string> Dungeons2 = new List<string>()
		{
			"The Void", "Path of Rage", "Road to Ruin"
		};

		public static readonly List<string> DungeonHeroes = new List<string>()
		{
			"Dorothy and Maria", "Leon and D'art", "Sigruna and Kriemhild"
		};

		public static readonly List<string> DungeonHeroes2 = new List<string>()
		{
			"Roland and Rochefort", "Cano and Theresa", "Melrisa and Demona"
		};

		public static readonly List<string> DungeonRestrictions = new List<string>()
		{
			"Mage and Priest", "Warrior and Hunter", "Archer and Paladin"
		};

		public static readonly List<string> DungeonMechanics = new List<string>()
		{
			"More than 30 SP Can be Risky!", "When one enemy is down, the other get stronger!", "N/A"
		};

		public static readonly DateTime DateDungeonWorld = new DateTime(2015, 2, 16, 11, 0, 0);

		public override void Initialize()
		{
			Commands.ChatCommands.Add(new Command(WorldBossDungeonInfoMain, "info"));
		}

		public void WorldBossDungeonInfoMain(CommandArgs args)
		{
			args.Args.Data.SendText(WorldBossInfo());
			args.Args.Data.SendText(DungeonInfo());
			args.Args.Data.SendText(DungeonInfo2());
		}

		private string DungeonInfo()
		{
			int Weeks = (int)Math.Floor(((DateTime.Now - DateDungeonWorld).TotalDays / 7.0));
			int dungeon = Weeks % Dungeons.Count;
			return String.Format("Dungeon this week is {6}{0}{6}, can't use {6}{2}{6}, drops {6}{1}{6}. Next week is {6}{3}{6}, can't use {6}{5}{6}, drops {6}{4}{6}.", Dungeons[dungeon], DungeonHeroes[dungeon], DungeonRestrictions[dungeon], (dungeon == Dungeons.Count - 1 ? Dungeons[0] : Dungeons[dungeon + 1]), (dungeon == DungeonHeroes.Count - 1 ? DungeonHeroes[0] : DungeonHeroes[dungeon + 1]), (dungeon == DungeonRestrictions.Count - 1 ? DungeonRestrictions[0] : DungeonRestrictions[dungeon + 1]), ControlCode.Bold);
		}

		private string DungeonInfo2()
		{
			int Weeks = (int)Math.Floor(((DateTime.Now - DateDungeonWorld).TotalDays / 7.0));
			int dungeon = Weeks % Dungeons.Count;
			return String.Format("Dungeon this week is {6}{0}{6}, dungeon mechanic is {6}{2}{6}, drops {6}{1}{6}. Next week is {6}{3}{6}, dungeon mechanic is {6}{5}{6}, drops {6}{4}{6}.", Dungeons2[dungeon], DungeonHeroes2[dungeon], DungeonMechanics[dungeon], (dungeon == Dungeons2.Count - 1 ? Dungeons2[0] : Dungeons2[dungeon + 1]), (dungeon == DungeonHeroes2.Count - 1 ? DungeonHeroes2[0] : DungeonHeroes2[dungeon + 1]), (dungeon == DungeonMechanics.Count - 1 ? DungeonMechanics[0] : DungeonMechanics[dungeon + 1]), ControlCode.Bold);
		}

		private string WorldBossInfo()
		{
			int Weeks = (int)Math.Floor(((DateTime.Now - DateDungeonWorld).TotalDays / 7.0));
			int worldboss = Weeks % WorldBosses.Count;
			return String.Format("World Boss this week is {4}{0}{4}, drops {4}{1}{4}. Next week is {4}{2}{4}, drops {4}{3}{4}.", WorldBosses[worldboss], WorldBossWeapons[worldboss], (worldboss == WorldBosses.Count - 1 ? WorldBosses[0] : WorldBosses[worldboss + 1]), (worldboss == WorldBossWeapons.Count - 1 ? WorldBossWeapons[0] : WorldBossWeapons[worldboss + 1]), ControlCode.Bold);
		}
	}
}
