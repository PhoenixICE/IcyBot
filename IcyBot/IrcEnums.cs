using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcyBot
{
	public static class ColorCode
	{
		public static string White = "\u000300";   /**< White */
		public static string Black = "\u000301";   /**< Black */
		public static string DarkBlue = "\u000302";   /**< Dark blue */
		public static string DarkGreen = "\u000303";   /**< Dark green */
		public static string Red = "\u000304";   /**< Red */
		public static string DarkRed = "\u000305";   /**< Dark red */
		public static string DarkViolet = "\u000306";   /**< Dark violet */
		public static string Orange = "\u000307";   /**< Orange */
		public static string Yellow = "\u000308";   /**< Yellow */
		public static string LightGreen = "\u000309";   /**< Light green */
		public static string Cyan = "\u0003010";   /**< Cornflower blue */
		public static string LightCyan = "\u0003011";   /**< Light blue */
		public static string Blue = "\u0003012";   /**< Blue */
		public static string Violet = "\u0003013";   /**< Violet */
		public static string DarkGray = "\u0003014";   /**< Dark gray */
		public static string LightGray = "\u0003015";   /**< Light gray */
	}

	public static class ControlCode
	{
		public static string Bold = "\x02";     /**< Bold */
		public static string Color = "\x03";     /**< Color */
		public static string Italic = "\x09";     /**< Italic */
		public static string StrikeThrough = "\x13";     /**< Strike-Through */
		public static string Reset = "\x0f";     /**< Reset */
		public static string Underline = "\x15";     /**< Underline */
		public static string Underline2 = "\x1f";     /**< Underline */
		public static string Reverse = "\x16";      /**< Reverse */
	}
}
