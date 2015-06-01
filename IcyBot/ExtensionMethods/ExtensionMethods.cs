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
			var CInfo = IcyBot.Config.ircConnectionInfo.FirstOrDefault(x => x.ServerName.ToLower() == data.Irc.Address.ToLower());
			if (CInfo != null)
			{
				if (!CInfo.UseColors)
					color = string.Empty;

				if (CInfo.MaxChar != -1)
				{
					List<string> messageArray = ChunksUpto(message, CInfo.MaxChar).ToList();
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
			else
			{
				Console.WriteLine("Error: Connect locate config for this server!");
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
