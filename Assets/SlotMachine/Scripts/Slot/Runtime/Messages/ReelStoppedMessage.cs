using SlotMachine.Messages;

namespace SlotMachine.Slot.Messages
{
    public readonly struct ReelStoppedMessage : IMessage
    {
        public readonly int ReelIndex;

        public ReelStoppedMessage(int reelIndex)
        {
            ReelIndex = reelIndex;
        }
    }
}
