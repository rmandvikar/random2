using System;
using System.Security.Cryptography;

namespace rm.Random2
{
	/// <note>
	/// see https://csharpindepth.com/Articles/Random
	/// </note>
	public static class RandomFactory
	{
		private static readonly Random _random = new Random();
		private static readonly RandomLocked _randomLocked = new RandomLocked(_random);
		private static readonly RNGCryptoServiceProvider _rngCrypto = new RNGCryptoServiceProvider();
		private static readonly RandomPerThread _randomPerThread = new RandomPerThread(_rngCrypto);

		public static Random GetRandom()
		{
			return _random;
		}

		public static RandomLocked GetRandomLocked()
		{
			return _randomLocked;
		}

		public static RandomPerThread GetRandomLockedPerThread()
		{
			return _randomPerThread;
		}
	}
}
