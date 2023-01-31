using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace rm.Random2
{
	/// <note>
	/// see https://devblogs.microsoft.com/pfxteam/getting-random-numbers-in-a-thread-safe-way/
	/// </note>
	public class ThreadStaticRandom : Random, IDisposable
	{
		private readonly RNGCryptoServiceProvider rngCrypto = new RNGCryptoServiceProvider();

		[ThreadStatic]
		private static Random randomLocal;

		private bool disposed = false;

		internal ThreadStaticRandom()
			: base()
		{ }

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposed)
			{
				return;
			}
			if (disposing)
			{
				rngCrypto?.Dispose();
			}
			disposed = true;
		}

		private Random CreateRandomIfNull()
		{
			var random = randomLocal;
			if (random == null)
			{
				randomLocal = random = CreateRandom();
			}
			return random;
		}

		// borrowed from
		// https://github.com/dotnet/runtime/pull/50297/files#diff-6fa7e54f57878bb019a11332aeeb42c75430a0ac87c78cdfa9ce382137b3d851R189-R190
		[MethodImpl(MethodImplOptions.NoInlining)]
		private Random CreateRandom()
		{
			byte[] buffer = new byte[4];
			rngCrypto.GetBytes(buffer);
			return new Random(BitConverter.ToInt32(buffer, 0));
		}

		public override int Next()
		{
			var random = CreateRandomIfNull();
			return random.Next();
		}

		public override int Next(int maxValue)
		{
			var random = CreateRandomIfNull();
			return random.Next(maxValue);
		}

		public override int Next(int minValue, int maxValue)
		{
			var random = CreateRandomIfNull();
			return random.Next(minValue, maxValue);
		}

		public override void NextBytes(byte[] buffer)
		{
			var random = CreateRandomIfNull();
			random.NextBytes(buffer);
		}

		public override double NextDouble()
		{
			var random = CreateRandomIfNull();
			return random.NextDouble();
		}

		protected override double Sample()
		{
			throw new NotImplementedException();
		}
	}
}
