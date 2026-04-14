using System;
using UnityEngine;

namespace SlotMachine.Slot.Data
{
    [Serializable]
    public class CoinVFXReference
    {
        [SerializeField] private RectTransform m_SpawnPoint;
        [SerializeField] private RectTransform m_PoolParent;

        public RectTransform SpawnPoint => m_SpawnPoint;
        public RectTransform PoolParent => m_PoolParent;
    }
}
