using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using Meebey.SmartIrc4net;

namespace IcyBot
{
	public class IcyBot
	{
		private static AutoResetEvent autoEvent = new AutoResetEvent(false);
		public static Random Rand = new Random();
		public static Config Config = new Config();
		private static readonly string filepathconfig = "D:\\IcyBot.Config.txt";

		// this method we will use to analyse queries (also known as private messages)
		public static void OnQueryMessage(object sender, IrcEventArgs e)
		{
			string message = Regex.Replace(e.Data.Message, @"[\x02\x1F\x0F\x16]|\x03(\d\d?(,\d\d?)?)?", String.Empty);
			if (message.StartsWith(IcyBot.Config.CommandSpecifier))
			{
				System.Console.WriteLine("Received: " + e.Data.RawMessage);
				try
				{
					string error = string.Empty;
					Commands.HandleCommand(e, e.Data.Irc, out error);
					if (!string.IsNullOrEmpty(error))
						System.Console.WriteLine(error);
				}
				catch (Exception ex)
				{
					Console.Write("An exeption occurred executing a command.");
					Console.Write(ex.ToString());
				}
			}
		}

		// this method handles when we receive "ERROR" from the IRC server
		public static void OnError(object sender, ErrorEventArgs e)
		{
			System.Console.WriteLine("Error: " + e.ErrorMessage);
		}

		// this method will get all IRC messages
		public static void OnRawMessage(object sender, IrcEventArgs e)
		{
			if (e != null && e.Data != null && e.Data.Message != null)
			if (e.Data.Message.StartsWith(IcyBot.Config.CommandSpecifier))
			{
				System.Console.WriteLine("Received: " + e.Data.RawMessage);
				try
				{
					string error = string.Empty;
					Commands.HandleCommand(e, e.Data.Irc, out error);
					if (!string.IsNullOrEmpty(error))
						e.Data.Irc.SendMessage(SendType.Message, e.Data.Channel, IcyBot.Config.ErrorColor + error);
				}
				catch (Exception ex)
				{
					e.Data.Irc.SendMessage(SendType.Message, e.Data.Channel, IcyBot.Config.ErrorColor + "An exeption occurred executing a command.");
					Console.Write(ex.ToString());
				}
			}
		}

		public static void Main(string[] args)
		{
			LoadConfig();
			LoadModules.Load();
			Thread.CurrentThread.Name = "Main";
			foreach (ConnectInfo cInfo in IcyBot.Config.ircConnectionInfo)
			{
				Task.Factory.StartNew(() =>
				{
					while (true)
					{
						IrcClient irc = new IrcClient();
						// UTF-8 test
						irc.Encoding = System.Text.Encoding.UTF8;

						// wait time between messages, we can set this lower on own irc servers
						irc.SendDelay = 200;

						// we use channel sync, means we can use irc.GetChannel() and so on
						irc.ActiveChannelSyncing = true;
						irc.SupportNonRfc = true;
						// here we connect the events of the API to our written methods
						// most have own event handler types, because they ship different data
						irc.OnQueryMessage += new IrcEventHandler(OnQueryMessage);
						irc.OnError += new ErrorEventHandler(OnError);
						irc.OnRawMessage += new IrcEventHandler(OnRawMessage);
						irc.AutoRejoin = true;
						irc.AutoRelogin = true;
						irc.AutoRetry = true;

						// the server we want to connect to, could be also a simple string
						string serverlist = cInfo.ServerName;

						int port = 6667;
						string[] channel = cInfo.Channels.ToArray();
						try
						{
							// here we try to connect to the server and exceptions get handled
							irc.Connect(serverlist, port);
						}
						catch (ConnectionException e)
						{
							// something went wrong, the reason will be shown
							System.Console.WriteLine("couldn't connect! Reason: " + e.Message);
						}

						try
						{
							// here we logon and register our nickname and so on 
							irc.Login(cInfo.Username, cInfo.Username, 0, cInfo.Username, cInfo.Password);

							// join the channel
							irc.RfcJoin(cInfo.Channels.ToArray());

							if (cInfo.Ghost)
							{
								irc.WriteLine(string.Format("ns ghost {0} {1}", cInfo.Username, cInfo.Password));
								irc.WriteLine(string.Format("ns recover {0} {1}", cInfo.Username, cInfo.Password));
								irc.WriteLine(string.Format("ns release {0} {1}", cInfo.Username, cInfo.Password));
								irc.WriteLine(string.Format("nick {0}", cInfo.Username));
								irc.WriteLine(string.Format("ns id {0}", cInfo.Password));					
							}

							if (cInfo.MaxChar > 0)
							{
								if (IcyBot.Config.MaxChars.Keys.Contains(irc.Address))
								{
									IcyBot.Config.MaxChars[irc.Address] = cInfo.MaxChar;
								}
								else
									IcyBot.Config.MaxChars.Add(irc.Address, cInfo.MaxChar);
							}

							irc.Listen();

							irc.Disconnect();
						}
						catch (ConnectionException e)
						{
							// this exception is handled because Disconnect() can throw a not
							// connected exception
							System.Console.WriteLine("Error occurred! Message: " + e.Message);
							System.Console.WriteLine("Exception: " + e.StackTrace);
						}
						catch (Exception e)
						{
							// this should not happen by just in case we handle it nicely
							System.Console.WriteLine("Error occurred! Message: " + e.Message);
							System.Console.WriteLine("Exception: " + e.StackTrace);
						}

						Console.WriteLine(string.Format("Error - Disconnected from Server {0}, attempting to reconnect!", irc.Address));
						Thread.Sleep(3600000);
					}
				});
			}
			autoEvent.WaitOne();
		}

		private static void LoadConfig()
		{
			try
			{
				if (System.IO.File.Exists(filepathconfig))
				{
					Config = new Config();
					Config = Config.Read(filepathconfig);
					return;
				}
				else
				{
					Config.Write(filepathconfig);
					return;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return;
			}
		}
	}
}
