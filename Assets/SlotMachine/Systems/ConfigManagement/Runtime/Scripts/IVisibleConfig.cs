namespace SlotMachine.ConfigManagement
{
    /// <summary>
    /// Marker interface for ScriptableObject configs that should be visible in the Config Manager window.
    /// </summary>
    public interface IVisibleConfig
    {
        /// <summary>
        /// Display name for this config in the Config Manager.
        /// </summary>
        string ConfigName { get; }

        /// <summary>
        /// Category name for grouping configs (e.g., "Network", "Audio", "Graphics").
        /// </summary>
        string Category { get; }
    }
}
