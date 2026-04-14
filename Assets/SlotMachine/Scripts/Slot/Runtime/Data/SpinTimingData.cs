using LitMotion;
using SlotMachine.ConfigManagement;
using SlotMachine.Core;
using SlotMachine.Slot.Core;
using UnityEngine;

namespace SlotMachine.Slot.Data
{
    [CreateAssetMenu(fileName = nameof(SpinTimingData), menuName = GlobalEnvironmentVariables.AppName + "/Slot/" + nameof(SpinTimingData))]
    public class SpinTimingData : ScriptableObject, IVisibleConfig
    {
        [Header("Spin Speed")]
        [SerializeField] private float m_SpinSpeed = 3000f;

        [Header("Ramp Up")]
        [SerializeField] private float m_RampUpDuration = 0.15f;
        [SerializeField] private Ease m_RampUpEase = Ease.OutQuad;

        [Header("Snap")]
        [SerializeField] private float m_SnapDuration = 0.1f;
        [SerializeField] private Ease m_SnapEase = Ease.OutBack;

        [Header("Reel Start Delay")]
        [SerializeField] private float m_ReelStartDelayStep = 0.06f;
        [SerializeField] private float m_ReelStartDelayJitter = 0.04f;

        [Header("Duration")]
        [SerializeField] private float m_BaseDuration = 0.9f;
        [SerializeField] private float m_NormalExtraDuration = 1.0f;
        [SerializeField] private float m_SlowExtraDuration = 2.25f;

        string IVisibleConfig.ConfigName => "Spin Timing";
        string IVisibleConfig.Category => "Slot";

        public float SpinSpeed => m_SpinSpeed;
        public float RampUpDuration => m_RampUpDuration;
        public Ease RampUpEase => m_RampUpEase;
        public float SnapDuration => m_SnapDuration;
        public Ease SnapEase => m_SnapEase;
        public float ReelStartDelayStep => m_ReelStartDelayStep;
        public float ReelStartDelayJitter => m_ReelStartDelayJitter;

        public float GetSpinDuration(StopMode mode) => mode switch
        {
            StopMode.Fast => m_BaseDuration,
            StopMode.Normal => m_BaseDuration + m_NormalExtraDuration,
            StopMode.Slow => m_BaseDuration + m_SlowExtraDuration,
            _ => m_BaseDuration
        };
    }
}
