using SlotMachine.MessagePipe.Pipes;
using SlotMachine.Slot.Core;
using SlotMachine.Slot.Data;
using SlotMachine.Slot.Messages;
using UnityEngine;
using VContainer;

namespace SlotMachine.Slot.View
{
    public class SlotView : MonoBehaviour, ISlotView
    {
        [Inject] private ReelContainer m_ReelContainer;
        [Inject] private GamePipe m_Pipe;

        void ISlotView.StartSpin(ResolvedSpinResult targetSymbols, StopMode stopMode)
        {
            // TODO : Implement the spin animation logic here.
        }
    }
}
