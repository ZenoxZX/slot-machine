namespace SlotMachine.Slot.Data
{
    public interface ISpinResultPersistence
    {
        void Save(int seed, int currentIndex);
        bool TryLoad(out int seed, out int currentIndex);
        void Clear();
    }
}
