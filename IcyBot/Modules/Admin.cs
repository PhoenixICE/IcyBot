using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Meebey.SmartIrc4net;

namespace IcyBot.Modules
{
	public class Admin : IrcPlugin
	{
		public override void Initialize()
		{
			Commands.ChatCommands.Add(new Command(Kick, "kick"));
			Commands.ChatCommands.Add(new Command(Interval, "interval"));
			Commands.ChatCommands.Add(new Command(Raw, "raw"));
			Commands.ChatCommands.Add(new Command(Ban, "ban"));
			Commands.ChatCommands.Add(new Command(UnBan, "unban"));
			Commands.ChatCommands.Add(new Command(Voice, "voice"));
			Commands.ChatCommands.Add(new Command(DeVoice, "devoice"));
			Commands.ChatCommands.Add(new Command(Mute, "mute"));
			Commands.ChatCommands.Add(new Command(UnMute, "unmute"));
			Commands.ChatCommands.Add(new Command(NickBan, "nc"));
			Commands.ChatCommands.Add(new Command(UnNickBan, "unnc"));
			Commands.ChatCommands.Add(new Command(Moderate, "mod"));
			Commands.ChatCommands.Add(new Command(DeModerate, "demod"));
		}

		private void UnNickBan(CommandArgs args)
		{
			if (!args.Args.IsOp())
			{
				return;
			}
			string inputstring = string.Join(" ", args.Parameters);
			var user = args.Client.GetChannelUser(args.Args.Data.Channel, string.Join(" ", inputstring));
			if (user == null)
			{
				args.Args.Data.SendText("Can't find User: {0}", inputstring);
			}
			else
			{
				args.Args.Data.RunCommand("mode {0} -b ~n:{1}", args.Args.Data.Channel, user.Host);
			}
		}

		private void NickBan(CommandArgs args)
		{
			if (!args.Args.IsOp())
			{
				return;
			}
			string inputstring = string.Join(" ", args.Parameters);
			var user = args.Client.GetChannelUser(args.Args.Data.Channel, string.Join(" ", inputstring));
			if (user == null)
			{
				args.Args.Data.SendText("Can't find User: {0}", inputstring);
			}
			else
			{
				args.Args.Data.RunCommand("mode {0} +b ~n:{1}", args.Args.Data.Channel, user.Host);
			}
		}

		private void Mute(CommandArgs args)
		{
			if (!args.Args.IsOp())
			{
				return;
			}
			string inputstring = string.Join(" ", args.Parameters);
			var user = args.Client.GetChannelUser(args.Args.Data.Channel, string.Join(" ", inputstring));
			if (user == null)
			{
				args.Args.Data.SendText("Can't find User: {0}", inputstring);
			}
			else
			{
				args.Args.Data.RunCommand("mode {0} +b ~q:{1}", args.Args.Data.Channel, user.Host);
			}
		}

		private void UnMute(CommandArgs args)
		{
			if (!args.Args.IsOp())
			{
				return;
			}
			string inputstring = string.Join(" ", args.Parameters);
			var user = args.Client.GetChannelUser(args.Args.Data.Channel, string.Join(" ", inputstring));
			if (user == null)
			{
				args.Args.Data.SendText("Can't find User: {0}", inputstring);
			}
			else
			{
				args.Args.Data.RunCommand("mode {0} -b ~q:{1}", args.Args.Data.Channel, user.Host);
			}
		}

		private void Voice(CommandArgs args)
		{
			if (!args.Args.IsOp())
			{
				return;
			}
			string inputstring = string.Join(" ", args.Parameters);
			var user = args.Client.GetChannelUser(args.Args.Data.Channel, string.Join(" ", inputstring));
			if (user == null)
			{
				args.Args.Data.SendText("Can't find User: {0}", inputstring);
			}
			else
			{
				args.Args.Data.RunCommand("cs VOP #cquest ADD {0}", user.Nick);
			}
		}

		private void DeVoice(CommandArgs args)
		{
			if (!args.Args.IsOp())
			{
				return;
			}
			string inputstring = string.Join(" ", args.Parameters);
			var user = args.Client.GetChannelUser(args.Args.Data.Channel, string.Join(" ", inputstring));
			if (user == null)
			{
				args.Args.Data.SendText("Can't find User: {0}", inputstring);
			}
			else
			{
				args.Args.Data.RunCommand("cs VOP #cquest DEL {0}", user.Nick);
			}
		}

		private void Moderate(CommandArgs args)
		{
			if (!args.Args.IsOp())
			{
				return;
			}
			args.Args.Data.SendText("This channel is now in Moderator mode only people with Voice or higher can speak.");
			args.Args.Data.RunCommand("mode {0} +m", args.Args.Data.Channel);	
		}

		private void DeModerate(CommandArgs args)
		{
			if (!args.Args.IsOp())
			{
				return;
			}
			args.Args.Data.SendText("Moderate mode has been disabled all users can now speak freely.");
			args.Args.Data.RunCommand("mode {0} -m", args.Args.Data.Channel);
		}

		private void Raw(CommandArgs args)
		{
			if (!args.Args.IsOp())
			{
				return;
			}
			string inputstring = string.Join(" ", args.Parameters);
			args.Client.SendMessage(SendType.Message, args.Args.Data.Channel, inputstring);
		}

		private void Ban(CommandArgs args)
		{
			if (!args.Args.IsOp())
			{
				return;
			}
			string inputstring = string.Join(" ", args.Parameters);
			var user = args.Client.GetChannelUser(args.Args.Data.Channel, string.Join(" ", inputstring));
			if (user == null)
			{
				args.Args.Data.SendText("Can't find User: {0}", inputstring);
			}
			else
			{
				args.Args.Data.RunCommand("mode {0} +b {1}", args.Args.Data.Channel, user.Host);
				args.Client.RfcKick(args.Args.Data.Channel, user.Nick);
			}
		}

		private void UnBan(CommandArgs args)
		{
			if (!args.Args.IsOp())
			{
				return;
			}
			string inputstring = string.Join(" ", args.Parameters);
			args.Args.Data.RunCommand("mode {0} -b {1}", args.Args.Data.Channel, inputstring);
		}

		private void Interval(CommandArgs args)
		{
			if (!args.Args.IsOp())
			{
				return;
			}
			int _interval;
			if (int.TryParse(args.Parameters[0], out _interval))
			{
				args.Client.SendMessage(SendType.Action, args.Args.Data.Channel, IcyBot.Config.Color + string.Format("Interval Set To: {0}s", _interval));
			}
		}

		private void Kick(CommandArgs args)
		{
			if (!args.Args.IsOp())
			{
				return;
			}
			string inputstring = string.Join(" ", args.Parameters);
			var user = args.Client.GetChannelUser(args.Args.Data.Channel, string.Join(" ", inputstring));
			if (user == null)
			{
				args.Args.Data.SendText("Can't find User: {0}", inputstring);
			}
			else
			{
				args.Client.RfcKick(args.Args.Data.Channel, user.Nick);
			}
		}
	}
}
