using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net;
using System.Xml.Linq;
using System.Xml;
using Meebey.SmartIrc4net;

namespace IcyBot.Modules
{
	public class WolframAlpha : IrcPlugin
	{
		public override void Initialize()
		{
			Commands.ChatCommands.Add(new Command(get_wa, "calc"));
		}

		private void get_wa(CommandArgs args)
		{
			if (args.Parameters.Count == 0)
			{
				args.Args.Data.SendErrorText("calc <query>");
				return;
			}
			string search = string.Join(" ", args.Parameters);
			string URL = "http://api.wolframalpha.com/v2/query?input=" + System.Web.HttpUtility.UrlEncode(search) + "&appid=" + "R7X37H-U8H66PR83G" + "&format=plaintext";
			XmlNodeList xnList = null;
			try
			{
				WebClient web = new WebClient();
				web.Encoding = Encoding.UTF8;
				string results = web.DownloadString(URL);
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.LoadXml(results);
				xnList = xmlDoc.SelectNodes("/queryresult/pod");
			}
			catch
			{
				args.Args.Data.SendErrorText("Could not fetch results");
			}
			if (xnList.Count > 1)
			{
				args.Args.Data.SendText("Query: " + xnList[0]["subpod"]["plaintext"].InnerText + " Answer: " + xnList[1]["subpod"]["plaintext"].InnerText);
			}
			else
			{
				args.Args.Data.SendErrorText("No Results Found.");
			}
		}
	}
}
