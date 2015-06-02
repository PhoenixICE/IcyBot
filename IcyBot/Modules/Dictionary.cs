using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meebey.SmartIrc4net;

namespace IcyBot.Modules
{
	class Dictionary : IrcPlugin
	{
		public override void Initialize()
		{
			//Commands.ChatCommands.Add(new Command(DictMain, "dict"));
		}

		/*private void DictMain(CommandArgs args)
		{
			if (args.Parameters.Count == 0)
			{
				args.Args.Data.SendText("dict <word>");
				return;
			}
			string word = string.Join(" ", args.Parameters);
			DictService svc = new DictService();
			WordDefinition wd = svc.Define(word);
			if (wd.Definitions.Length == 0)
			{
				args.Args.Data.SendText("No definitions for {0} found.", word));
				return;
			}
			foreach (Definition d in wd.Definitions)
			{
				args.Args.Data.SendText("From {0}:", d.Dictionary.Name));
				args.Args.Data.SendText(d.WordDefinition);
			}
		}*/
	}
}
