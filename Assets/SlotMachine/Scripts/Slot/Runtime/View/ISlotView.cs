using SlotMachine.Slot.Core;

namespace SlotMachine.Slot.View
{
    public interface ISlotView
    {
        void StartSpin(ResolvedSpinResult targetSymbols, StopMode stopMode);
    }
}
