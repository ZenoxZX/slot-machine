using SlotMachine.Slot.Core;

namespace SlotMachine.Slot.Data
{
    public interface ISpinResultProvider
    {
        SpinResult GetNext();
        int CurrentIndex { get; }
        int PoolSize { get; }
    }
}
