namespace SlotMachine.Slot.Core
{
    public readonly struct ResolvedSpinResult
    {
        public readonly Symbol Left;
        public readonly Symbol Middle;
        public readonly Symbol Right;

        public ResolvedSpinResult(Symbol left, Symbol middle, Symbol right)
        {
            Left = left;
            Middle = middle;
            Right = right;
        }
    }
}
