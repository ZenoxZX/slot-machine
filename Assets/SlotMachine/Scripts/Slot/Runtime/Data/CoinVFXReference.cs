using System;
using UnityEngine;

namespace SlotMachine.Slot.Data
{
    [Serializable]
    public class CoinVFXReference
    {
        [SerializeField] private Transform m_SpawnPoint;
        [SerializeField] private Transform m_PoolParent;
        [SerializeField] private Camera m_Camera;

        public Transform SpawnPoint => m_SpawnPoint;
        public Transform PoolParent => m_PoolParent;
        public Camera Camera => m_Camera;
    }
}
