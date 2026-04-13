using LitMotion;
using SlotMachine.Slot.Core;
using SlotMachine.Slot.Data;
using UnityEngine;
using UnityEngine.UI;

namespace SlotMachine.Slot.View
{
    public class SymbolView : MonoBehaviour
    {
        [SerializeField] private Image m_NormalImage;
        [SerializeField] private Image m_BlurImage;

        private SymbolViewData m_Data;
        private MotionHandle m_MotionHandle;
        private bool m_IsBlur;

        public void Initialize(SymbolViewData data) => m_Data = data;

        public void SetSymbol(Symbol symbol)
        {
            m_NormalImage.sprite = m_Data.GetNormal(symbol);
            m_BlurImage.sprite = m_Data.GetBlur(symbol);
        }

        public void SetBlur(bool value)
        {
            CancelMotion();
            m_IsBlur = value;

            m_MotionHandle = LMotion.Create(0f, 1f, m_Data.BlurFadeDuration)
                .WithEase(Ease.Linear)
                .Bind
                (
                    this,
                    static (t, self) =>
                    {
                        Image normal = self.m_NormalImage;
                        Image blur = self.m_BlurImage;
                        float normalAlpha = self.m_IsBlur ? 1f - t : t;
                        float blurAlpha = self.m_IsBlur ? t : 1f - t;
                        normal.color = new Color(1f, 1f, 1f, normalAlpha);
                        blur.color = new Color(1f, 1f, 1f, blurAlpha);
                    }
                );
        }

        private void OnDestroy() => CancelMotion();

        private void CancelMotion()
        {
            if (m_MotionHandle.IsActive())
                m_MotionHandle.Cancel();
        }
    }
}