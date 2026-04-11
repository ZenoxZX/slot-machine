using SlotMachine.Messages;
using SlotMachine.Slot.Core;

namespace SlotMachine.Slot.Messages
{
    public readonly struct SpinCompletedMessage : IMessage
    {
        public readonly ResolvedSpinResult Result;
        public readonly bool IsWin;

        public SpinCompletedMessage(ResolvedSpinResult result, bool isWin)
        {
            Result = result;
            IsWin = isWin;
        }
    }
}
