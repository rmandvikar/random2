using System;

namespace rm.Random2;

/// <note>
/// see https://csharpindepth.com/Articles/Random
/// </note>
public static class RandomFactory
{
	private static readonly Random random = new();
	private static readonly LockRandom lockRandom = new();
	private static readonly ThreadStaticRandom threadStaticRandom = new();
	private static readonly ThreadLocalRandom threadLocalRandom = new();

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

#if NET6_0_OR_GREATER
	public static Random GetSharedRandom()
	{
		return Random.Shared;
	}
#endif
}
