using System;
using System.Collections.Generic;
using SlotMachine.Slot.Core;

namespace SlotMachine.Slot.Data
{
    public class SpinResultProvider : ISpinResultProvider
    {
        private const int k_PoolCapacity = 100;

        private static readonly SpinResultEntry[] s_ResultTable =
        {
            new(SpinResult.Jackpot_Jackpot_Jackpot, 5),
            new(SpinResult.Wild_Wild_Wild, 6),
            new(SpinResult.Seven_Seven_Seven, 7),
            new(SpinResult.Bonus_Bonus_Bonus, 8),
            new(SpinResult.A_A_A, 9),
            new(SpinResult.A_Wild_Bonus, 13),
            new(SpinResult.Wild_Wild_Seven, 13),
            new(SpinResult.Jackpot_Jackpot_A, 13),
            new(SpinResult.Wild_Bonus_A, 13),
            new(SpinResult.Bonus_A_Jackpot, 13),
        };

        private readonly SpinResult[] m_Pool = new SpinResult[k_PoolCapacity];
        private readonly bool[] m_Occupied = new bool[k_PoolCapacity];
        private readonly List<int> m_EmptySlots;
        private readonly Random m_Random;

        private bool m_IsPoolGenerated;
        private int m_CurrentIndex;

        public SpinResultProvider() : this(Environment.TickCount) { }
        public SpinResultProvider(int seed)
        {
            m_Random = new(seed);
            m_EmptySlots = new(k_PoolCapacity);
            GeneratePool();
        }

        public int CurrentIndex => m_CurrentIndex;
        public int PoolSize => k_PoolCapacity;

        public SpinResult GetNext()
        {
            SpinResult result = m_Pool[m_CurrentIndex];
            m_CurrentIndex++;

            if (m_CurrentIndex < k_PoolCapacity)
                return result;

            GeneratePool();
            m_CurrentIndex = 0;
            return result;
        }

        private void GeneratePool()
        {
            if (m_IsPoolGenerated)
                ClearCollection();

            foreach (SpinResultEntry entry in s_ResultTable)
                PlaceWithPeriod(entry.Result, entry.Count);

            if (!m_IsPoolGenerated)
                m_IsPoolGenerated = true;
        }

        private void ClearCollection()
        {
            for (int i = 0; i < k_PoolCapacity; i++)
            {
                m_Pool[i] = default;
                m_Occupied[i] = false;
            }
        }

        private void PlaceWithPeriod(SpinResult result, int count)
        {
            int baseBlockSize = k_PoolCapacity / count;
            int remainder = k_PoolCapacity % count;
            int blockStart = 0;

            for (int i = 0; i < count; i++)
            {
                int blockSize = baseBlockSize + (i < remainder ? 1 : 0);
                int blockEnd = blockStart + blockSize;
                m_EmptySlots.Clear();

                for (int j = blockStart; j < blockEnd; j++)
                {
                    if (!m_Occupied[j])
                        m_EmptySlots.Add(j);
                }

                int chosen;

                if (m_EmptySlots.Count > 0)
                {
                    chosen = m_EmptySlots[m_Random.Next(m_EmptySlots.Count)];
                }
                else
                {
                    chosen = FindNearestEmpty(blockStart, blockEnd);
                }

                m_Pool[chosen] = result;
                m_Occupied[chosen] = true;
                blockStart = blockEnd;
            }
        }

        private int FindNearestEmpty(int blockStart, int blockEnd)
        {
            int mid = (blockStart + blockEnd) / 2;

            for (int offset = 1; offset < k_PoolCapacity; offset++)
            {
                int forward = mid + offset;

                if (forward < k_PoolCapacity && !m_Occupied[forward])
                    return forward;

                int backward = mid - offset;

                if (backward >= 0 && !m_Occupied[backward])
                    return backward;
            }

            return -1;
        }
    }
}
