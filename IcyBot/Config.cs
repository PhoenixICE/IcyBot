using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace IcyBot
{
	public class Config
	{
		public Dictionary<string, int> MaxChars = new Dictionary<string, int>();
		public string CommandSpecifier { get; set; }
		public string SavePath { get; set; }
		public string fileName { get; set; }
		public string Color { get; set; }
		public string ErrorColor { get; set; }
		public List<ConnectInfo> ircConnectionInfo { get; set; }

		public static Config Read(string path)
		{
			if (!File.Exists(path))
				return new Config();
			using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				return Read(fs);
			}
		}
		public static Config Read(Stream stream)
		{
			using (var sr = new StreamReader(stream))
			{
				var cf = JsonConvert.DeserializeObject<Config>(sr.ReadToEnd());
				if (ConfigRead != null)
					ConfigRead(cf);
				return cf;
			}
		}

		public void Write(string path)
		{
			using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
			{
				Write(fs);
			}
		}

		public void Write(Stream stream)
		{
			var str = JsonConvert.SerializeObject(this, Formatting.Indented);
			using (var sw = new StreamWriter(stream))
			{
				sw.Write(str);
			}
		}
		public static Action<Config> ConfigRead;
	}

	public class ConnectInfo
	{
		public string ServerName { get; set; }
		public List<string> Channels { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public int SendDelay { get; set; }
		public bool Ghost { get; set; }
		public int MaxChar { get; set; }
		public ConnectInfo(string server, List<string> channels, string name, string pass, int senddelay = 200, bool ghost = false, int maxChar = -1)
		{
			Ghost = ghost;
			ServerName = server;
			Channels = channels;
			Username = name;
			Password = pass;
			MaxChar = maxChar;
			SendDelay = senddelay;
		}
	}
}
