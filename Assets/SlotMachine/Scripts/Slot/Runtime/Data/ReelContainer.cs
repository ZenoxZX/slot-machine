using System;
using System.Collections.Generic;
using SlotMachine.Slot.View;
using UnityEngine;

namespace SlotMachine.Slot.Data
{
    [Serializable]
    public class ReelContainer
    {
        [SerializeField] private ReelView m_LeftReel;
        [SerializeField] private ReelView m_MiddleReel;
        [SerializeField] private ReelView m_RightReel;
        
        public ReelView LeftReel => m_LeftReel;
        public ReelView MiddleReel => m_MiddleReel;
        public ReelView RightReel => m_RightReel;
        
        public IEnumerable<ReelView> GetReels()
        {
            yield return m_LeftReel;
            yield return m_MiddleReel;
            yield return m_RightReel;
        }
    }
}