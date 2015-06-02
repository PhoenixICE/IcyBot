﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IcyBot.Modules;
using System.Reflection;

namespace IcyBot
{
	public static class LoadModules
	{
		public static void Load()
		{
			//Admin.OnInitialize();
			//Find.OnInitialize();
			//Quiz.OnInitialize();
			//Quotes.OnInitialize();
			//UrbanDictionary.OnInitialize();
			//WorldBossDungeonInfo.OnInitialize();
			//JsonPhraser.OnInitialize();
			//Translate.OnInitialize();
			//WolframAlpha.OnInitialize();
			Commands.HelpCommand();

			foreach(var obj in ReflectiveEnumerator.GetEnumerableOfType<IrcPlugin>())
			{
				obj.Initialize();
			}
			
		}
		public static class ReflectiveEnumerator
		{
			static ReflectiveEnumerator() { }

			public static IEnumerable<T> GetEnumerableOfType<T>(params object[] constructorArgs) where T : class
			{
				List<T> objects = new List<T>();
				foreach (Type type in
					Assembly.GetAssembly(typeof(T)).GetTypes()
					.Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
				{
					objects.Add((T)Activator.CreateInstance(type, constructorArgs));
				}
				return objects;
			}
		}
	}
}