﻿using System;
using System.Threading;

namespace rm.Random2
{
	/// <summary>
	/// Convenience class for dealing with randomness.
	/// </summary>
	/// <note>
	/// see https://codeblog.jonskeet.uk/2009/11/04/revisiting-randomness/
	/// </note>
	public class ThreadLocalRandom : Random
	{
		/// <summary>
		/// Random number generator used to generate seeds,
		/// which are then used to create new random number
		/// generators on a per-thread basis.
		/// </summary>
		private static readonly Random globalRandom = new Random();
		private static readonly object locker = new object();

		/// <summary>
		/// Random number generator.
		/// </summary>
		private static readonly ThreadLocal<Random> threadRandom = new ThreadLocal<Random>(NewRandom);

		/// <summary>
		/// Creates a new instance of Random. The seed is derived
		/// from a global (static) instance of Random, rather
		/// than time.
		/// </summary>
		private static Random NewRandom()
		{
			lock (locker)
			{
				return new Random(globalRandom.Next());
			}
		}

		private static Random random => threadRandom.Value;

		internal ThreadLocalRandom()
			: base()
		{ }

		/// <summary>See <see cref="Random.Next()" />.</summary>
		public override int Next()
		{
			return random.Next();
		}

		/// <summary>See <see cref="Random.Next(int)" />.</summary>
		public override int Next(int maxValue)
		{
			return random.Next(maxValue);
		}

		/// <summary>See <see cref="Random.Next(int, int)" />.</summary>
		public override int Next(int minValue, int maxValue)
		{
			return random.Next(minValue, maxValue);
		}

		/// <summary>See <see cref="Random.NextBytes(byte[])" />.</summary>
		public override void NextBytes(byte[] buffer)
		{
			random.NextBytes(buffer);
		}

		/// <summary>See <see cref="Random.NextDouble()" />.</summary>
		public override double NextDouble()
		{
			return random.NextDouble();
		}

		/// <summary>See <see cref="Random.Sample()" />.</summary>
		protected override double Sample()
		{
			throw new NotImplementedException();
		}
	}
}
