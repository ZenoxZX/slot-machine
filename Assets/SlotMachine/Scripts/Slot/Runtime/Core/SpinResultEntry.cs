namespace SlotMachine.Slot.Core
{
    public readonly struct SpinResultEntry
    {
        public readonly SpinResult Result;
        public readonly int Count;

        public SpinResultEntry(SpinResult result, int count)
        {
            Result = result;
            Count = count;
        }
    }
}
