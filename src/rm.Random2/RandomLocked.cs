using System;

namespace rm.Random2
{
	/// <note>
	/// see https://devblogs.microsoft.com/pfxteam/getting-random-numbers-in-a-thread-safe-way/
	/// </note>
	public class RandomLocked
	{
		private readonly Random _random;

		internal RandomLocked(Random random)
		{
			_random = random
				?? throw new ArgumentNullException(nameof(random));
		}

		public int Next(int minValue = 0, int maxValue = int.MaxValue)
		{
			lock (_random)
			{
				return _random.Next(minValue, maxValue);
			}
		}
	}
}
