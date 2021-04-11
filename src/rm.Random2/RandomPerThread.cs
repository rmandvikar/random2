using System;
using System.Security.Cryptography;

namespace rm.Random2
{
	/// <note>
	/// see https://devblogs.microsoft.com/pfxteam/getting-random-numbers-in-a-thread-safe-way/
	/// </note>
	public class RandomPerThread : IDisposable
	{
		private readonly RNGCryptoServiceProvider _rngCrypto;

		[ThreadStatic]
		private Random _randomLocal;

		private bool _disposed = false;

		internal RandomPerThread(RNGCryptoServiceProvider rngCrypto)
		{
			_rngCrypto = rngCrypto
				?? throw new ArgumentNullException(nameof(rngCrypto));
		}

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

		private Random CreateRandom()
		{
			byte[] buffer = new byte[4];
			_rngCrypto.GetBytes(buffer);
			return new Random(BitConverter.ToInt32(buffer, 0));
		}

		public int Next(int minValue = 0, int maxValue = int.MaxValue)
		{
			var random = _randomLocal;
			if (random == null)
			{
				_randomLocal = random = CreateRandom();
			}
			return random.Next(minValue, maxValue);
		}
	}
}
