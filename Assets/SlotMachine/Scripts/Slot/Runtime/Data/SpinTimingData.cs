using System;
using SlotMachine.Core;
using SlotMachine.Slot.Core;
using UnityEngine;

namespace SlotMachine.Slot.Data
{
    [CreateAssetMenu(fileName = nameof(SpinTimingData), menuName = GlobalEnvironmentVariables.AppName + "/Slot/" + nameof(SpinTimingData))]
    public class SpinTimingData : ScriptableObject
    {
        [Header("Reel Start Delay")]
        [SerializeField] private float m_ReelStartDelayStep = 0.06f;
        [SerializeField] private float m_ReelStartDelayJitter = 0.04f;

        [Header("Fast Spin Profile")]
        [SerializeField] private SpinProfile m_FastProfile;

        [Header("Normal Spin Profile")]
        [SerializeField] private SpinProfile m_NormalProfile;

        [Header("Slow Spin Profile")]
        [SerializeField] private SpinProfile m_SlowProfile;

        public float ReelStartDelayStep => m_ReelStartDelayStep;
        public float ReelStartDelayJitter => m_ReelStartDelayJitter;

        public SpinProfile GetProfile(StopMode mode) => mode switch
        {
            StopMode.Fast => m_FastProfile,
            StopMode.Normal => m_NormalProfile,
            StopMode.Slow => m_SlowProfile,
            _ => m_FastProfile
        };
    }

    [Serializable]
    public struct SpinProfile
    {
        [Tooltip("Total duration of the spin (ramp up + constant + slow down)")]
        [SerializeField] private float m_Duration;

        [Tooltip("Total number of full rotations (1 rotation = 4 cells)")]
        [SerializeField] private int m_Rotations;

        [Tooltip("Progress curve: X = normalized time (0-1), Y = normalized distance (0-1). Shape defines ramp up, constant speed, and slow down phases.")]
        [SerializeField] private AnimationCurve m_ProgressCurve;

        public float Duration => m_Duration;
        public int Rotations => m_Rotations;
        public AnimationCurve ProgressCurve => m_ProgressCurve;
    }
}
