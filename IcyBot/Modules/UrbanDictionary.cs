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
				args.Args.Data.SendErrorText("ud [number] <text>");
				return;
			}

			int _number = 0;

			string word = string.Join(" ", args.Parameters);

			if (int.TryParse(args.Parameters[0], out _number))
			{
				args.Parameters.RemoveAt(0);
				word = string.Join(" ", args.Parameters);
			}

			var req = HttpWebRequest.Create(UrlRandom);
			var res = req.GetResponse();
			var reader = new StreamReader(res.GetResponseStream());
			var obj = JObject.ReadFrom(new JsonTextReader(reader));

			req = HttpWebRequest.Create(string.Format(UrlDef, HttpUtility.UrlEncode(word)));
			res = req.GetResponse();
			reader = new StreamReader(res.GetResponseStream());
			obj = JObject.ReadFrom(new JsonTextReader(reader));
			JArray _defList = ((JArray)obj["list"]);
			if (_defList.Count != 0)
			{
				if (_number == 0)
				{
					for (int i = 0; i < _defList.Count; i++)
					{
						string definition = ((string)((JObject)_defList[i]).Property("definition").Value).Trim();
						if (definition.Length > 203)
						{
							definition = definition.Substring(0, 203) + "...";
						}
						if (!string.IsNullOrWhiteSpace(definition))
							args.Args.Data.SendText(":: [{0}/{1}] {2} :: {3} ::", i + 1, _defList.Count, word, definition);
						if (i > 1)
						{
							break;
						}
					}
					args.Args.Data.SendText("To view a single definition with a related example, type: {0}ud [def_number] {1}", IcyBot.Config.CommandSpecifier, word);
				}
				else
				{
					if (_number - 1 > _defList.Count)
					{
						args.Args.Data.SendErrorText("No definition found");
						return;
					}
					else
					{
						args.Args.Data.SendText(_number + ": " + (string)((JObject)_defList[_number - 1]).Property("definition").Value);
					}
				}
			}
			else
			{
				args.Args.Data.SendErrorText("No definition found");
			}	
		}
	}
}
