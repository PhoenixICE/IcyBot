using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Meebey.SmartIrc4net;
using System.Net;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace IcyBot.Modules
{
	public class Anime : IrcPlugin
	{
		public readonly string MalAppInfoUri = "http://myanimelist.net/api/anime|manga/search.xml";

		public string UserAgent = "MalApiExample";

		private int m_timeoutInMs = 15 * 1000;

		public int TimeoutInMs { get { return m_timeoutInMs; } set { m_timeoutInMs = value; } }

		public override void Initialize()
		{
			Commands.ChatCommands.Add(new Command(AnimeMain, "anime"));
		}

		private void AnimeMain(CommandArgs args)
		{
			if (args.Parameters.Count == 0)
			{
				args.Args.Data.SendText("anime <name>");
				return;
			}

			GetAnime(string.Join(" ", args.Parameters), args.Args.Data);
		}

		public void GetAnime(string search, IrcMessageData data)
		{
			search = WebUtility.HtmlEncode(search);
			string sUrl = "http://myanimelist.net/api/anime/search.xml?q=" + search;
			HttpWebRequest MALInfo = (HttpWebRequest)HttpWebRequest.Create(sUrl);
			MALInfo.AutomaticDecompression = DecompressionMethods.GZip;
			MALInfo.Credentials = new NetworkCredential(IcyBot.Config.MalUsername, IcyBot.Config.MalPassword);
			MALInfo.UserAgent = "";

			Stream ObjStream = MALInfo.GetResponse().GetResponseStream();

			using (StreamReader reader = new StreamReader(ObjStream))
			{
				string _str = reader.ReadToEnd();
				_str = XmlConvert.DecodeName(_str).Replace(@"&mdash;", "").Replace(@"mdash;", "");
				if (!string.IsNullOrWhiteSpace(_str))
				{
					var _xml = MalAppInfoXml.Parse(_str);
					int count = _xml.AnimeList.Count;
					if (count == 0)
					{
						data.SendErrorText("No Anime found for name: {0}", search);
					}
					else if (count > 1)
					{
						data.SendText("More then one match found: {0}", string.Join(", ", _xml.AnimeList.Select(x => x.AnimeInfo.Title)));
					}
					var animeinfo = _xml.AnimeList.ElementAt(0).AnimeInfo;
					data.SendText("Title: {0} Episodes: {1} Type: {2} Status: {3} Image: {4}", animeinfo.Title, animeinfo.NumEpisodes, animeinfo.Type, animeinfo.Status, animeinfo.ImageUrl);
					data.SendText("{0}", animeinfo.Synopsis);
				}
				else
				{
					data.SendErrorText("No Anime found for name: {0}", search);
				}
			}
		}

		public static class MalAppInfoXml
		{
			/// <summary>
			/// Parses XML obtained from malappinfo.php. The XML is sanitized to account for MAL's invalid XML if, for example,
			/// a user has a &amp; character in their tags.
			/// </summary>
			/// <param name="xmlTextReader"></param>
			/// <returns></returns>
			/// <exception cref="MalApi.MalUserNotFoundException"></exception>
			/// <exception cref="MalApi.MalApiException"></exception>
			public static MalUserLookupResults Parse(string xmlText)
			{
				using (var xmlTextReader = SanitizeAnimeListXml(xmlText))
				{
					XDocument doc = XDocument.Load(xmlTextReader);
					return Parse(doc);
				}
			}

			// Rumor has it that compiled regexes are far more performant than non-compiled regexes on large pieces of text.
			// I haven't profiled it though.
			private static Lazy<Regex> s_tagElementContentsRegex =
				new Lazy<Regex>(() => new Regex("<my_tags>(?<TagText>.*?)</my_tags>", RegexOptions.Compiled | RegexOptions.CultureInvariant));
			private static Regex TagElementContentsRegex { get { return s_tagElementContentsRegex.Value; } }

			private static Lazy<Regex> s_nonEntityAmpersandRegex =
				new Lazy<Regex>(() => new Regex("&(?!lt;)(?!gt;)(?!amp;)(?!apos;)(?!quot;)(?!#x[0-9a-fA-f]+;)(?!#[0-9]+;)", RegexOptions.Compiled | RegexOptions.CultureInvariant));
			private static Regex NonEntityAmpersandRegex { get { return s_nonEntityAmpersandRegex.Value; } }

			// Remove any code points not in: U+0009, U+000A, U+000D, U+0020–U+D7FF, U+E000–U+FFFD (see http://en.wikipedia.org/wiki/Xml)
			private static Lazy<Regex> s_invalidXmlCharacterRegex =
				new Lazy<Regex>(() => new Regex("[^\\u0009\\u000A\\u000D\\u0020-\\uD7FF\\uE000-\\uFFFD]", RegexOptions.Compiled | RegexOptions.CultureInvariant));
			private static Regex InvalidXmlCharacterRegex { get { return s_invalidXmlCharacterRegex.Value; } }

			// Replace & with &amp; only if the & is not part of &lt; &gt; &amp; &apos; &quot; &#x<hex digits>; &#<decimal digits>;
			private static MatchEvaluator TagElementContentsReplacer = (Match match) =>
			{
				string tagText = match.Groups["TagText"].Value;
				string replacementTagText = NonEntityAmpersandRegex.Replace(tagText, "&amp;");
				replacementTagText = InvalidXmlCharacterRegex.Replace(replacementTagText, "");
				return "<my_tags>" + replacementTagText + "</my_tags>";
			};

			/// <summary>
			/// Sanitizes anime list XML which is not always well-formed. If a user uses &amp; characters in their tags,
			/// they will not be escaped in the XML.
			/// </summary>
			/// <param name="xmlTextReader"></param>
			/// <returns></returns>
			public static TextReader SanitizeAnimeListXml(string xmlTextReader)
			{
				string sanitizedXml = TagElementContentsRegex.Replace(xmlTextReader, TagElementContentsReplacer);
				return new StringReader(sanitizedXml);
			}

			/// <summary>
			/// Parses XML obtained from malappinfo.php.
			/// </summary>
			/// <param name="doc"></param>
			/// <returns></returns>
			public static MalUserLookupResults Parse(XDocument doc)
			{
				List<MyAnimeListEntry> entries = new List<MyAnimeListEntry>();

				IEnumerable<XElement> animes = doc.Root.Elements("entry");
				foreach (XElement anime in animes)
				{
					int animeId = GetElementValueInt(anime, "id");
					string title = GetElementValueString(anime, "title");

					string synonymList = GetElementValueString(anime, "synonyms");
					string[] rawSynonyms = synonymList.Split(SynonymSeparator, StringSplitOptions.RemoveEmptyEntries);

					// filter out synonyms that are the same as the main title
					HashSet<string> synonyms = new HashSet<string>(rawSynonyms.Where(synonym => !synonym.Equals(title, StringComparison.Ordinal)));

					string seriesType = GetElementValueString(anime, "type");

					int numEpisodes = GetElementValueInt(anime, "episodes");

					string seriesStatus = GetElementValueString(anime, "status");

					string seriesStartString = GetElementValueString(anime, "start_date");
					UncertainDate seriesStart = UncertainDate.FromMalDateString(seriesStartString);

					string seriesEndString = GetElementValueString(anime, "end_date");
					UncertainDate seriesEnd = UncertainDate.FromMalDateString(seriesEndString);

					string seriesImage = GetElementValueString(anime, "image");

					string seriesSynopsis = GetElementValueString(anime, "synopsis");

					MalAnimeInfoFromUserLookup animeInfo = new MalAnimeInfoFromUserLookup(animeId: animeId, title: title,
						type: seriesType, synonyms: synonyms, status: seriesStatus, numEpisodes: numEpisodes, startDate: seriesStart,
						endDate: seriesEnd, imageUrl: seriesImage, synopsis: seriesSynopsis.Replace("<br /> <br />", ""));					

					MyAnimeListEntry entry = new MyAnimeListEntry(animeInfo: animeInfo);

					entries.Add(entry);
				}

				MalUserLookupResults results = new MalUserLookupResults(animeList: entries);
				return results;
			}

			private static readonly string[] SynonymSeparator = new string[] { "; " };
			private static readonly char[] TagSeparator = new char[] { ',' };

			private static XElement GetExpectedElement(XContainer container, string elementName)
			{
				XElement element = container.Element(elementName);
				if (element == null)
				{
					throw new Exception(string.Format("Did not find element {0}.", elementName));
				}
				return element;
			}

			private static string GetElementValueString(XContainer container, string elementName)
			{
				XElement element = GetExpectedElement(container, elementName);

				try
				{
					return (string)element;
				}
				catch (FormatException ex)
				{
					throw new Exception(string.Format("Unexpected value \"{0}\" for element {1}.", element.Value, elementName), ex);
				}
			}

			private static int GetElementValueInt(XContainer container, string elementName)
			{
				XElement element = GetExpectedElement(container, elementName);

				try
				{
					return (int)element;
				}
				catch (FormatException ex)
				{
					throw new Exception(string.Format("Unexpected value \"{0}\" for element {1}.", element.Value, elementName), ex);
				}
			}

			private static long GetElementValueLong(XContainer container, string elementName)
			{
				XElement element = GetExpectedElement(container, elementName);

				try
				{
					return (long)element;
				}
				catch (FormatException ex)
				{
					throw new Exception(string.Format("Unexpected value \"{0}\" for element {1}.", element.Value, elementName), ex);
				}
			}

			private static decimal GetElementValueDecimal(XContainer container, string elementName)
			{
				XElement element = GetExpectedElement(container, elementName);

				try
				{
					return (decimal)element;
				}
				catch (FormatException ex)
				{
					throw new Exception(string.Format("Unexpected value \"{0}\" for element {1}.", element.Value, elementName), ex);
				}
			}

			public class MalUserLookupResults
			{
				public ICollection<MyAnimeListEntry> AnimeList { get; private set; }


				public MalUserLookupResults(ICollection<MyAnimeListEntry> animeList)
				{
					AnimeList = animeList;
				}
			}

			public class MyAnimeListEntry : IEquatable<MyAnimeListEntry>
			{
				public MalAnimeInfoFromUserLookup AnimeInfo { get; private set; }

				public MyAnimeListEntry(MalAnimeInfoFromUserLookup animeInfo)
				{
					AnimeInfo = animeInfo;
				}

				public bool Equals(MyAnimeListEntry other)
				{
					if (other == null) return false;
					return this.AnimeInfo.AnimeId == other.AnimeInfo.AnimeId;
				}

				public override bool Equals(object obj)
				{
					return Equals(obj as MyAnimeListEntry);
				}

				public override int GetHashCode()
				{
					return AnimeInfo.AnimeId.GetHashCode();
				}

				public override string ToString()
				{
					return AnimeInfo.Title;
				}
			}

			public struct UncertainDate : IEquatable<UncertainDate>
			{
				public int? Year { get; private set; }

				private int? m_month;
				public int? Month
				{
					get { return m_month; }
					set
					{
						if (value < 1 || value > 12)
						{
							throw new ArgumentOutOfRangeException(string.Format("Month cannot be {0}.", value));
						}
						m_month = value;
					}
				}

				private int? m_day;
				public int? Day
				{
					get { return m_day; }
					set
					{
						if (value < 1 || value > 31)
						{
							throw new ArgumentOutOfRangeException(string.Format("Day cannot be {0}.", value));
						}
						m_day = value;
					}
				}

				public UncertainDate(int? year, int? month, int? day)
					: this()
				{
					Year = year;
					Month = month;
					Day = day;
				}

				public static UncertainDate Unknown { get { return new UncertainDate(); } }

				/// <summary>
				/// Creates an UncertainDate from the format MAL uses in its XML, YYYY-MM-DD with 00 or 0000 indicating an unknown.
				/// 2005-07-00 would indicate July 2005.
				/// </summary>
				/// <param name="malDateString"></param>
				/// <returns></returns>
				public static UncertainDate FromMalDateString(string malDateString)
				{
					string[] yearMonthDay = malDateString.Split('-');
					if (yearMonthDay.Length != 3)
					{
						throw new FormatException(string.Format("{0} is not in YYYY-MM-DD format.", malDateString));
					}

					int? year = int.Parse(yearMonthDay[0]);
					if (year == 0) year = null;

					int? month = int.Parse(yearMonthDay[1]);
					if (month == 0) month = null;

					int? day = int.Parse(yearMonthDay[2]);
					if (day == 0) day = null;

					return new UncertainDate(year: year, month: month, day: day);
				}

				public bool Equals(UncertainDate other)
				{
					return this.Year == other.Year && this.Month == other.Month && this.Day == other.Day;
				}

				public override bool Equals(object obj)
				{
					if (obj is UncertainDate)
						return Equals((UncertainDate)obj);
					else
						return false;
				}

				public override int GetHashCode()
				{
					unchecked
					{
						int hash = 23;
						if (Year != null) hash = hash * 17 + Year.Value;
						if (Month != null) hash = hash * 17 + Month.Value;
						if (Day != null) hash = hash * 17 + Day.Value;
						return hash;
					}
				}

				public override string ToString()
				{
					string year;
					string month;
					string day;

					if (Year == null)
						year = "0000";
					else
						year = Year.Value.ToString("D4");

					if (Month == null)
						month = "00";
					else
						month = Month.Value.ToString("D2");

					if (Day == null)
						day = "00";
					else
						day = Day.Value.ToString("D2");

					return string.Format("{0}-{1}-{2}", year, month, day);
				}
			}

			public class MalAnimeInfoFromUserLookup : IEquatable<MalAnimeInfoFromUserLookup>
			{
				public int AnimeId { get; private set; }
				public string Title { get; private set; }

				/// <summary>
				/// Could be something other than the enumerated values if MAL adds new types!
				/// </summary>
				public string Type { get; private set; }

				public ICollection<string> Synonyms { get; private set; }
				public string Status { get; private set; }

				/// <summary>
				/// Could be 0 for anime that hasn't aired yet or less than the planned number of episodes for a series currently airing.
				/// </summary>
				public int NumEpisodes { get; private set; }

				public UncertainDate StartDate { get; private set; }
				public UncertainDate EndDate { get; private set; }
				public string ImageUrl { get; private set; }
				public string Synopsis { get; private set; }

				public MalAnimeInfoFromUserLookup(int animeId, string title, string type, ICollection<string> synonyms, string status,
					int numEpisodes, UncertainDate startDate, UncertainDate endDate, string imageUrl, string synopsis)
				{
					AnimeId = animeId;
					Title = title;
					Type = type;
					Synonyms = synonyms;
					Status = status;
					NumEpisodes = numEpisodes;
					StartDate = startDate;
					EndDate = endDate;
					ImageUrl = imageUrl;
					Synopsis = synopsis;
				}

				public override bool Equals(object obj)
				{
					return Equals(obj as MalAnimeInfoFromUserLookup);
				}

				public bool Equals(MalAnimeInfoFromUserLookup other)
				{
					if (other == null) return false;
					return this.AnimeId == other.AnimeId;
				}

				public override int GetHashCode()
				{
					return AnimeId.GetHashCode();
				}

				public override string ToString()
				{
					return Title;
				}
			}
		}
	}
}
