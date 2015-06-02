using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IcyBot
{
	public static class EightBall
	{
		public static List<string> Answers { get; set; }
		
		public static void SetupEightBall()
		{
			Answers = new List<string>();
			Answers.Add("It is certain");
			Answers.Add("It is decidedly so");
			Answers.Add("Without a doubt");
			Answers.Add("Yes definitely");
			Answers.Add("You may rely on it");
			Answers.Add("As I see it, yes");
			Answers.Add("Most likely");
			Answers.Add("Outlook good");
			Answers.Add("Yes");
			Answers.Add("Signs point to yes");
			Answers.Add("Reply hazy try again");
			Answers.Add("Ask again later");
			Answers.Add("Better not tell you now");
			Answers.Add("Cannot predict now");
			Answers.Add("Concentrate and ask again");
			Answers.Add("Don't count on it");
			Answers.Add("My reply is no");
			Answers.Add("My sources say no");
			Answers.Add("Outlook not so good");
			Answers.Add("Very doubtful");
		}
	}
}
