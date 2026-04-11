using System;
using JetBrains.Annotations;
using SlotMachine.MessagePipe.Pipes;
using SlotMachine.Slot.Core;
using SlotMachine.Slot.Data;
using SlotMachine.Slot.Messages;
using SlotMachine.Slot.Utils;
using SlotMachine.Slot.View;
using VContainer.Unity;

namespace SlotMachine.Slot.Controller
{
    [UsedImplicitly]
    public class SpinController : IInitializable, IDisposable
    {
        private const int k_ReelCount = 3;

        private readonly ISpinResultProvider m_Provider;
        private readonly ISpinResultPersistence m_Persistence;
        private readonly ISlotView m_View;
        private readonly GamePipe m_Pipe;

        private ResolvedSpinResult m_CurrentResult;
        private bool m_IsSpinning;
        private int m_StoppedReelCount;

        public SpinController(ISpinResultProvider provider, ISpinResultPersistence persistence, ISlotView view, GamePipe pipe)
        {
            m_Provider = provider;
            m_Persistence = persistence;
            m_View = view;
            m_Pipe = pipe;
        }
        
        void IInitializable.Initialize()
        {
            m_Pipe.SubscribeTo<SpinRequestedMessage>(OnSpinRequested);
            m_Pipe.SubscribeTo<ReelStoppedMessage>(OnReelStopped);
        }

        void IDisposable.Dispose()
        {
            m_Pipe.UnsubscribeFrom<SpinRequestedMessage>(OnSpinRequested);
            m_Pipe.UnsubscribeFrom<ReelStoppedMessage>(OnReelStopped);
        }

        private void OnSpinRequested(ref SpinRequestedMessage message)
        {
            if (m_IsSpinning)
                return;

            m_IsSpinning = true;
            m_StoppedReelCount = 0;

            SpinResult spinResult = m_Provider.GetNext();
            m_CurrentResult = spinResult.Resolve();
            StopMode stopMode = DetermineStopMode(m_CurrentResult);

            m_Persistence.Save(m_Provider.Seed, m_Provider.CurrentIndex);
            m_View.StartSpin(m_CurrentResult, stopMode);
        }

        private void OnReelStopped(ref ReelStoppedMessage message)
        {
            m_StoppedReelCount++;

            if (m_StoppedReelCount < k_ReelCount)
                return;

            m_IsSpinning = false;
            bool isWin = IsTriple(m_CurrentResult);
            SpinCompletedMessage completedMessage = new(m_CurrentResult, isWin);
            m_Pipe.Raise(in completedMessage);
        }

        private static StopMode DetermineStopMode(ResolvedSpinResult result)
        {
            if (result.Left != result.Middle)
                return StopMode.Fast;

            if (result.Middle != result.Right)
                return StopMode.Normal;

            return StopMode.Slow;
        }

        private static bool IsTriple(ResolvedSpinResult result) =>
            result.Left == result.Middle && result.Middle == result.Right;
    }
}
