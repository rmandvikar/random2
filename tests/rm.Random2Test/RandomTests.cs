using System;
using System.Diagnostics;
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
			private static readonly object locker = new object();

			[Test]
			[TestCase(false)]
			[TestCase(true)]
			public void Random_Instances_In_Quick_Succession_Give_Same_Results_In_NetFramework(bool delay)
			{
				lock (locker)
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

					lock (locker)
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
			// if the test were to fail it would fail quickly at start but not later,
			// so repeat it many times with a small timeout to force a failure
			private const int repeatCount = 20;

			[Repeat(repeatCount)]
			[Test(Description = "Next() results in 0s once in a while.")]
			public void Verify_Correctness_Random()
			{
				var random = RandomFactory.GetRandom();
				VerifyCorrectness(() => random.Next());
			}

			[Repeat(repeatCount)]
			[Test]
			public void Verify_Correctness_LockRandom()
			{
				var random = RandomFactory.GetLockRandom();
				VerifyCorrectness(() => random.Next());
			}

			[Repeat(repeatCount)]
			[Test]
			public void Verify_Correctness_ThreadStaticRandom()
			{
				var random = RandomFactory.GetThreadStaticRandom();
				VerifyCorrectness(() => random.Next());
			}

			[Repeat(repeatCount)]
			[Test]
			public void Verify_Correctness_ThreadLocalRandom()
			{
				var random = RandomFactory.GetThreadLocalRandom();
				VerifyCorrectness(() => random.Next());
			}

			[Repeat(repeatCount)]
			[Test]
			public void Verify_Correctness_NewInstance()
			{
				VerifyCorrectness(() => new Random().Next());
			}

			[Repeat(repeatCount)]
			[Test]
			public void Verify_Correctness_NewGuidAsSeed()
			{
				VerifyCorrectness(() => new Random(Guid.NewGuid().GetHashCode()).Next());
			}

#if NET6_0_OR_GREATER
			[Repeat(repeatCount)]
			[Test]
			public void Verify_Correctness_SharedRandom()
			{
				var random = RandomFactory.GetSharedRandom();
				VerifyCorrectness(() => random.Next());
			}
#endif

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

				const int timeoutInMilliseconds = 250;
				var timeout = TimeSpan.FromMilliseconds(timeoutInMilliseconds);
				var stopwatch = Stopwatch.StartNew();

				// consecutive 0s count
				var zeroes = 0;
				while (stopwatch.Elapsed.TotalSeconds <= timeout.TotalSeconds)
				{
					var next = randomFunc();

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
						Assert.Fail("incorrect: last 10 values are 0s");
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
				var random = RandomFactory.GetRandom();
				VerifyPerf(() => random.Next());
			}

			[Test]
			public void Verify_Perf_LockRandom()
			{
				var random = RandomFactory.GetLockRandom();
				VerifyPerf(() => random.Next());
			}

			[Test]
			public void Verify_Perf_ThreadStaticRandom()
			{
				var random = RandomFactory.GetThreadStaticRandom();
				VerifyPerf(() => random.Next());
			}

			[Test]
			public void Verify_Perf_ThreadLocalRandom()
			{
				var random = RandomFactory.GetThreadLocalRandom();
				VerifyPerf(() => random.Next());
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

#if NET6_0_OR_GREATER
			[Test]
			public void Verify_Perf_SharedRandom()
			{
				var random = RandomFactory.GetSharedRandom();
				VerifyPerf(() => random.Next());
			}
#endif

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
				var random = RandomFactory.GetRandom();
				VerifyDistribution(() => random.Next(10));
			}

			[Test]
			public void Verify_Distribution_LockRandom()
			{
				var random = RandomFactory.GetLockRandom();
				VerifyDistribution(() => random.Next(10));
			}

			[Test]
			public void Verify_Distribution_ThreadStaticRandom()
			{
				var random = RandomFactory.GetThreadStaticRandom();
				VerifyDistribution(() => random.Next(10));
			}

			[Test]
			public void Verify_Distribution_ThreadLocalRandom()
			{
				var random = RandomFactory.GetThreadLocalRandom();
				VerifyDistribution(() => random.Next(10));
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

#if NET6_0_OR_GREATER
			[Test]
			public void Verify_Distribution_SharedRandom()
			{
				var random = RandomFactory.GetSharedRandom();
				VerifyDistribution(() => random.Next(10));
			}
#endif

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
	}
}
