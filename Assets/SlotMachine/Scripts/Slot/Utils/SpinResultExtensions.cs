using SlotMachine.Slot.Core;

namespace SlotMachine.Slot.Utils
{
    public static class SpinResultExtensions
    {
        public static ResolvedSpinResult Resolve(this SpinResult result) => result switch
        {
            SpinResult.A_Wild_Bonus => new(Symbol.A, Symbol.Wild, Symbol.Bonus),
            SpinResult.Wild_Wild_Seven => new(Symbol.Wild, Symbol.Wild, Symbol.Seven),
            SpinResult.Jackpot_Jackpot_A => new(Symbol.Jackpot, Symbol.Jackpot, Symbol.A),
            SpinResult.Wild_Bonus_A => new(Symbol.Wild, Symbol.Bonus, Symbol.A),
            SpinResult.Bonus_A_Jackpot => new(Symbol.Bonus, Symbol.A, Symbol.Jackpot),
            SpinResult.A_A_A => new(Symbol.A, Symbol.A, Symbol.A),
            SpinResult.Bonus_Bonus_Bonus => new(Symbol.Bonus, Symbol.Bonus, Symbol.Bonus),
            SpinResult.Seven_Seven_Seven => new(Symbol.Seven, Symbol.Seven, Symbol.Seven),
            SpinResult.Wild_Wild_Wild => new(Symbol.Wild, Symbol.Wild, Symbol.Wild),
            SpinResult.Jackpot_Jackpot_Jackpot => new(Symbol.Jackpot, Symbol.Jackpot, Symbol.Jackpot),
            _ => new(Symbol.A, Symbol.A, Symbol.A)
        };
    }
}
