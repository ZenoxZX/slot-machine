using SlotMachine.ConfigManagement;
using SlotMachine.Core;
using SlotMachine.Slot.Core;
using SlotMachine.Slot.View;
using UnityEngine;

namespace SlotMachine.Slot.Data
{
    [CreateAssetMenu(fileName = nameof(CoinVFXData), menuName = GlobalEnvironmentVariables.AppName + "/Slot/" + nameof(CoinVFXData))]
    public class CoinVFXData : ScriptableObject, IVisibleConfig
    {
        [Header("Coin Counts")]
        [SerializeField] private int m_JackpotCount = 30;
        [SerializeField] private int m_WildCount = 25;
        [SerializeField] private int m_SevenCount = 20;
        [SerializeField] private int m_BonusCount = 15;
        [SerializeField] private int m_ACount = 10;

        [Header("Timing")]
        [SerializeField] private float m_Duration = 4f;
        [SerializeField] private float m_SpawnInterval = 0.02f;
        [SerializeField] private float m_FadeOutDuration = 0.2f;

        [Header("Motion")]
        [SerializeField] private float m_InitialSpeedMin = 600f;
        [SerializeField] private float m_InitialSpeedMax = 1000f;
        [SerializeField] private float m_AngleMin = 50f;
        [SerializeField] private float m_AngleMax = 130f;
        [SerializeField] private float m_Gravity = 800f;

        [Header("Scale (Perspective)")]
        [SerializeField] private float m_StartScale = 0.3f;
        [SerializeField] private float m_EndScale = 1.2f;

        [Header("Rotation")]
        [SerializeField] private float m_RotationSpeedMin = -360f;
        [SerializeField] private float m_RotationSpeedMax = 360f;

        [Header("Prefab")]
        [SerializeField] private CoinView m_CoinPrefab;
        [SerializeField] private int m_PoolSize = 30;

        [Header("Animation")]
        [SerializeField] private float m_FrameRate = 30f;
        [SerializeField] private Sprite[] m_CoinFrames;

        string IVisibleConfig.ConfigName => "Coin VFX";
        string IVisibleConfig.Category => "Slot";

        public float Duration => m_Duration;
        public float SpawnInterval => m_SpawnInterval;
        public float FadeOutDuration => m_FadeOutDuration;
        public float InitialSpeedMin => m_InitialSpeedMin;
        public float InitialSpeedMax => m_InitialSpeedMax;
        public float AngleMin => m_AngleMin;
        public float AngleMax => m_AngleMax;
        public float Gravity => m_Gravity;
        public float StartScale => m_StartScale;
        public float EndScale => m_EndScale;
        public float RotationSpeedMin => m_RotationSpeedMin;
        public float RotationSpeedMax => m_RotationSpeedMax;
        public CoinView CoinPrefab => m_CoinPrefab;
        public int PoolSize => m_PoolSize;
        public float FrameRate => m_FrameRate;
        public Sprite[] CoinFrames => m_CoinFrames;

        public int GetCoinCount(Symbol symbol) => symbol switch
        {
            Symbol.Jackpot => m_JackpotCount,
            Symbol.Wild => m_WildCount,
            Symbol.Seven => m_SevenCount,
            Symbol.Bonus => m_BonusCount,
            Symbol.A => m_ACount,
            _ => m_ACount
        };
    }
}
