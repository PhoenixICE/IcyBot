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
		public static ChatterBotFactory factory { get; set; }
		//public static ChatterBot bot1 = factory.Create(ChatterBotType.PANDORABOTS, "b0dafd24ee35a477");
		//public static ChatterBot bot1 = factory.Create(ChatterBotType.JABBERWACKY);
		public static ChatterBot bot1 { get; set; }
		public static ChatterBotSession bot1session { get; set; }

		public override void Initialize()
		{
			Commands.ChatCommands.Add(new Command(ChatBotMain, "icybot"));
			Commands.ChatCommands.Add(new Command(ChatBotSwap, "chatbot"));
			factory = new ChatterBotFactory(); 
			bot1 = factory.Create(ChatterBotType.CLEVERBOT);
			bot1session = bot1.CreateSession();
		}

		private void ChatBotSwap(CommandArgs args)
		{
			if (args.Parameters.Count == 0)
			{
				args.Args.Data.SendErrorText("chatbot <type> | type = {0}", string.Join(", ", Enum.GetValues(typeof(ChatterBotType)).Cast<ChatterBotType>().Select(x => x.ToString())));
				return;
			}
			ChatterBotType _switch = (ChatterBotType)Enum.Parse(typeof(ChatterBotType), args.Parameters[0], true);
			switch (_switch)
			{
				case ChatterBotType.CLEVERBOT:
					factory = new ChatterBotFactory(); 
					bot1 = factory.Create(ChatterBotType.CLEVERBOT);
					bot1session = bot1.CreateSession();
					return;
				case ChatterBotType.JABBERWACKY:
					factory = new ChatterBotFactory();
					bot1 = factory.Create(ChatterBotType.JABBERWACKY);
					bot1session = bot1.CreateSession();
					return;
				case ChatterBotType.PANDORABOTS:
					factory = new ChatterBotFactory();
					bot1 = factory.Create(ChatterBotType.PANDORABOTS, "b0dafd24ee35a477");
					bot1session = bot1.CreateSession();
					return;
				default:
					args.Args.Data.SendErrorText("chatbot <type> | type = {0}", Enum.GetValues(typeof(ChatterBotType)));
					return;
			}
		}

		private void ChatBotMain(CommandArgs args)
		{
			args.Args.Data.SendText(bot1session.Think(string.Join(" ", args.Parameters)));
		}
	}
}
