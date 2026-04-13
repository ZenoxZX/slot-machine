using Cysharp.Threading.Tasks;
using SlotMachine.MessagePipe.Pipes;
using SlotMachine.Slot.Core;
using SlotMachine.Slot.Data;
using UnityEngine;
using VContainer;

namespace SlotMachine.Slot.View
{
    public class SlotView : MonoBehaviour, ISlotView
    {
        [Inject] private ReelContainer m_ReelContainer;
        [Inject] private SpinTimingData m_TimingData;
        [Inject] private SymbolViewData m_SymbolData;
        [Inject] private GamePipe m_Pipe;

        private ReelView m_Left;
        private ReelView m_Middle;
        private ReelView m_Right;
        private ReelView[] m_Reels;

        private void Start()
        {
            m_Left = m_ReelContainer.LeftReel;
            m_Middle = m_ReelContainer.MiddleReel;
            m_Right = m_ReelContainer.RightReel;
            m_Reels = new[] { m_Left, m_Middle, m_Right };

            for (int i = 0; i < m_Reels.Length; i++)
                m_Reels[i].Initialize(m_TimingData, m_SymbolData, m_Pipe);
        }

        private void Update()
        {
            if (m_Reels == null)
                return;

            float dt = Time.deltaTime;

            for (int i = 0; i < m_Reels.Length; i++)
                m_Reels[i].Tick(dt);
        }

        async void ISlotView.StartSpin(ResolvedSpinResult targetSymbols, StopMode stopMode)
        {
            m_Left.StartSpin(targetSymbols.Left, StopMode.Fast);

            await DelayReelStart();

            m_Middle.StartSpin(targetSymbols.Middle, StopMode.Fast);

            await DelayReelStart();

            m_Right.StartSpin(targetSymbols.Right, stopMode);
        }

        private UniTask DelayReelStart() =>
            Delay(m_TimingData.ReelStartDelayStep + Random.Range(0f, m_TimingData.ReelStartDelayJitter));

        private UniTask Delay(float seconds) =>
            UniTask.Delay(System.TimeSpan.FromSeconds(seconds), cancellationToken: destroyCancellationToken);
    }
}
