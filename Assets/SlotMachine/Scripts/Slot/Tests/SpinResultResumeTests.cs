using System.Collections.Generic;
using NUnit.Framework;
using SlotMachine.Slot.Core;
using SlotMachine.Slot.Data;

namespace SlotMachine.Slot.Tests
{
    [TestFixture]
    public class SpinResultResumeTests
    {
        private const int k_PoolSize = SpinResultTable.PoolCapacity;
        private const int k_TestSeed = 42;

        [Test]
        public void Constructor_WithStartIndex_ResumesFromCorrectPosition()
        {
            SpinResultProvider reference = new(k_TestSeed);

            for (int i = 0; i < 37; i++)
                reference.GetNext();

            SpinResultProvider resumed = new(k_TestSeed, 37);

            for (int i = 37; i < k_PoolSize; i++)
            {
                Assert.AreEqual
                (
                    reference.GetNext(),
                    resumed.GetNext(),
                    $"Spin {i} differs between reference and resumed provider."
                );
            }
        }

        [Test]
        public void Constructor_WithStartIndex_MaintainsDistribution()
        {
            const int consumed = 40;
            const int remaining = k_PoolSize - consumed;

            SpinResultProvider first = new(k_TestSeed);
            Dictionary<SpinResult, int> counts = new();

            for (int i = 0; i < consumed; i++)
            {
                SpinResult r = first.GetNext();

                if (!counts.ContainsKey(r))
                    counts[r] = 0;

                counts[r]++;
            }

            SpinResultProvider resumed = new(first.Seed, first.CurrentIndex);

            for (int i = 0; i < remaining; i++)
            {
                SpinResult r = resumed.GetNext();

                if (!counts.ContainsKey(r))
                    counts[r] = 0;

                counts[r]++;
            }

            foreach (SpinResultEntry entry in SpinResultTable.Entries)
            {
                Assert.IsTrue
                (
                    counts.ContainsKey(entry.Result),
                    $"{entry.Result} missing from combined results."
                );

                Assert.AreEqual
                (
                    entry.Count,
                    counts[entry.Result],
                    $"{entry.Result} expected {entry.Count} but got {counts[entry.Result]}."
                );
            }
        }

        [Test]
        public void Seed_ExposedCorrectly_AfterConstruction()
        {
            SpinResultProvider provider = new(k_TestSeed);

            Assert.AreEqual
            (
                k_TestSeed,
                provider.Seed,
                "Seed should match the value passed to constructor."
            );
        }

        [Test]
        public void Seed_ChangesAfterPoolExhaustion()
        {
            SpinResultProvider provider = new(k_TestSeed);
            int originalSeed = provider.Seed;

            for (int i = 0; i < k_PoolSize; i++)
                provider.GetNext();

            Assert.AreNotEqual
            (
                originalSeed,
                provider.Seed,
                "Seed should change after pool is exhausted."
            );
        }

        [Test]
        public void Constructor_WithStartIndex_SeedAndIndexMatchExpected()
        {
            const int expected = 50;
            SpinResultProvider provider = new(k_TestSeed, expected);

            Assert.AreEqual(k_TestSeed, provider.Seed);
            Assert.AreEqual(expected, provider.CurrentIndex);
        }
    }
}
