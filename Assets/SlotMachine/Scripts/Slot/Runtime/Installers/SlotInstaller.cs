using System;
using SlotMachine.Slot.Controller;
using SlotMachine.Slot.Data;
using SlotMachine.Slot.View;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace SlotMachine.Slot.Installers
{
    [Serializable]
    public class SlotInstaller : IInstaller
    {
        [SerializeField] private SlotView m_SlotView;
        [SerializeField] private SpinButtonView m_SpinButtonView;
        [SerializeField] private SymbolViewData m_SymbolViewData;
        [SerializeField] private ReelContainer m_ReelContainer;

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<SpinController>().AsSelf();
            builder.RegisterEntryPoint<SpinResultProvider>();
            builder.RegisterEntryPoint<SpinResultPersistence>();

            builder.RegisterComponent<ISlotView>(m_SlotView);
            builder.RegisterComponent<SpinButtonView>(m_SpinButtonView);

            builder.RegisterInstance(m_SymbolViewData).AsSelf();
            builder.RegisterInstance(m_ReelContainer).AsSelf();
        }
    }
}