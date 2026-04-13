using SlotMachine.Core;
using SlotMachine.Slot.Core;
using UnityEngine;

namespace SlotMachine.Slot.Data
{
    [CreateAssetMenu(fileName = nameof(SymbolViewData), menuName = GlobalEnvironmentVariables.AppName + "/Slot/" + nameof(SymbolViewData))]
    public class SymbolViewData : ScriptableObject
    {
        [Header("Settings")]
        [SerializeField] private float m_BlurFadeDuration = 0.15f;
        
        [Header("Normal Sprites")]
        [SerializeField] private Sprite m_A;
        [SerializeField] private Sprite m_Bonus;
        [SerializeField] private Sprite m_Seven;
        [SerializeField] private Sprite m_Wild;
        [SerializeField] private Sprite m_Jackpot;

        [Header("Blur Sprites")]
        [SerializeField] private Sprite m_ABlur;
        [SerializeField] private Sprite m_BonusBlur;
        [SerializeField] private Sprite m_SevenBlur;
        [SerializeField] private Sprite m_WildBlur;
        [SerializeField] private Sprite m_JackpotBlur;
        
        public float BlurFadeDuration => m_BlurFadeDuration;

        public Sprite GetNormal(Symbol symbol) => symbol switch
        {
            Symbol.A => m_A,
            Symbol.Bonus => m_Bonus,
            Symbol.Seven => m_Seven,
            Symbol.Wild => m_Wild,
            Symbol.Jackpot => m_Jackpot,
            _ => m_A
        };
        
        public Sprite GetBlur(Symbol symbol) => symbol switch
        {
            Symbol.A => m_ABlur,
            Symbol.Bonus => m_BonusBlur,
            Symbol.Seven => m_SevenBlur,
            Symbol.Wild => m_WildBlur,
            Symbol.Jackpot => m_JackpotBlur,
            _ => m_ABlur
        };
    }
}