using System.IO;
using UnityEngine;

namespace SlotMachine.Slot.Data
{
    public class SpinResultPersistence : ISpinResultPersistence
    {
        private const string k_FileName = "spin_state.bin";

        private readonly string m_FilePath = Path.Combine(Application.persistentDataPath, k_FileName);

        public void Save(int seed, int currentIndex)
        {
            using FileStream stream = new(m_FilePath, FileMode.Create, FileAccess.Write);
            using BinaryWriter writer = new(stream);
            writer.Write(seed);
            writer.Write(currentIndex);
        }

        public bool TryLoad(out int seed, out int currentIndex)
        {
            seed = 0;
            currentIndex = 0;

            if (!File.Exists(m_FilePath))
                return false;

            using FileStream stream = new(m_FilePath, FileMode.Open, FileAccess.Read);
            using BinaryReader reader = new(stream);
            seed = reader.ReadInt32();
            currentIndex = reader.ReadInt32();
            return true;
        }

        public void Clear()
        {
            if (File.Exists(m_FilePath))
                File.Delete(m_FilePath);
        }
    }
}
