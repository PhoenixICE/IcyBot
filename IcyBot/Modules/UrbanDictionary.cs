using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Meebey.SmartIrc4net;

namespace IcyBot.Modules
{
	public class UrbanDictionary : IrcPlugin
	{
		private const string UrlRandom = "http://api.urbandictionary.com/v0/random";
		private const string UrlDef = "http://api.urbandictionary.com/v0/define?term={0}";

		public override void Initialize()
		{
			Commands.ChatCommands.Add(new Command(UrbanDictionaryMain, "ud"));
		}

		public void UrbanDictionaryMain(CommandArgs args)
		{					
			if (args.Parameters.Count == 0)
			{
				args.Args.Data.SendErrorText("ud <text>");
				return;
			}
			string inputstring = string.Join(" ", args.Parameters);
			args.Args.Data.SendText(UrbanDictionaryMethod(inputstring));
		}

		public string UrbanDictionaryMethod(string word)
		{
			var req = HttpWebRequest.Create(UrlRandom);
			var res = req.GetResponse();
			var reader = new StreamReader(res.GetResponseStream());
			var obj = JObject.ReadFrom(new JsonTextReader(reader));

			req = HttpWebRequest.Create(string.Format(UrlDef, HttpUtility.UrlEncode(word)));
			res = req.GetResponse();
			reader = new StreamReader(res.GetResponseStream());
			obj = JObject.ReadFrom(new JsonTextReader(reader));

			foreach (var i in (JArray)obj["list"])
			{
				return (string)((JObject)i).Property("definition").Value;
			}
			return "No definition found";
		}
	}
}
