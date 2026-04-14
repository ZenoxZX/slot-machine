using LitMotion;
using SlotMachine.MessagePipe.Pipes;
using SlotMachine.Slot.Core;
using SlotMachine.Slot.Data;
using SlotMachine.Slot.Messages;
using UnityEngine;

namespace SlotMachine.Slot.View
{
    public enum ReelState
    {
        Idle,
        Spinning,
        Snapping
    }

    public class ReelView : MonoBehaviour
    {
        private const int k_SlotCount = 5;
        private const int k_SymbolCount = 5;

        [SerializeField] private SymbolView[] m_Symbols;
        [SerializeField] private float m_CellHeight = 150f;
        [SerializeField] private int m_ReelIndex;

        private RectTransform[] m_SymbolTransforms;
        private float m_WrapThreshold;
        private float m_StripHeight;

        private SpinTimingData m_TimingData;
        private GamePipe m_Pipe;

        private ReelState m_State;
        private float m_CurrentSpeed;
        private float m_SpinTimer;
        private float m_SpinDuration;
        private Symbol m_TargetSymbol;

        private MotionHandle m_RampUpHandle;
        private MotionHandle m_SnapHandle;

        public ReelState State => m_State;

        public void Initialize(SpinTimingData timingData, SymbolViewData symbolData, GamePipe pipe)
        {
            m_TimingData = timingData;
            m_Pipe = pipe;

            m_SymbolTransforms = new RectTransform[k_SlotCount];

            for (int i = 0; i < k_SlotCount; i++)
            {
                m_Symbols[i].Initialize(symbolData);
                m_SymbolTransforms[i] = m_Symbols[i].GetComponent<RectTransform>();
            }

            m_StripHeight = m_CellHeight * k_SlotCount;
            m_WrapThreshold = -m_CellHeight * 2f;

            AssignRandomSymbols();
        }

        public void StartSpin(Symbol targetSymbol, StopMode stopMode)
        {
            if (m_State != ReelState.Idle)
                return;

            m_TargetSymbol = targetSymbol;
            m_SpinDuration = m_TimingData.GetSpinDuration(stopMode);
            m_SpinTimer = 0f;
            m_CurrentSpeed = 0f;
            m_State = ReelState.Spinning;

            SetAllBlur(true);
            CancelHandles();

            m_RampUpHandle = LMotion.Create(0f, m_TimingData.SpinSpeed, m_TimingData.RampUpDuration)
                .WithEase(m_TimingData.RampUpEase)
                .Bind(this, static (speed, self) => self.m_CurrentSpeed = speed);
        }

        public void Tick(float deltaTime)
        {
            if (m_State != ReelState.Spinning)
                return;

            m_SpinTimer += deltaTime;

            if (m_SpinTimer >= m_SpinDuration)
            {
                BeginSnap();
                return;
            }

            float delta = m_CurrentSpeed * deltaTime;

            if (delta <= 0f)
                return;

            for (int i = 0; i < k_SlotCount; i++)
            {
                RectTransform rect = m_SymbolTransforms[i];
                Vector2 pos = rect.anchoredPosition;
                pos.y -= delta;

                if (pos.y <= m_WrapThreshold)
                {
                    pos.y += m_StripHeight;
                    m_Symbols[i].SetSymbol(GetRandomSymbol());
                }

                rect.anchoredPosition = pos;
            }
        }

        private void BeginSnap()
        {
            m_CurrentSpeed = 0f;
            m_State = ReelState.Snapping;

            SetAllBlur(false);
            CancelHandles();

            // Place symbols on grid, one cellHeight above final position
            PlaceSymbolsForSnap();

            // Tween all symbols down by 1 cellHeight
            m_SnapHandle = LMotion.Create(m_CellHeight, 0f, m_TimingData.SnapDuration)
                .WithEase(m_TimingData.SnapEase)
                .WithOnComplete(OnSnapComplete)
                .Bind(this, static (offset, self) =>
                {
                    for (int i = 0; i < k_SlotCount; i++)
                    {
                        float baseY = self.GetGridY(i);
                        self.m_SymbolTransforms[i].anchoredPosition = new Vector2(
                            self.m_SymbolTransforms[i].anchoredPosition.x,
                            baseY + offset);
                    }
                });
        }

        private void PlaceSymbolsForSnap()
        {
            // Index 0 = top buffer (+300), Index 2 = center (0), Index 4 = bottom buffer (-300)
            // Target goes to center (index 2), rest random
            m_Symbols[2].SetSymbol(m_TargetSymbol);
            m_Symbols[0].SetSymbol(GetRandomSymbol());
            m_Symbols[1].SetSymbol(GetRandomSymbol());
            m_Symbols[3].SetSymbol(GetRandomSymbol());
            m_Symbols[4].SetSymbol(GetRandomSymbol());

            // Position all at grid + 1 cellHeight offset (will tween down)
            for (int i = 0; i < k_SlotCount; i++)
            {
                float y = GetGridY(i) + m_CellHeight;
                m_SymbolTransforms[i].anchoredPosition = new Vector2(
                    m_SymbolTransforms[i].anchoredPosition.x, y);
            }
        }

        private float GetGridY(int index) => (2 - index) * m_CellHeight;

        private void OnSnapComplete()
        {
            m_State = ReelState.Idle;

            ReelStoppedMessage message = new(m_ReelIndex);
            m_Pipe.Raise(in message);
        }

        private void SetAllBlur(bool isBlur)
        {
            for (int i = 0; i < k_SlotCount; i++)
                m_Symbols[i].SetBlur(isBlur);
        }

        private void AssignRandomSymbols()
        {
            for (int i = 0; i < k_SlotCount; i++)
                m_Symbols[i].SetSymbol(GetRandomSymbol());
        }

        private static Symbol GetRandomSymbol() => (Symbol)Random.Range(0, k_SymbolCount);

        private void OnDestroy() => CancelHandles();

        private void CancelHandles()
        {
            if (m_RampUpHandle.IsActive())
                m_RampUpHandle.Cancel();

            if (m_SnapHandle.IsActive())
                m_SnapHandle.Cancel();
        }
    }
}
