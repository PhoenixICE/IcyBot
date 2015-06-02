using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Meebey.SmartIrc4net;
using ChatterBotAPI;

namespace IcyBot.Modules
{
	public class ChatBot : IrcPlugin
	{
		public static ChatterBotFactory factory = new ChatterBotFactory();
		//public static ChatterBot bot1 = factory.Create(ChatterBotType.PANDORABOTS, "b0dafd24ee35a477");
		//public static ChatterBot bot1 = factory.Create(ChatterBotType.JABBERWACKY);
		public static ChatterBot bot1 = factory.Create(ChatterBotType.CLEVERBOT);
		public static ChatterBotSession bot1session = bot1.CreateSession();	

		public override void Initialize()
		{
			Commands.ChatCommands.Add(new Command(ChatBotMain, "icybot"));
		}

		private void ChatBotMain(CommandArgs args)
		{
			args.Args.Data.SendText(bot1session.Think(string.Join(" ", args.Parameters)));
		}
	}
}
