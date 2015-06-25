using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;
using Meebey.SmartIrc4net;
using System.IO;
using ChuckNorris;

namespace IcyBot.Modules
{
	public class Joke : IrcPlugin
	{
		private List<string> Urls = new List<string>()
		{
			"http://www.randomjoke.com/topic/riddles.php",
			"http://www.randomjoke.com/topic/oneliners.php"
		};

		public override void Initialize()
		{
			Commands.ChatCommands.Add(new Command(JokeMain, "joke"));
			Commands.ChatCommands.Add(new Command(ChuckNorrisMain, "chucknorris"));
		}

		private void ChuckNorrisMain(CommandArgs args)
		{			
			var response = ChuckNorris.API.Random(exclude: new string[] { "explicit" });
			var joke = response.Result;

			args.Args.Data.SendText(joke.Text);
		}

		private void JokeMain(CommandArgs args)
		{
			string sUrl = "http://www.randomjoke.com/topic/riddles.php";
			HttpWebRequest JokeInfo = (HttpWebRequest)HttpWebRequest.Create(sUrl);
			JokeInfo.AutomaticDecompression = DecompressionMethods.GZip;
			Stream ObjStream = JokeInfo.GetResponse().GetResponseStream();

			using (StreamReader reader = new StreamReader(ObjStream))
			{
				string _str = reader.ReadToEnd();
				if (!string.IsNullOrWhiteSpace(_str))
				{
					string _search = "ISMAP ALT=\"next joke|back to topic list\"></P>\n<P>\n";
					int _startPos = _str.IndexOf(_search);
					int _endPos = _str.IndexOf("\n<CENTER>\n<div align=\"center\">\n<p></p>\n");
					string _joke = _str.Substring(_startPos + _search.Count(), (_endPos - _startPos - _search.Count()));
					_joke = Regex.Replace(_joke, @"\<.*\>", string.Empty);
					args.Args.Data.SendText(_joke);
					return;
				}
			}
		}
	}
}
