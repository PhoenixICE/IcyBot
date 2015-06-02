﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Meebey.SmartIrc4net;

namespace IcyBot
{
	public delegate void CommandDelegate(CommandArgs args);
	public class CommandArgs : EventArgs
	{
		public IrcEventArgs Args { get; private set; }
		public IrcClient Client { get; private set; }
		public List<string> Parameters { get; private set; }
		public CommandArgs(IrcEventArgs args, IrcClient client, List<string> param)
		{
			Args = args;
			Client = client;
			Parameters = param;
		}
	}
	public class Command
	{
		public string Name { get; set; }
		public string HelpText { get; set; }
		private CommandDelegate commandDelegate;
		public CommandDelegate CommandDelegate
		{
			get { return commandDelegate; }
			set
			{
				commandDelegate = value;
			}
		}

		public Command(CommandDelegate cmd, string name)
		{
			CommandDelegate = cmd;
			HelpText = "No help available.";
			Name = name;
		}

		public bool Run(IrcEventArgs args, IrcClient client, List<string> param)
		{
			try
			{
				CommandDelegate(new CommandArgs(args, client, param));
			}
			catch (Exception e)
			{
				Console.Write(e.ToString());
				client.SendMessage(SendType.Message, args.Data.Channel, "An Error Has Occured Please Contact - IcyPhoenix");
			}

			return true;
		}
	}
	public static class Commands
	{
		public static List<Command> ChatCommands = new List<Command>();

		public static void HelpCommand()
		{
			ChatCommands.Add(new Command(Help, "help"));
		}

		private static void Help(CommandArgs args)
		{
			args.Client.SendMessage(SendType.Message, args.Args.Data.Channel, "Commands: " + string.Join(", ", ChatCommands.Select(x => x.Name)));
		}

		public static void HandleCommand(IrcEventArgs e, IrcClient client, out string error)
		{
			error = string.Empty;
			string text = e.Data.Message;
			string user = e.Data.Nick;
			string cmdText = text.Remove(0, 1);
			string cmdPrefix = text[0].ToString();

			var args = ParseParameters(cmdText);
			if (args.Count < 1)
				return;

			string cmdName = args[0].ToLower();
			args.RemoveAt(0);

			var cmd = ChatCommands.FirstOrDefault(c => c.Name.ToLower() == cmdName.ToLower());

			if (cmd == null)
			{
				error = string.Format("Invalid command entered. Type {0}help for a list of valid commands.", IcyBot.Config.CommandSpecifier);
				return;
			}
			cmd.Run(e, client, args);
		}

		private static List<String> ParseParameters(string str)
		{
			var ret = new List<string>();
			var sb = new StringBuilder();
			bool instr = false;
			for (int i = 0; i < str.Length; i++)
			{
				char c = str[i];

				if (c == '\\' && ++i < str.Length)
				{
					if (str[i] != '"' && str[i] != ' ' && str[i] != '\\')
						sb.Append('\\');
					sb.Append(str[i]);
				}
				else if (c == '"')
				{
					instr = !instr;
					if (!instr)
					{
						ret.Add(sb.ToString());
						sb.Clear();
					}
					else if (sb.Length > 0)
					{
						ret.Add(sb.ToString());
						sb.Clear();
					}
				}
				else if (IsWhiteSpace(c) && !instr)
				{
					if (sb.Length > 0)
					{
						ret.Add(sb.ToString());
						sb.Clear();
					}
				}
				else
					sb.Append(c);
			}
			if (sb.Length > 0)
				ret.Add(sb.ToString());

			return ret;
		}

		private static bool IsWhiteSpace(char c)
		{
			return c == ' ' || c == '\t' || c == '\n';
		}
	}
}
