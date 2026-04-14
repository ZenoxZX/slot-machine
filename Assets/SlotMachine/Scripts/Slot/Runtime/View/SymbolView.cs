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

        public void Initialize(SymbolViewData data) => m_Data = data;

        public void SetSymbol(Symbol symbol)
        {
            m_NormalImage.sprite = m_Data.GetNormal(symbol);
            m_BlurImage.sprite = m_Data.GetBlur(symbol);
        }

        // @Aygun:
        // Before: Both normal and blur alpha were tweened simultaneously (crossfade).
        // This caused transparency during transition, revealing the background.
        // After: Normal image always stays visible (alpha 1), only blur alpha is tweened.
        // Both images share the same atlas so keeping normal enabled adds no extra draw call.
        // Blur image renders on top of normal, so when blur alpha = 1 the normal is fully occluded.
        public void SetBlur(bool value)
        {
            CancelMotion();

            float from = value ? 0f : 1f;
            float to = value ? 1f : 0f;

            m_MotionHandle = LMotion.Create(from, to, m_Data.BlurFadeDuration)
                .WithEase(Ease.Linear)
                .Bind(m_BlurImage, static (alpha, blur) => blur.color = new Color(1f, 1f, 1f, alpha));
        }

        private void OnDestroy() => CancelMotion();

        private void CancelMotion()
        {
            if (m_MotionHandle.IsActive())
                m_MotionHandle.Cancel();
        }
    }
}