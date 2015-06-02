using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meebey.SmartIrc4net;

namespace IcyBot
{
	public static class ExtensionMethods
	{		
		public static void SendTextReal(IrcMessageData data, string text, string color, params object[] param)
		{
			if (param == null)
				param = new object[0];
			string message = string.Format(text, param);
			int _maxChar = -1;
			if (IcyBot.Config.MaxChars.TryGetValue(data.Irc.Address, out _maxChar))
			{
				List<string> messageArray = ChunksUpto(message, _maxChar).ToList();
				foreach (string str in messageArray)
				{
					data.Irc.SendMessage(SendType.Message, data.Channel, color + str);
				}
			}
			else 
			{
				data.Irc.SendMessage(SendType.Message, data.Channel, color + message);
			}
		}
		public static void SendText(this IrcMessageData data, string text, params object[] param)
		{
			SendTextReal(data, text, IcyBot.Config.Color, param);
		}
		public static void SendErrorText(this IrcMessageData data, string text, params object[] param)
		{
			SendTextReal(data, text, IcyBot.Config.ErrorColor, param);
		}
		public static void RunCommand(this IrcClient client, string text, params object[] param)
		{
			if (param == null)
				param = new object[0];
			client.WriteLine(string.Format(text, param));
		}

		static IEnumerable<string> ChunksUpto(string str, int maxChunkSize)
		{
			for (int i = 0; i < str.Length; i += maxChunkSize)
				yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
		}
	}
}
