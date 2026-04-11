using System;
using SlotMachine.Slot.Controller;
using SlotMachine.Slot.Data;
using VContainer;
using VContainer.Unity;

namespace SlotMachine.Slot.Installers
{
    [Serializable]
    public class SlotInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<SpinController>().AsSelf();
            builder.RegisterEntryPoint<SpinResultProvider>();
            builder.RegisterEntryPoint<SpinResultPersistence>();
        }
    }
}