using System;
using System.Diagnostics;
using System.Threading.Tasks;
using NUnit.Framework;
using rm.Random2;

namespace rm.Random2Test
{
	[TestFixture]
	public class RandomTests
	{
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
	}
}
