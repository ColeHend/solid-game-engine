using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
	}
}