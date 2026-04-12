using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SlotMachine.Slot.Core;

namespace SlotMachine.Slot.Data
{
    [UsedImplicitly]
    public class SpinResultProvider : ISpinResultProvider
    {
        private static readonly SpinResultEntry[] s_ResultTable = SpinResultTable.Entries;
        private const int k_PoolCapacity = SpinResultTable.PoolCapacity;

        private readonly SpinResult[] m_Pool = new SpinResult[k_PoolCapacity];
        private readonly int[] m_BlockCursors = new int[s_ResultTable.Length];
        private readonly int[] m_BlockStarts = new int[s_ResultTable.Length];
        private readonly int[] m_BlockEnds = new int[s_ResultTable.Length];
        private readonly List<int> m_Candidates;
        private Random m_Random;

        private int m_Seed;
        private int m_CurrentIndex;

        public SpinResultProvider() : this(Environment.TickCount) { }
        public SpinResultProvider(int seed) : this(seed, 0) { }
        public SpinResultProvider(int seed, int startIndex)
        {
            m_Seed = seed;
            m_Random = new(seed);
            m_Candidates = new(s_ResultTable.Length);
            GeneratePool();
            m_CurrentIndex = startIndex;
        }

        public int Seed => m_Seed;
        public int CurrentIndex => m_CurrentIndex;
        public int PoolSize => k_PoolCapacity;

        public SpinResult GetNext()
        {
            SpinResult result = m_Pool[m_CurrentIndex];
            m_CurrentIndex++;

            if (m_CurrentIndex < k_PoolCapacity)
                return result;

            m_Seed = m_Random.Next();
            m_Random = new Random(m_Seed);
            
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
