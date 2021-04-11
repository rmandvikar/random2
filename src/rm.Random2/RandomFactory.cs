using System;

namespace rm.Random2
{
	/// <note>
	/// see https://csharpindepth.com/Articles/Random
	/// </note>
	public static class RandomFactory
	{
		private static readonly Random _random = new Random();
		private static readonly LockRandom _lockRandom = new LockRandom();
		private static readonly ThreadStaticRandom _threadStaticRandom = new ThreadStaticRandom();

		public static Random GetRandom()
		{
			return _random;
		}

		public static LockRandom GetLockRandom()
		{
			return _lockRandom;
		}

		public static ThreadStaticRandom GetThreadStaticRandom()
		{
			return _threadStaticRandom;
		}
	}
}
