using SlotMachine.Slot.Core;

namespace SlotMachine.Slot.Data
{
    public static class SpinResultTable
    {
        public static readonly SpinResultEntry[] Entries =
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

        public const int PoolCapacity = 100;
    }
}
