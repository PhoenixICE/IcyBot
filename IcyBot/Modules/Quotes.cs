using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

using Meebey.SmartIrc4net;

namespace IcyBot.Modules
{
	public class Quotes : IrcPlugin
	{
		public static List<Quote> QuoteList = new List<Quote>();

		public override void Initialize()
		{
			Commands.ChatCommands.Add(new Command(QuotesMain, "quote"));
			LoadQuotes();
		}

		public void QuotesMain(CommandArgs args)
		{
			if (args.Parameters.Count < 2)
			{
				args.Args.Data.SendText("quote add <quote>");
				args.Args.Data.SendText("quote del <id>");
				args.Args.Data.SendText("quote read <id/random/last>");
				return;
			}
			if (args.Parameters[0].ToLower() == "add")
			{
				var _quote = AddQuote(string.Join(" ", args.Parameters.Skip(1)), args.Args.Data.Nick);
				args.Args.Data.SendText(_quote.ToString());
				args.Args.Data.SendText(_quote.ToString2());
				return;
			}
			else if (args.Parameters[0].ToLower() == "del" && false)
			{
				int ID;
				if (!int.TryParse(args.Parameters[1], out ID))
				{
					args.Args.Data.SendText("Invalid Quote ID: {0}", ID);
					return;
				}
				var _quote = QuoteList.FirstOrDefault(x => x.ID == ID);
				if (_quote == null)
				{
					args.Args.Data.SendText("No Quote Found for ID: {0}", ID);
					return;
				}
				QuoteList.Remove(_quote);
				args.Args.Data.SendText("Quote ID {0} has been succesfully deleted!", ID);
				string json = JsonConvert.SerializeObject(QuoteList);

				//write string to file
				System.IO.File.WriteAllText(@"D:\Quotes.txt", json);
				return;
			}
			else if (args.Parameters[0].ToLower() == "find")
			{
				var _quoteList = QuoteList.Where(x => x != null && x.QuoteString.ToLower().Contains(args.Parameters[1].ToLower()));
				if (_quoteList != null)
				{
					if (_quoteList.Count() > 1)
					{
						args.Args.Data.SendText("Quote IDs: " + string.Join(", ", _quoteList.Select(x => x.ID.ToString())));
					}
					else if (_quoteList.Count() != 0)
					{
						args.Args.Data.SendText("Quote IDs: " + _quoteList.ToArray()[0].ToString());
						args.Args.Data.SendText("Quote IDs: " + _quoteList.ToArray()[0].ToString2());
					}
					else
					{
						args.Args.Data.SendText("No Quotes Found with String: {0}", args.Parameters[1]);
					}
				}
				else
				{
					args.Args.Data.SendText("No Quotes Found with String: {0}", args.Parameters[1]);
				}
			}
			else if (args.Parameters[0].ToLower() == "read")
			{
				if (args.Parameters[1].ToLower() == "random")
				{
					var _quote = QuoteList[IcyBot.Rand.Next(0, QuoteList.Count)];
					args.Args.Data.SendText(_quote.ToString());
					args.Args.Data.SendText(_quote.ToString2());
				}			
				else if (args.Parameters[1].ToLower() == "last")
				{
					var _quote = QuoteList[QuoteList.Count - 1];
					args.Args.Data.SendText(_quote.ToString());
					args.Args.Data.SendText(_quote.ToString2());
				}
				else
				{
					int ID;
					if (!int.TryParse(args.Parameters[1], out ID))
					{
						args.Args.Data.SendText("Invalid Quote ID: {0}", ID);
					}
					else
					{
						var _quote = QuoteList.FirstOrDefault(x => x.ID == ID);
						if (_quote == null)
						{
							args.Args.Data.SendText("No Quote Found for ID: {0}", ID);
						}
						else
						{
							args.Args.Data.SendText(_quote.ToString());
							args.Args.Data.SendText(_quote.ToString2());
						}
					}				
				}
			}
		}

		private Quote AddQuote(string quote, string user)
		{
			int id = 1;
			if (QuoteList.Count > 0)
			{
				id = QuoteList[QuoteList.Count - 1].ID + 1;
			}
			var _quote = new Quote(id, DateTime.Now, user, quote);
			QuoteList.Add(_quote);

			string json = JsonConvert.SerializeObject(QuoteList);

			//write string to file
			System.IO.File.WriteAllText(@"D:\Quotes.txt", json);

			return _quote;
		}

		private void LoadQuotes()
		{
			using (StreamReader r = new StreamReader(@"D:\Quotes.txt"))
			{
				string json = r.ReadToEnd();
				QuoteList = JsonConvert.DeserializeObject<List<Quote>>(json);
				if (QuoteList == null)
				{
					QuoteList = new List<Quote>();
				}
			}
		}
	}
}
