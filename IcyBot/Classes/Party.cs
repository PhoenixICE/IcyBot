using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IcyBot
{
	public class Party
	{
		public Information FrontLine { get; set; }
		public Information MiddleLine { get; set; }
		public Information BackLine { get; set; }
		public Party()
		{
			FrontLine = new Information();
			MiddleLine = new Information();
			BackLine = new Information();
		}
	}

	public class PlayerParty
	{
		public List<Party> PvP { get; set; }
		public List<Party> PvE { get; set; }
		public List<Party> WB { get; set; }
		public PlayerParty()
		{
			PvP = new List<Party>();
			PvE = new List<Party>();
			WB = new List<Party>();
		}
	}
}
