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
		}

		[TestFixture]
		public class Verify_Correctness
		{
			[Explicit]
			[Test(Description = "Next() results in 0s once in a while.")]
			public void Verify_Correctness_Random()
			{
				VerifyCorrectness(RandomFactory.GetRandom());
			}

			[Explicit]
			[Test]
			public void Verify_Correctness_LockRandom()
			{
				VerifyCorrectness(RandomFactory.GetLockRandom());
			}

			[Explicit]
			[Test]
			public void Verify_Correctness_ThreadStaticRandom()
			{
				VerifyCorrectness(RandomFactory.GetThreadStaticRandom());
			}

			[Explicit]
			[Test]
			public void Verify_Correctness_ThreadLocalRandom()
			{
				VerifyCorrectness(RandomFactory.GetThreadLocalRandom());
			}

			private void VerifyCorrectness(Random random)
			{
				const int iterations = 1_000_000;
				Parallel.For(0, iterations, (i, loop) =>
				{
					if (random.Next() == 0)
					{
						//loop.Stop();
					}
				});

				// consecutive 0s count
				var zeroes = 0;
				while (true)
				{
					var next = random.Next();
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
				VerifyPerf(RandomFactory.GetRandom());
			}

			[Test]
			public void Verify_Perf_LockRandom()
			{
				VerifyPerf(RandomFactory.GetLockRandom());
			}

			[Test]
			public void Verify_Perf_ThreadStaticRandom()
			{
				VerifyPerf(RandomFactory.GetThreadStaticRandom());
			}

			[Test]
			public void Verify_Perf_ThreadLocalRandom()
			{
				VerifyPerf(RandomFactory.GetThreadLocalRandom());
			}

			private void VerifyPerf(Random random)
			{
				const int iterations = 10_000_000;
				var sw = Stopwatch.StartNew();
				Parallel.For(0, iterations, i =>
				{
					random.Next();
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
				VerifyDistribution(RandomFactory.GetRandom());
			}

			[Test]
			public void Verify_Distribution_LockRandom()
			{
				VerifyDistribution(RandomFactory.GetLockRandom());
			}

			[Test]
			public void Verify_Distribution_ThreadStaticRandom()
			{
				VerifyDistribution(RandomFactory.GetThreadStaticRandom());
			}

			[Test]
			public void Verify_Distribution_ThreadLocalRandom()
			{
				VerifyDistribution(RandomFactory.GetThreadLocalRandom());
			}

			private void VerifyDistribution(Random random)
			{
				const int iterations = 100;
				Parallel.For(0, iterations, i =>
				{
					var next = random.Next(10);
					Console.WriteLine(next);
				});
			}
		}
	}
}
