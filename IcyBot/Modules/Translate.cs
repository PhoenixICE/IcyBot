using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using Meebey.SmartIrc4net;
namespace IcyBot.Modules
{
	public class Translate : IrcPlugin
	{
		public override void Initialize()
		{
			Commands.ChatCommands.Add(new Command(TranslateMain, "translate"));
		}
		public void TranslateMain(CommandArgs args)
		{
			if (args.Parameters.Count == 0)
			{
				args.Args.Data.SendErrorText("translate <text>");
				return;
			}
			args.Client.SendMessage(SendType.Message, args.Args.Data.Channel, TranslateText(string.Join(" ", args.Parameters)));
		}
		public string TranslateText(string content)
		{
			// Set the From and To language
			string fromLanguage = "Auto";
			string toLanguage = "English";

			// Create a Language mapping
			var languageMap = new Dictionary<string, string>();
			InitLanguageMap(languageMap);

			// Create an instance of WebClient in order to make the language translation
			Uri address = new Uri("https://translate.google.com/#auto/en/");
			WebClient wc = new WebClient();

			/// Async Upload to the specified source i.e http://translate.google.com/translate_t for handling the translation.
			string result = wc.UploadString(address, GetPostData(languageMap[fromLanguage], languageMap[toLanguage], content));
			var doc = new HtmlDocument();
			doc.LoadHtml(result);
			var node = doc.DocumentNode.SelectSingleNode("//span[@id='result_box']");
			var output = node != null ? node.InnerText : result;
			return output;
		}

		/// <summary>
		/// Initialize Language Mapping, Key value pair of Language Name, Language Code
		/// </summary>
		/// <param name="languageMap"></param>
		void InitLanguageMap(Dictionary<string, string> languageMap)
		{
			languageMap.Add("Auto", "auto");
			languageMap.Add("Afrikaans", "af");
			languageMap.Add("Albanian", "sq");
			languageMap.Add("Arabic", "ar");
			languageMap.Add("Armenian", "hy");
			languageMap.Add("Azerbaijani", "az");
			languageMap.Add("Basque", "eu");
			languageMap.Add("Belarusian", "be");
			languageMap.Add("Bengali", "bn");
			languageMap.Add("Bulgarian", "bg");
			languageMap.Add("Catalan", "ca");
			languageMap.Add("Chinese", "zh-CN");
			languageMap.Add("Croatian", "hr");
			languageMap.Add("Czech", "cs");
			languageMap.Add("Danish", "da");
			languageMap.Add("Dutch", "nl");
			languageMap.Add("English", "en");
			languageMap.Add("Esperanto", "eo");
			languageMap.Add("Estonian", "et");
			languageMap.Add("Filipino", "tl");
			languageMap.Add("Finnish", "fi");
			languageMap.Add("French", "fr");
			languageMap.Add("Galician", "gl");
			languageMap.Add("German", "de");
			languageMap.Add("Georgian", "ka");
			languageMap.Add("Greek", "el");
			languageMap.Add("Haitian Creole", "ht");
			languageMap.Add("Hebrew", "iw");
			languageMap.Add("Hindi", "hi");
			languageMap.Add("Hungarian", "hu");
			languageMap.Add("Icelandic", "is");
			languageMap.Add("Indonesian", "id");
			languageMap.Add("Irish", "ga");
			languageMap.Add("Italian", "it");
			languageMap.Add("Japanese", "ja");
			languageMap.Add("Korean", "ko");
			languageMap.Add("Lao", "lo");
			languageMap.Add("Latin", "la");
			languageMap.Add("Latvian", "lv");
			languageMap.Add("Lithuanian", "lt");
			languageMap.Add("Macedonian", "mk");
			languageMap.Add("Malay", "ms");
			languageMap.Add("Maltese", "mt");
			languageMap.Add("Norwegian", "no");
			languageMap.Add("Persian", "fa");
			languageMap.Add("Polish", "pl");
			languageMap.Add("Portuguese", "pt");
			languageMap.Add("Romanian", "ro");
			languageMap.Add("Russian", "ru");
			languageMap.Add("Serbian", "sr");
			languageMap.Add("Slovak", "sk");
			languageMap.Add("Slovenian", "sl");
			languageMap.Add("Spanish", "es");
			languageMap.Add("Swahili", "sw");
			languageMap.Add("Swedish", "sv");
			languageMap.Add("Tamil", "ta");
			languageMap.Add("Telugu", "te");
			languageMap.Add("Thai", "th");
			languageMap.Add("Turkish", "tr");
			languageMap.Add("Ukrainian", "uk");
			languageMap.Add("Urdu", "ur");
			languageMap.Add("Vietnamese", "vi");
			languageMap.Add("Welsh", "cy");
			languageMap.Add("Yiddish", "yi");
		}

		/// <summary>
		/// Construct the Post data required for Google Translation
		/// </summary>
		/// <param name="fromLanguage"></param>
		/// <param name="toLanguage"></param>
		/// <returns></returns>
		string GetPostData(string fromLanguage, string toLanguage, string content)
		{
			// Set the language translation. All we need is the language pair, from and to.
			string strPostData = string.Format("hl=en&ie=UTF8&oe=UTF8submit=Translate&langpair={0}|{1}",
												 fromLanguage,
												 toLanguage);

			// Encode the content and set the text query string param
			return strPostData += "&text=" + HttpUtility.UrlEncode(content);
		}
	}
}
