using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Meebey.SmartIrc4net;

namespace IcyBot.Modules
{
	public class AskEightBall : IrcPlugin
	{
		public override void Initialize()
		{
			EightBall.SetupEightBall();
			Commands.ChatCommands.Add(new Command(EightBallMain, "8ball"));
		}

		private void EightBallMain(CommandArgs args)
		{
			args.Args.Data.SendText(EightBall.Answers[IcyBot.Rand.Next(0, EightBall.Answers.Count - 1)]);
		}
	}
}
