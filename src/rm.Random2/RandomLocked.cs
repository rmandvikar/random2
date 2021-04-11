using System;

namespace rm.Random2
{
	/// <note>
	/// see https://devblogs.microsoft.com/pfxteam/getting-random-numbers-in-a-thread-safe-way/
	/// </note>
	public class RandomLocked : Random
	{
		private readonly object _lock = new object();

		internal RandomLocked()
			: base()
		{ }

		internal RandomLocked(int seed)
			: base(seed)
		{ }

		public override int Next()
		{
			lock (_lock)
			{
				return base.Next();
			}
		}

		public override int Next(int maxValue)
		{
			lock (_lock)
			{
				return base.Next(maxValue);
			}
		}

		public override int Next(int minValue, int maxValue)
		{
			lock (_lock)
			{
				return base.Next(minValue, maxValue);
			}
		}

		public override void NextBytes(byte[] buffer)
		{
			lock (_lock)
			{
				base.NextBytes(buffer);
			}
		}

		public override double NextDouble()
		{
			lock (_lock)
			{
				return base.NextDouble();
			}
		}

		protected override double Sample()
		{
			lock (_lock)
			{
				return base.Sample();
			}
		}
	}
}
