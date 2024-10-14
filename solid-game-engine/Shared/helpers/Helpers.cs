using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using solid_game_engine.Shared.entity;

namespace solid_game_engine.Shared.helpers
{
    public static class Helpers
	{
		public static T Clone<T>(this T obj)
		{
			return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
		}
		public static bool InRange(this float number, float min, float max)
		{
			return number > min && number < max;
		}

		public static bool InRange(int number, int min, int max)
		{
			return number > min && number < max;
		}
		
		public static List<Option> AddOption(this List<Option> list, string name, Action action)
		{
			list.Add(new Option(name, list.Count + 1, action));
			return list;
		}

		public static List<Option> AddOption(this List<Option> list, string name, Action action, Action intercept)
		{
			Action newActionIntercept = ()=>{
				intercept();
				action();
			};
			list.Add(new Option(name, list.Count + 1, newActionIntercept));
			return list;
		}
	}
}