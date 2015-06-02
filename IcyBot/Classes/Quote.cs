using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IcyBot
{
	public class Quote
	{
		public int ID { get; set; }
		public DateTime DateAdded { get; set; }
		public string UserAdded { get; set; }
		public string QuoteString { get; set; }
		public Quote(int id, DateTime dateadded, string useradded, string quotestring)
		{
			ID = id;
			DateAdded = dateadded;
			UserAdded = useradded;
			QuoteString = quotestring;
		}
		public override string ToString()
		{
			return string.Format("[Quote] #{0} added by {1} {2} ago.", ID, UserAdded, GetElapsedTime(DateAdded, DateTime.Now));
		}
		public string ToString2()
		{
			return string.Format("[Quote] {0}", QuoteString);
		}

		public string GetElapsedTime(DateTime from_date, DateTime to_date)
		{
			// If from_date > to_date, switch them around.
			int years = 0;
			int months = 0;
			int days = 0;
			int hours = 0;
			int minutes = 0;
			int seconds = 0;
			int milliseconds = 0;
			if (from_date > to_date)
			{
				GetElapsedTime(to_date, from_date);
			}
			else
			{
				// Handle the years.
				years = to_date.Year - from_date.Year;

				// See if we went too far.
				DateTime test_date = from_date.AddMonths(12 * years);
				if (test_date > to_date)
				{
					years--;
					test_date = from_date.AddMonths(12 * years);
				}

				// Add months until we go too far.
				months = 0;
				while (test_date <= to_date)
				{
					months++;
					test_date = from_date.AddMonths(12 * years + months);
				}
				months--;

				// Subtract to see how many more days,
				// hours, minutes, etc. we need.
				from_date = from_date.AddMonths(12 * years + months);
				TimeSpan remainder = to_date - from_date;
				days = remainder.Days;
				hours = remainder.Hours;
				minutes = remainder.Minutes;
				seconds = remainder.Seconds;
				milliseconds = remainder.Milliseconds;
			}

			if (years == 0 && months == 0 && days == 0 && hours == 0 && minutes == 0 && seconds == 0)
			{
				return "1 Second";
			}
			else
				return string.Format("{0}{1}{2}{3}{4}{5}", (years > 0 ? (years > 1 ? years + " Years " : years + " Year ") : ""), (months > 0 ? (months > 1 ? months + " Months " : months + " Month ") : ""),
					(days > 0 ? (days > 1 ? days + " Days " : days + " Day ") : ""), (hours > 0 ? (hours > 1 ? hours + " Hours " : hours + " Hour ") : ""),
					(minutes > 0 ? (minutes > 1 ? minutes + " Minutes " : minutes + " Minute ") : ""), (seconds > 0 ? (seconds > 1 ? seconds + " Seconds " : seconds + " Second ") : "")).Trim(' ');
		}
	}
}
