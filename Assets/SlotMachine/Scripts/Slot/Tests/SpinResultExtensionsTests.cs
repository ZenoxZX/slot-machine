using NUnit.Framework;
using SlotMachine.Slot.Core;
using SlotMachine.Slot.Utils;

namespace SlotMachine.Slot.Tests
{
    [TestFixture]
    public class SpinResultExtensionsTests
    {
        [Test]
        public void Resolve_AWildBonus_ReturnsCorrectSymbols()
        {
            ResolvedSpinResult resolved = SpinResult.A_Wild_Bonus.Resolve();

            Assert.AreEqual(Symbol.A, resolved.Left);
            Assert.AreEqual(Symbol.Wild, resolved.Middle);
            Assert.AreEqual(Symbol.Bonus, resolved.Right);
        }

        [Test]
        public void Resolve_WildWildSeven_ReturnsCorrectSymbols()
        {
            ResolvedSpinResult resolved = SpinResult.Wild_Wild_Seven.Resolve();

            Assert.AreEqual(Symbol.Wild, resolved.Left);
            Assert.AreEqual(Symbol.Wild, resolved.Middle);
            Assert.AreEqual(Symbol.Seven, resolved.Right);
        }

        [Test]
        public void Resolve_JackpotJackpotA_ReturnsCorrectSymbols()
        {
            ResolvedSpinResult resolved = SpinResult.Jackpot_Jackpot_A.Resolve();

            Assert.AreEqual(Symbol.Jackpot, resolved.Left);
            Assert.AreEqual(Symbol.Jackpot, resolved.Middle);
            Assert.AreEqual(Symbol.A, resolved.Right);
        }

        [Test]
        public void Resolve_WildBonusA_ReturnsCorrectSymbols()
        {
            ResolvedSpinResult resolved = SpinResult.Wild_Bonus_A.Resolve();

            Assert.AreEqual(Symbol.Wild, resolved.Left);
            Assert.AreEqual(Symbol.Bonus, resolved.Middle);
            Assert.AreEqual(Symbol.A, resolved.Right);
        }

        [Test]
        public void Resolve_BonusAJackpot_ReturnsCorrectSymbols()
        {
            ResolvedSpinResult resolved = SpinResult.Bonus_A_Jackpot.Resolve();

            Assert.AreEqual(Symbol.Bonus, resolved.Left);
            Assert.AreEqual(Symbol.A, resolved.Middle);
            Assert.AreEqual(Symbol.Jackpot, resolved.Right);
        }

        [Test]
        public void Resolve_TripleA_ReturnsAllSameSymbol()
        {
            ResolvedSpinResult resolved = SpinResult.A_A_A.Resolve();

            Assert.AreEqual(Symbol.A, resolved.Left);
            Assert.AreEqual(Symbol.A, resolved.Middle);
            Assert.AreEqual(Symbol.A, resolved.Right);
        }

        [Test]
        public void Resolve_TripleBonus_ReturnsAllSameSymbol()
        {
            ResolvedSpinResult resolved = SpinResult.Bonus_Bonus_Bonus.Resolve();

            Assert.AreEqual(Symbol.Bonus, resolved.Left);
            Assert.AreEqual(Symbol.Bonus, resolved.Middle);
            Assert.AreEqual(Symbol.Bonus, resolved.Right);
        }

        [Test]
        public void Resolve_TripleSeven_ReturnsAllSameSymbol()
        {
            ResolvedSpinResult resolved = SpinResult.Seven_Seven_Seven.Resolve();

            Assert.AreEqual(Symbol.Seven, resolved.Left);
            Assert.AreEqual(Symbol.Seven, resolved.Middle);
            Assert.AreEqual(Symbol.Seven, resolved.Right);
        }

        [Test]
        public void Resolve_TripleWild_ReturnsAllSameSymbol()
        {
            ResolvedSpinResult resolved = SpinResult.Wild_Wild_Wild.Resolve();

            Assert.AreEqual(Symbol.Wild, resolved.Left);
            Assert.AreEqual(Symbol.Wild, resolved.Middle);
            Assert.AreEqual(Symbol.Wild, resolved.Right);
        }

        [Test]
        public void Resolve_TripleJackpot_ReturnsAllSameSymbol()
        {
            ResolvedSpinResult resolved = SpinResult.Jackpot_Jackpot_Jackpot.Resolve();

            Assert.AreEqual(Symbol.Jackpot, resolved.Left);
            Assert.AreEqual(Symbol.Jackpot, resolved.Middle);
            Assert.AreEqual(Symbol.Jackpot, resolved.Right);
        }

        [Test]
        public void Resolve_None_ReturnsFallback()
        {
            ResolvedSpinResult resolved = SpinResult.None.Resolve();

            Assert.AreEqual(Symbol.A, resolved.Left);
            Assert.AreEqual(Symbol.A, resolved.Middle);
            Assert.AreEqual(Symbol.A, resolved.Right);
        }
    }
}
