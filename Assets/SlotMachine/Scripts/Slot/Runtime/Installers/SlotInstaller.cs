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
        [SerializeField] private SpinTimingData m_SpinTimingData;
        [SerializeField] private CoinVFXData m_CoinVFXData;
        [SerializeField] private CoinVFXReference m_CoinVFXReference;
        [SerializeField] private ReelReference m_ReelReference;

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<SpinController>().AsSelf();
            builder.Register<SpinResultProvider>(Lifetime.Singleton).As<ISpinResultProvider>();
            builder.Register<SpinResultPersistence>(Lifetime.Singleton).As<ISpinResultPersistence>();

            builder.RegisterComponent<ISlotView>(m_SlotView);
            builder.RegisterComponent(m_SpinButtonView);

            builder.RegisterInstance(m_SymbolViewData).AsSelf();
            builder.RegisterInstance(m_SpinTimingData).AsSelf();
            builder.RegisterInstance(m_CoinVFXData).AsSelf();
            builder.RegisterInstance(m_CoinVFXReference).AsSelf();
            builder.RegisterInstance(m_ReelReference).AsSelf();

            builder.RegisterEntryPoint<CoinVFXController>();
        }
    }
}