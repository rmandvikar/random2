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
