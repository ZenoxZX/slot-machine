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
            new(SpinResult.A_Wild_Bonus, 13),
            new(SpinResult.Wild_Wild_Seven, 13),
            new(SpinResult.Jackpot_Jackpot_A, 13),
            new(SpinResult.Wild_Bonus_A, 13),
            new(SpinResult.Bonus_A_Jackpot, 13),
            new(SpinResult.A_A_A, 9),
            new(SpinResult.Bonus_Bonus_Bonus, 8),
            new(SpinResult.Seven_Seven_Seven, 7),
            new(SpinResult.Wild_Wild_Wild, 6),
            new(SpinResult.Jackpot_Jackpot_Jackpot, 5),
        };

        private readonly SpinResult[] m_Pool = new SpinResult[k_PoolCapacity];
        private readonly int[] m_BlockCursors = new int[s_ResultTable.Length];
        private readonly int[] m_BlockStarts = new int[s_ResultTable.Length];
        private readonly int[] m_BlockEnds = new int[s_ResultTable.Length];
        private readonly List<int> m_Candidates;
        private readonly Random m_Random;

        private int m_CurrentIndex;

        public SpinResultProvider() : this(Environment.TickCount) { }
        public SpinResultProvider(int seed)
        {
            m_Random = new(seed);
            m_Candidates = new(s_ResultTable.Length);
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
            for (int r = 0; r < s_ResultTable.Length; r++)
            {
                m_BlockCursors[r] = 0;
                ComputeBlockRange(r);
            }

            for (int slot = 0; slot < k_PoolCapacity; slot++)
            {
                m_Candidates.Clear();
                int minRemaining = int.MaxValue;

                for (int r = 0; r < s_ResultTable.Length; r++)
                {
                    if (m_BlockCursors[r] >= s_ResultTable[r].Count)
                        continue;

                    if (slot < m_BlockStarts[r] || slot >= m_BlockEnds[r])
                        continue;

                    int remaining = m_BlockEnds[r] - slot;

                    if (remaining < minRemaining)
                    {
                        minRemaining = remaining;
                        m_Candidates.Clear();
                        m_Candidates.Add(r);
                    }
                    else if (remaining == minRemaining)
                    {
                        m_Candidates.Add(r);
                    }
                }

                if (m_Candidates.Count == 0)
                    continue;

                int chosen = m_Candidates[m_Random.Next(m_Candidates.Count)];
                m_Pool[slot] = s_ResultTable[chosen].Result;
                m_BlockCursors[chosen]++;

                if (m_BlockCursors[chosen] < s_ResultTable[chosen].Count)
                    ComputeBlockRange(chosen);
            }
        }

        private void ComputeBlockRange(int resultIndex)
        {
            int count = s_ResultTable[resultIndex].Count;
            int cursor = m_BlockCursors[resultIndex];
            int baseBlockSize = k_PoolCapacity / count;
            int remainder = k_PoolCapacity % count;
            m_BlockStarts[resultIndex] = cursor * baseBlockSize + Math.Min(cursor, remainder);
            m_BlockEnds[resultIndex] = m_BlockStarts[resultIndex] + baseBlockSize + (cursor < remainder ? 1 : 0);
        }
    }
}
