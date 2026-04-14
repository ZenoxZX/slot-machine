using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SlotMachine.MessagePipe.Pipes;
using SlotMachine.Slot.Data;
using SlotMachine.Slot.Messages;
using UnityEngine;
using VContainer.Unity;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace SlotMachine.Slot.View
{
    [UsedImplicitly]
    public class CoinVFXController : IInitializable, IDisposable, ITickable
    {
        private readonly CoinVFXData m_Data;
        private readonly CoinVFXReference m_Reference;
        private readonly GamePipe m_Pipe;

        private readonly List<CoinView> m_Pool = new();
        private readonly List<CoinView> m_ActiveCoins = new();

        private int m_CoinsToSpawn;
        private float m_SpawnTimer;
        private bool m_IsSpawning;

        public CoinVFXController(CoinVFXData data, CoinVFXReference reference, GamePipe pipe)
        {
            m_Data = data;
            m_Reference = reference;
            m_Pipe = pipe;
        }

        void IInitializable.Initialize()
        {
            m_Pipe.SubscribeTo<SpinCompletedMessage>(OnSpinCompleted);
            WarmPool();
        }

        void IDisposable.Dispose()
        {
            m_Pipe.UnsubscribeFrom<SpinCompletedMessage>(OnSpinCompleted);
        }

        void ITickable.Tick()
        {
            float dt = Time.deltaTime;

            if (m_IsSpawning)
                UpdateSpawning(dt);

            for (int i = m_ActiveCoins.Count - 1; i >= 0; i--)
            {
                CoinView coin = m_ActiveCoins[i];
                coin.Tick(dt);

                if (!coin.IsActive)
                    m_ActiveCoins.RemoveAt(i);
            }
        }

        private void OnSpinCompleted(ref SpinCompletedMessage message)
        {
            if (!message.IsWin)
                return;

            int count = m_Data.GetCoinCount(message.Result.Left);
            BeginBurst(count);
        }

        private void BeginBurst(int count)
        {
            m_CoinsToSpawn = count;
            m_SpawnTimer = 0f;
            m_IsSpawning = true;
        }

        private void UpdateSpawning(float deltaTime)
        {
            m_SpawnTimer += deltaTime;

            while (m_SpawnTimer >= m_Data.SpawnInterval && m_CoinsToSpawn > 0)
            {
                m_SpawnTimer -= m_Data.SpawnInterval;
                m_CoinsToSpawn--;
                SpawnCoin();
            }

            if (m_CoinsToSpawn <= 0)
                m_IsSpawning = false;
        }

        private void SpawnCoin()
        {
            CoinView coin = GetFromPool();

            if (coin == null)
                return;

            float angle = Random.Range(m_Data.AngleMin, m_Data.AngleMax);
            float speed = Random.Range(m_Data.InitialSpeedMin, m_Data.InitialSpeedMax);
            Vector2 spawnPos = m_Reference.SpawnPoint.anchoredPosition;

            coin.Launch(spawnPos, angle, speed, m_Data.Duration);
            m_ActiveCoins.Add(coin);
        }

        private void WarmPool()
        {
            for (int i = 0; i < m_Data.PoolSize; i++)
            {
                CoinView coin = Object.Instantiate(m_Data.CoinPrefab, m_Reference.PoolParent);
                coin.Initialize(m_Data);
                coin.Deactivate();
                m_Pool.Add(coin);
            }
        }

        private CoinView GetFromPool()
        {
            for (int i = 0; i < m_Pool.Count; i++)
            {
                if (!m_Pool[i].IsActive)
                    return m_Pool[i];
            }

            CoinView coin = Object.Instantiate(m_Data.CoinPrefab, m_Reference.PoolParent);
            coin.Initialize(m_Data);
            coin.Deactivate();
            m_Pool.Add(coin);
            return coin;
        }
    }
}
