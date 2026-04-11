using System.Collections.Generic;
using NUnit.Framework;
using SlotMachine.Slot.Core;
using SlotMachine.Slot.Data;

namespace SlotMachine.Slot.Tests
{
    [TestFixture]
    public class SpinResultProviderTests
    {
        private const int k_PoolSize = 100;
        private const int k_TestSeed = 42;

        private static readonly SpinResultEntry[] s_ExpectedDistribution =
        {
            new(SpinResult.A_Wild_Bonus, 13),
            new(SpinResult.Wild_Wild_Seven, 13),
            new(SpinResult.Jackpot_Jackpot_A, 13),
            new(SpinResult.Wild_Bonus_A, 13),
            new(SpinResult.Bonus_A_Jackpot, 13),
            new(SpinResult.A_A_A, 9),
            new(SpinResult.Bonus_Bonus_Bonus, 8),
            new(SpinResult.Seven_Seven_Seven, 7),
            new(SpinResult.Wild_Wild_Wild, 6),
            new(SpinResult.Jackpot_Jackpot_Jackpot, 5),
        };

        private SpinResultProvider m_Provider;

        [SetUp]
        public void SetUp()
        {
            m_Provider = new(k_TestSeed);
        }

        [TearDown]
        public void TearDown()
        {
            m_Provider = null;
        }

        [Test]
        public void GetNext_After100Spins_HasExactDistribution()
        {
            Dictionary<SpinResult, int> counts = CountResults(m_Provider, k_PoolSize);

            foreach (SpinResultEntry entry in s_ExpectedDistribution)
            {
                Assert.IsTrue(counts.ContainsKey(entry.Result),
                    $"{entry.Result} never appeared in 100 spins.");

                Assert.AreEqual(entry.Count, counts[entry.Result],
                    $"{entry.Result} expected {entry.Count} times but got {counts[entry.Result]}.");
            }
        }

        [Test]
        public void GetNext_After100Spins_NeverReturnsNone()
        {
            for (int i = 0; i < k_PoolSize; i++)
            {
                SpinResult result = m_Provider.GetNext();

                Assert.AreNotEqual(SpinResult.None, result,
                    $"Spin {i} returned None.");
            }
        }

        [Test]
        public void GetNext_WithSameSeed_ProducesSameSequence()
        {
            SpinResultProvider otherProvider = new(k_TestSeed);

            for (int i = 0; i < k_PoolSize; i++)
            {
                Assert.AreEqual(m_Provider.GetNext(), otherProvider.GetNext(),
                    $"Spin {i} differs between same-seed providers.");
            }
        }

        [Test]
        public void GetNext_SecondPool_HasExactDistribution()
        {
            for (int i = 0; i < k_PoolSize; i++)
                m_Provider.GetNext();

            Dictionary<SpinResult, int> counts = CountResults(m_Provider, k_PoolSize);

            foreach (SpinResultEntry entry in s_ExpectedDistribution)
            {
                Assert.IsTrue(counts.ContainsKey(entry.Result),
                    $"{entry.Result} never appeared in second pool.");

                Assert.AreEqual(entry.Count, counts[entry.Result],
                    $"{entry.Result} expected {entry.Count} times but got {counts[entry.Result]} in second pool.");
            }
        }

        [Test]
        public void GetNext_JackpotTriple_AppearsOncePerBlock()
        {
            SpinResult[] pool = CollectPool(m_Provider, k_PoolSize);

            AssertPeriod(pool, SpinResult.Jackpot_Jackpot_Jackpot, 5);
        }

        [Test]
        public void GetNext_WildTriple_AppearsOncePerBlock()
        {
            SpinResult[] pool = CollectPool(m_Provider, k_PoolSize);

            AssertPeriod(pool, SpinResult.Wild_Wild_Wild, 6);
        }

        [Test]
        public void GetNext_SevenTriple_AppearsOncePerBlock()
        {
            SpinResult[] pool = CollectPool(m_Provider, k_PoolSize);

            AssertPeriod(pool, SpinResult.Seven_Seven_Seven, 7);
        }

        [Test]
        public void GetNext_BonusTriple_AppearsOncePerBlock()
        {
            SpinResult[] pool = CollectPool(m_Provider, k_PoolSize);

            AssertPeriod(pool, SpinResult.Bonus_Bonus_Bonus, 8);
        }

        [Test]
        public void GetNext_ATriple_AppearsOncePerBlock()
        {
            SpinResult[] pool = CollectPool(m_Provider, k_PoolSize);

            AssertPeriod(pool, SpinResult.A_A_A, 9);
        }

        [Test]
        public void GetNext_MixedResults_AppearOncePerBlock()
        {
            SpinResult[] pool = CollectPool(m_Provider, k_PoolSize);

            AssertPeriod(pool, SpinResult.A_Wild_Bonus, 13);
            AssertPeriod(pool, SpinResult.Wild_Wild_Seven, 13);
            AssertPeriod(pool, SpinResult.Jackpot_Jackpot_A, 13);
            AssertPeriod(pool, SpinResult.Wild_Bonus_A, 13);
            AssertPeriod(pool, SpinResult.Bonus_A_Jackpot, 13);
        }

        [Test]
        public void GetNext_MultipleSeeds_AllHaveExactDistribution(
            [Values(0, 1, 999, 12345, int.MaxValue)] int seed)
        {
            SpinResultProvider provider = new(seed);
            Dictionary<SpinResult, int> counts = CountResults(provider, k_PoolSize);

            foreach (SpinResultEntry entry in s_ExpectedDistribution)
            {
                Assert.IsTrue(counts.ContainsKey(entry.Result),
                    $"Seed {seed}: {entry.Result} never appeared.");

                Assert.AreEqual(entry.Count, counts[entry.Result],
                    $"Seed {seed}: {entry.Result} expected {entry.Count} but got {counts[entry.Result]}.");
            }
        }

        [Test]
        public void GetNext_After100Spins_Returns100ValidResults()
        {
            int validCount = 0;

            for (int i = 0; i < k_PoolSize; i++)
            {
                SpinResult result = m_Provider.GetNext();

                if (result != SpinResult.None)
                    validCount++;
            }

            Assert.AreEqual(k_PoolSize, validCount,
                $"Expected {k_PoolSize} valid results but got {validCount}.");
        }

        [Test]
        public void CurrentIndex_AfterEachSpin_IncrementsCorrectly()
        {
            for (int i = 0; i < k_PoolSize; i++)
            {
                Assert.AreEqual(i, m_Provider.CurrentIndex,
                    $"CurrentIndex should be {i} before spin {i}.");

                m_Provider.GetNext();
            }
        }

        [Test]
        public void CurrentIndex_AfterPoolExhausted_ResetsToZero()
        {
            for (int i = 0; i < k_PoolSize; i++)
                m_Provider.GetNext();

            Assert.AreEqual(0, m_Provider.CurrentIndex,
                "CurrentIndex should reset to 0 after pool is exhausted.");
        }

        private static Dictionary<SpinResult, int> CountResults(SpinResultProvider provider, int count)
        {
            Dictionary<SpinResult, int> counts = new();

            for (int i = 0; i < count; i++)
            {
                SpinResult result = provider.GetNext();

                if (!counts.ContainsKey(result))
                    counts[result] = 0;

                counts[result]++;
            }

            return counts;
        }

        private static SpinResult[] CollectPool(SpinResultProvider provider, int count)
        {
            SpinResult[] pool = new SpinResult[count];

            for (int i = 0; i < count; i++)
                pool[i] = provider.GetNext();

            return pool;
        }

        private static void AssertPeriod(SpinResult[] pool, SpinResult target, int expectedCount)
        {
            int baseBlockSize = pool.Length / expectedCount;
            int remainder = pool.Length % expectedCount;
            int blockStart = 0;

            for (int b = 0; b < expectedCount; b++)
            {
                int blockSize = baseBlockSize + (b < remainder ? 1 : 0);
                int blockEnd = blockStart + blockSize;
                int hitCount = 0;

                for (int i = blockStart; i < blockEnd; i++)
                {
                    if (pool[i] == target)
                        hitCount++;
                }

                Assert.AreEqual(1, hitCount,
                    $"{target} block {b} (spin {blockStart}-{blockEnd - 1}) has {hitCount}, expected 1.");

                blockStart = blockEnd;
            }
        }
    }
}
