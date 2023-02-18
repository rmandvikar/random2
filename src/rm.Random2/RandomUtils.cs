using System;
using System.Security.Cryptography;

namespace rm.Random2;

public static class RandomUtils
{
#if !NET6_0_OR_GREATER
	/// <summary>
	/// Random number generator used to generate seeds,
	/// which are then used to create new random number
	/// generators on a per-thread basis.
	/// </summary>
	private static readonly RNGCryptoServiceProvider rngCrypto = new();
#endif

	/// <summary>
	/// Creates a new instance of Random. The seed is derived
	/// from a global (static) instance of Random, rather
	/// than time.
	/// </summary>
	public static Random NewRandom()
	{
		// see https://devblogs.microsoft.com/pfxteam/getting-random-numbers-in-a-thread-safe-way/
		//   Of course, if you really care about the quality of the random numbers, you should be
		//   using RNGCryptoServiceProvider, which generates cryptographically-strong random
		//   numbers (Addendum: davidacoder makes a good point in his comments on this post that
		//   while Random has certain statistical properties, using multiple Random instances as
		//   part of the same algorithm may change the statistical properties in unknown or
		//   undesirable ways). For a look at how to get a Random-based facade for RNGCryptoServiceProvider,
		//   see .NET Matters: Tales from the CryptoRandom in the September 2007 issue of MSDN Magazine.
		//   You could also settle on an intermediate solution, such as using an RNGCryptoServiceProvider
		//   to provide the seed values for the ThreadStatic Random instances in a solution like
		//   that in RandomGen2 (which would help to avoid another issue here, that of two threads
		//   starting with the same seed value due to accessing the global Random instance in the
		//   same time quantum).
		const int bytesCount = 4;
#if NET6_0_OR_GREATER
		byte[] buffer = RandomNumberGenerator.GetBytes(bytesCount);
#else
		byte[] buffer = new byte[bytesCount];
		rngCrypto.GetBytes(buffer);
#endif
		var seed = BitConverter.ToInt32(buffer, 0);
		return new Random(seed);
	}
}
