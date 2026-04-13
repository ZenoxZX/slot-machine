using LitMotion;
using SlotMachine.MessagePipe.Pipes;
using SlotMachine.Slot.Core;
using SlotMachine.Slot.Data;
using SlotMachine.Slot.Messages;
using UnityEngine;

namespace SlotMachine.Slot.View
{
    public class ReelView : MonoBehaviour
    {
        private const int k_SlotCount = 4;
        private const int k_SymbolCount = 5;

        [SerializeField] private SymbolView[] m_Symbols;
        [SerializeField] private float m_CellHeight = 150f;
        [SerializeField] private int m_ReelIndex;

        private RectTransform[] m_SymbolTransforms;
        private float m_StripHeight;
        private float m_TopY;

        private SpinTimingData m_TimingData;
        private GamePipe m_Pipe;

        private bool m_IsSpinning;
        private float m_Progress;
        private float m_TotalDistance;
        private float m_PrevDistance;
        private SpinProfile m_Profile;
        private Symbol m_TargetSymbol;
        private bool m_BlurActive;

        private MotionHandle m_ProgressHandle;

        // Pre-computed symbol sequence for the entire spin
        private Symbol[] m_SymbolSequence;
        private int m_NextSymbolIndex;

        public bool IsSpinning => m_IsSpinning;

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
            m_TopY = m_CellHeight * 1.5f;

            AssignRandomSymbols();
        }

        public void StartSpin(Symbol targetSymbol, StopMode stopMode)
        {
            if (m_IsSpinning)
                return;

            m_TargetSymbol = targetSymbol;
            m_Profile = m_TimingData.GetProfile(stopMode);
            m_TotalDistance = m_Profile.Rotations * m_StripHeight;
            m_Progress = 0f;
            m_PrevDistance = 0f;
            m_IsSpinning = true;
            m_NextSymbolIndex = 0;

            BuildSymbolSequence();
            SetAllBlur(true);
            CancelProgress();

            m_ProgressHandle = LMotion.Create(0f, 1f, m_Profile.Duration)
                .WithEase(m_Profile.ProgressCurve)
                .WithOnComplete(OnSpinComplete)
                .Bind(this, static (p, self) => self.m_Progress = p);
        }

        public void Tick(float deltaTime)
        {
            if (!m_IsSpinning)
                return;

            float currentDistance = m_Progress * m_TotalDistance;
            float delta = currentDistance - m_PrevDistance;
            m_PrevDistance = currentDistance;

            if (delta <= 0f)
                return;

            // Blur transition: switch to normal when in slow down phase (last ~20% of progress)
            if (m_BlurActive && m_Progress > 0.8f)
            {
                SetAllBlur(false);
                m_BlurActive = false;
            }

            for (int i = 0; i < k_SlotCount; i++)
            {
                RectTransform rect = m_SymbolTransforms[i];
                Vector2 pos = rect.anchoredPosition;
                pos.y -= delta;

                if (pos.y <= -m_CellHeight * 2f)
                {
                    pos.y += m_StripHeight;

                    if (m_NextSymbolIndex < m_SymbolSequence.Length)
                        m_Symbols[i].SetSymbol(m_SymbolSequence[m_NextSymbolIndex++]);
                    else
                        m_Symbols[i].SetSymbol(GetRandomSymbol());
                }

                rect.anchoredPosition = pos;
            }
        }

        private void BuildSymbolSequence()
        {
            // Total wraps across all 4 slots during entire spin
            int totalWraps = m_Profile.Rotations * k_SlotCount;
            m_SymbolSequence = new Symbol[totalWraps];

            // Fill with random symbols, place target near the end
            // Target needs to land on center row (index 1 from top after sort)
            // Last few symbols before the end determine what's visible when stopped
            // We place target at totalWraps - 3 so it ends up in center row
            int targetIndex = totalWraps - 3;

            for (int i = 0; i < totalWraps; i++)
            {
                if (i == targetIndex)
                    m_SymbolSequence[i] = m_TargetSymbol;
                else
                    m_SymbolSequence[i] = GetRandomSymbol();
            }
        }

        private void OnSpinComplete()
        {
            m_IsSpinning = false;
            m_BlurActive = false;

            SnapToGrid();
            SetAllBlur(false);

            ReelStoppedMessage message = new(m_ReelIndex);
            m_Pipe.Raise(in message);
        }

        private void SnapToGrid()
        {
            SortSymbolsByY();

            for (int i = 0; i < k_SlotCount; i++)
            {
                float y = m_TopY - i * m_CellHeight;
                m_SymbolTransforms[i].anchoredPosition = new Vector2(
                    m_SymbolTransforms[i].anchoredPosition.x, y);
            }
        }

        private void SortSymbolsByY()
        {
            for (int i = 1; i < k_SlotCount; i++)
            {
                RectTransform keyRect = m_SymbolTransforms[i];
                SymbolView keySymbol = m_Symbols[i];
                float keyY = keyRect.anchoredPosition.y;
                int j = i - 1;

                while (j >= 0 && m_SymbolTransforms[j].anchoredPosition.y < keyY)
                {
                    m_SymbolTransforms[j + 1] = m_SymbolTransforms[j];
                    m_Symbols[j + 1] = m_Symbols[j];
                    j--;
                }

                m_SymbolTransforms[j + 1] = keyRect;
                m_Symbols[j + 1] = keySymbol;
            }
        }

        private void SetAllBlur(bool isBlur)
        {
            m_BlurActive = isBlur;

            for (int i = 0; i < k_SlotCount; i++)
            {
                m_Symbols[i].SetBlur(isBlur);
            }
        }

        private void AssignRandomSymbols()
        {
            for (int i = 0; i < k_SlotCount; i++)
            {
                m_Symbols[i].SetSymbol(GetRandomSymbol());
                m_Symbols[i].SetBlur(false);
            }
        }

        private static Symbol GetRandomSymbol() => (Symbol)Random.Range(0, k_SymbolCount);

        private void OnDestroy() => CancelProgress();

        private void CancelProgress()
        {
            if (m_ProgressHandle.IsActive())
                m_ProgressHandle.Cancel();
        }
    }
}
