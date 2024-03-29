﻿using System;
using System.Threading;

namespace rm.Random2Test;

/// <summary>
/// Convenience class for dealing with randomness.
/// <para></para>
/// <see href="https://codeblog.jonskeet.uk/2009/11/04/revisiting-randomness/">src</see>
/// </summary>
public static class ThreadLocalRandom
{
	/// <summary>
	/// Random number generator used to generate seeds,
	/// which are then used to create new random number
	/// generators on a per-thread basis.
	/// </summary>
	private static readonly Random globalRandom = new();
	private static readonly object globalLock = new();

	/// <summary>
	/// Random number generator
	/// </summary>
	private static readonly ThreadLocal<Random> threadRandom = new(NewRandom);

	/// <summary>
	/// Creates a new instance of Random. The seed is derived
	/// from a global (static) instance of Random, rather
	/// than time.
	/// </summary>
	public static Random NewRandom()
	{
		lock (globalLock)
		{
			return new Random(globalRandom.Next());
		}
	}

	/// <summary>
	/// Returns an instance of Random which can be used freely
	/// within the current thread.
	/// </summary>
	public static Random Instance { get { return threadRandom.Value!; } }

	/// <summary>See <see cref="Random.Next()" /></summary>
	public static int Next()
	{
		return Instance.Next();
	}

	/// <summary>See <see cref="Random.Next(int)" /></summary>
	public static int Next(int maxValue)
	{
		return Instance.Next(maxValue);
	}

	/// <summary>See <see cref="Random.Next(int, int)" /></summary>
	public static int Next(int minValue, int maxValue)
	{
		return Instance.Next(minValue, maxValue);
	}

	/// <summary>See <see cref="Random.NextDouble()" /></summary>
	public static double NextDouble()
	{
		return Instance.NextDouble();
	}

	/// <summary>See <see cref="Random.NextBytes(byte[])" /></summary>
	public static void NextBytes(byte[] buffer)
	{
		Instance.NextBytes(buffer);
	}
}
