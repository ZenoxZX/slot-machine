using System;
using SlotMachine.Slot.View;
using UnityEngine;

namespace SlotMachine.Slot.Data
{
    [Serializable]
    public class ReelReference
    {
        [SerializeField] private ReelView m_LeftReel;
        [SerializeField] private ReelView m_MiddleReel;
        [SerializeField] private ReelView m_RightReel;
        
        public ReelView LeftReel => m_LeftReel;
        public ReelView MiddleReel => m_MiddleReel;
        public ReelView RightReel => m_RightReel;
    }
}