using System;

namespace rm.Random2;

public static class RandomUtils
{
	/// <summary>
	/// Random number generator used to generate seeds,
	/// which are then used to create new random number
	/// generators on a per-thread basis.
	/// </summary>
	private static readonly Random globalRandom = new();
	private static readonly object locker = new();

	/// <summary>
	/// Creates a new instance of Random. The seed is derived
	/// from a global (static) instance of Random, rather
	/// than time.
	/// </summary>
	public static Random NewRandom()
	{
		lock (locker)
		{
			return new Random(globalRandom.Next());
		}
	}
}
