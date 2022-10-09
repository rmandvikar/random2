using System;

namespace rm.Random2
{
	/// <note>
	/// see https://devblogs.microsoft.com/pfxteam/getting-random-numbers-in-a-thread-safe-way/
	/// </note>
	public class LockRandom : Random
	{
		private readonly object locker = new object();

		internal LockRandom()
			: base()
		{ }

		internal LockRandom(int seed)
			: base(seed)
		{ }

		public override int Next()
		{
			lock (locker)
			{
				return base.Next();
			}
		}

		public override int Next(int maxValue)
		{
			lock (locker)
			{
				return base.Next(maxValue);
			}
		}

		public override int Next(int minValue, int maxValue)
		{
			lock (locker)
			{
				return base.Next(minValue, maxValue);
			}
		}

		public override void NextBytes(byte[] buffer)
		{
			lock (locker)
			{
				base.NextBytes(buffer);
			}
		}

		public override double NextDouble()
		{
			lock (locker)
			{
				return base.NextDouble();
			}
		}

		protected override double Sample()
		{
			lock (locker)
			{
				return base.Sample();
			}
		}
	}
}
