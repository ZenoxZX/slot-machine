using SlotMachine.Slot.Core;

namespace SlotMachine.Slot.Data
{
    public interface ISpinResultProvider
    {
        SpinResult GetNext();
        int Seed { get; }
        int CurrentIndex { get; }
        int PoolSize { get; }
    }
}
