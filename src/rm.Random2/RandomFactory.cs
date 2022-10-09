using System;

namespace rm.Random2
{
	/// <note>
	/// see https://csharpindepth.com/Articles/Random
	/// </note>
	public static class RandomFactory
	{
		private static readonly Random random = new Random();
		private static readonly LockRandom lockRandom = new LockRandom();
		private static readonly ThreadStaticRandom threadStaticRandom = new ThreadStaticRandom();
		private static readonly ThreadLocalRandom threadLocalRandom = new ThreadLocalRandom();

		public static Random GetRandom()
		{
			return random;
		}

		public static LockRandom GetLockRandom()
		{
			return lockRandom;
		}

		public static ThreadStaticRandom GetThreadStaticRandom()
		{
			return threadStaticRandom;
		}

		public static ThreadLocalRandom GetThreadLocalRandom()
		{
			return threadLocalRandom;
		}
	}
}
