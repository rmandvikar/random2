using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using rm.Random2;

namespace rm.Random2Test
{
	[TestFixture]
	public class RandomTests
	{
		public class Showcase_Issues
		{
			private static readonly object _lock = new object();

			[Test]
			[TestCase(false)]
			[TestCase(true)]
			public void Random_Instances_In_Quick_Succession_Give_Same_Results_In_NetFramework(bool delay)
			{
				lock (_lock)
				{
					var random1 = new Random();

					/// <note>
					/// delay of 15ms+ (time interval after which <see cref="Environment.TickCount"/> changes)
					/// causes the random values to be different.
					/// </note>
					if (delay)
					{
						Thread.Sleep(15);
					}

					lock (_lock)
					{
						var random2 = new Random();

						Console.WriteLine("The first random number generator:");
						for (int ctr = 1; ctr <= 10; ctr++)
						{
							Console.WriteLine("   {0}", random1.Next());
						}

						Console.WriteLine("The second random number generator:");
						for (int ctr = 1; ctr <= 10; ctr++)
						{
							Console.WriteLine("   {0}", random2.Next());
						}
					}
				}
			}

			[Test]
			public void Random_Instances_With_Global_And_Child_Instances_Exhibit_Patterns()
			{
				const int OFFSET = 1337;
				const int LIMIT = 200;

				// repeat experiment with different global RNGs
				for (int iGlobal = 0; iGlobal < 30; ++iGlobal)
				{
					// create global RNG
					var globalRandom = new Random(iGlobal + OFFSET);

					// obtain seed from global RNG
					var seed = globalRandom.Next();

					// create main RNG from seed
					var random = new Random(seed);

					// patterns exhibited by numbers:
					// 3rd is same number always!
					// others exhibit patterns
					for (int i = 0; i < 100; i++)
					{
						Console.Write("{0,3}", random.Next(LIMIT));
						Console.Write(", ");
					}
					Console.WriteLine();
				}
			}

			[Test]
			public void Mimic_Thread_Affinity_RNGs_As_ThreadStatic_And_ThreadLocal()
			{
				const int LIMIT = 200;

				// create global RNG
				var globalRandom = new Random();

				// repeat experiment with different global RNGs
				for (int ithreads = 0; ithreads < 30; ++ithreads)
				{
					// obtain seed from global RNG
					var seed = globalRandom.Next();

					// create main RNG from seed
					var random = new Random(seed);

					for (int i = 0; i < 100; i++)
					{
						Console.Write("{0,3}", random.Next(LIMIT));
						Console.Write(", ");
					}
					Console.WriteLine();
				}
			}

			[Test]
			public void Random_Is_Deterministic()
			{
				const int iterations = 100;
				const int seed = 1337;
				var random1 = new Random(seed);
				var random2 = new Random(seed);
				for (int i = 0; i < iterations; i++)
				{
					Assert.AreEqual(random1.Next(), random2.Next());
				}
			}
		}

		[TestFixture]
		public class Verify_Correctness
		{
			[Explicit]
			[Test(Description = "Next() results in 0s once in a while.")]
			public void Verify_Correctness_Random()
			{
				VerifyCorrectness(() => RandomFactory.GetRandom().Next());
			}

			[Explicit]
			[Test]
			public void Verify_Correctness_LockRandom()
			{
				VerifyCorrectness(() => RandomFactory.GetLockRandom().Next());
			}

			[Explicit]
			[Test]
			public void Verify_Correctness_ThreadStaticRandom()
			{
				VerifyCorrectness(() => RandomFactory.GetThreadStaticRandom().Next());
			}

			[Explicit]
			[Test]
			public void Verify_Correctness_ThreadLocalRandom()
			{
				VerifyCorrectness(() => RandomFactory.GetThreadLocalRandom().Next());
			}

			[Explicit]
			[Test]
			public void Verify_Correctness_NewInstance()
			{
				VerifyCorrectness(() => new Random().Next());
			}

			[Explicit]
			[Test]
			public void Verify_Correctness_NewGuidAsSeed()
			{
				VerifyCorrectness(() => new Random(Guid.NewGuid().GetHashCode()).Next());
			}

			private void VerifyCorrectness(Func<int> randomFunc)
			{
				const int iterations = 1_000_000;
				Parallel.For(0, iterations, (i, loop) =>
				{
					if (randomFunc() == 0)
					{
						//loop.Stop();
					}
				});

				// consecutive 0s count
				var zeroes = 0;
				while (true)
				{
					var next = randomFunc();
					Console.WriteLine(next);

					var isZero = next == 0;
					if (isZero)
					{
						zeroes++;
					}
					else
					{
						zeroes = 0;
					}

					if (zeroes == 10)
					{
						break;
					}
				}
			}
		}

		[TestFixture]
		public class Verify_Perf
		{
			[Test]
			public void Verify_Perf_Random()
			{
				VerifyPerf(() => RandomFactory.GetRandom().Next());
			}

			[Test]
			public void Verify_Perf_LockRandom()
			{
				VerifyPerf(() => RandomFactory.GetLockRandom().Next());
			}

			[Test]
			public void Verify_Perf_ThreadStaticRandom()
			{
				VerifyPerf(() => RandomFactory.GetThreadStaticRandom().Next());
			}

			[Test]
			public void Verify_Perf_ThreadLocalRandom()
			{
				VerifyPerf(() => RandomFactory.GetThreadLocalRandom().Next());
			}

			[Test]
			public void Verify_Perf_NewInstance()
			{
				VerifyPerf(() => new Random().Next());
			}

			[Test]
			public void Verify_Perf_NewGuidAsSeed()
			{
				VerifyPerf(() => new Random(Guid.NewGuid().GetHashCode()).Next());
			}

			private void VerifyPerf(Func<int> randomFunc)
			{
				const int iterations = 10_000_000;
				var sw = Stopwatch.StartNew();
				Parallel.For(0, iterations, i =>
				{
					randomFunc();
				});
				sw.Stop();
				Console.WriteLine(sw.ElapsedMilliseconds);
			}
		}

		[TestFixture]
		public class Verify_Distribution
		{
			[Test]
			public void Verify_Distribution_Random()
			{
				VerifyDistribution(() => RandomFactory.GetRandom().Next(10));
			}

			[Test]
			public void Verify_Distribution_LockRandom()
			{
				VerifyDistribution(() => RandomFactory.GetLockRandom().Next(10));
			}

			[Test]
			public void Verify_Distribution_ThreadStaticRandom()
			{
				VerifyDistribution(() => RandomFactory.GetThreadStaticRandom().Next(10));
			}

			[Test]
			public void Verify_Distribution_ThreadLocalRandom()
			{
				VerifyDistribution(() => RandomFactory.GetThreadLocalRandom().Next(10));
			}

			[Test]
			public void Verify_Distribution_NewInstance()
			{
				VerifyDistribution(() => new Random().Next(10));
			}

			[Test]
			public void Verify_Distribution_NewGuidAsSeed()
			{
				VerifyDistribution(() => new Random(Guid.NewGuid().GetHashCode()).Next(10));
			}

			private void VerifyDistribution(Func<int> randomFunc)
			{
				const int iterations = 100;
				Parallel.For(0, iterations, i =>
				{
					var next = randomFunc();
					Console.WriteLine(next);
				});
			}
		}

		[TestFixture]
		public class HashAlgorithmTests
		{
			[Explicit]
			[Test(Description = "HashAlgorithm is not thread-safe")]
			public void Bork_HashAlgorithm()
			{
				var bytes = Encoding.UTF8.GetBytes("the overtinkerer");
				var md5 = MD5.Create();
				{
					Parallel.For(0, iterations, (i, loop) =>
					{
						var hash = md5.ComputeHash(bytes);
						//Console.WriteLine(hash);
					});
				}
				//Console.WriteLine("stop");
			}

			[Explicit]
			[Test(Description = "HashAlgorithm is not thread-safe")]
			public void Bork_HashAlgorithm_New()
			{
				var bytes = Encoding.UTF8.GetBytes("the overtinkerer");

				Parallel.For(0, iterations, (i, loop) =>
				{
					using var md5 = MD5.Create();
					{
						var hash = md5.ComputeHash(bytes);
						//Console.WriteLine(hash);
					}
				});
				//Console.WriteLine("stop");
			}

			[Explicit]
			[Test(Description = "HashAlgorithm is not thread-safe")]
			public void Bork_HashAlgorithm_ThreadStatic()
			{
				var bytes = Encoding.UTF8.GetBytes("the overtinkerer");

				Parallel.For(0, iterations, (i, loop) =>
				{
					var md5 = CreateHashAlgorithmIfNull();
					var hash = md5.ComputeHash(bytes);
					//var hex = BitConverter.ToString(hash);
					//Console.WriteLine(hex);
				});
				//Console.WriteLine("stop");
			}

			[Explicit]
			[Test(Description = "HashAlgorithm is not thread-safe")]
			public void Bork_HashAlgorithm_ThreadLocal()
			{
				var bytes = Encoding.UTF8.GetBytes("the overtinkerer");

				Parallel.For(0, iterations, (i, loop) =>
				{
					var hash = hashAlgorithmThreadLocal.Value.ComputeHash(bytes);
					//var hex = BitConverter.ToString(hash);
					//Console.WriteLine(hex);
				});
				//Console.WriteLine("stop");
			}

			private static ThreadLocal<HashAlgorithm> hashAlgorithmThreadLocal =
				new ThreadLocal<HashAlgorithm>(() => Md5Factory);
			private static MD5 Md5Factory => MD5.Create();

			[Explicit]
			[Test(Description = "HashAlgorithm is not thread-safe")]
			public void Bork_HashAlgorithm_StaticMethod()
			{
				var bytes = Encoding.UTF8.GetBytes("the overtinkerer");

				Parallel.For(0, iterations, (i, loop) =>
				{
					var hash = MD5.HashData(bytes);
					//var hex = BitConverter.ToString(hash);
					//Console.WriteLine(hex);
				});
				//Console.WriteLine("stop");
			}

			[Explicit]
			[Test(Description = "HashAlgorithm is not thread-safe")]
			public void Bork_HashAlgorithm_StaticMethodTry()
			{
				var bytes = Encoding.UTF8.GetBytes("the overtinkerer");

				Parallel.For(0, iterations, (i, loop) =>
				{
					var hash = new Span<byte>(new byte[16]);
					if (!MD5.TryHashData(bytes, hash, out var _))
					{
						throw new InvalidOperationException();
					}
					//var hex = BitConverter.ToString(hash);
					//Console.WriteLine(hex);
				});
				//Console.WriteLine("stop");
			}

			[Explicit]
			[Test(Description = "HashAlgorithm is not thread-safe")]
			public void Bork_HashAlgorithm_Lock()
			{
				var bytes = Encoding.UTF8.GetBytes("the overtinkerer");
				var locker = new object();
				var md5 = MD5.Create();
				{
					Parallel.For(0, iterations, (i, loop) =>
					{
						lock (locker)
						{
							var hash = md5.ComputeHash(bytes);
						}
						//Console.WriteLine(hash);
					});
				}
				//Console.WriteLine("stop");
			}

			const int iterations = 10_000_000;

			[ThreadStatic]
			private static HashAlgorithm hashAlgorithmThreadStatic;
			private HashAlgorithm CreateHashAlgorithmIfNull()
			{
				var hashAlgorithmThread = hashAlgorithmThreadStatic;
				if (hashAlgorithmThread == null)
				{
					hashAlgorithmThreadStatic = hashAlgorithmThread = MD5.Create();
				}
				return hashAlgorithmThread;
			}
		}
	}
}
