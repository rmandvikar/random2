using System;
using System.Security.Cryptography;

namespace rm.Random2
{
	/// <note>
	/// see https://devblogs.microsoft.com/pfxteam/getting-random-numbers-in-a-thread-safe-way/
	/// </note>
	public class RandomPerThread : Random, IDisposable
	{
		private readonly RNGCryptoServiceProvider _rngCrypto = new RNGCryptoServiceProvider();

		[ThreadStatic]
		private Random _randomLocal;

		private bool _disposed = false;

		internal RandomPerThread()
			: base()
		{ }

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}
			if (disposing)
			{
				_rngCrypto?.Dispose();
			}
			_disposed = true;
		}

		private Random CreateRandomIfNull()
		{
			var random = _randomLocal;
			if (random == null)
			{
				_randomLocal = random = CreateRandom();
			}
			return random;
		}

		private Random CreateRandom()
		{
			byte[] buffer = new byte[4];
			_rngCrypto.GetBytes(buffer);
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
