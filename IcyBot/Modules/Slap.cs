using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Meebey.SmartIrc4net;

namespace IcyBot.Modules
{
	public class Slap : IrcPlugin
	{
		public override void Initialize()
		{
			Commands.ChatCommands.Add(new Command(SlapMain, "slap"));
		}

		private void SlapMain(CommandArgs args)
		{
			if (args.Parameters.Count == 0)
			{
				args.Args.Data.SendErrorText("slap <name>");
				return;
			}
			args.Args.Data.SendText("{0} slaps {1} around a bit with a {2}", args.Args.Data.Nick, string.Join(" ", args.Parameters), JsonPhraser.Animals[IcyBot.Rand.Next(0, JsonPhraser.Animals.Count - 1)]);
		}
	}
}
