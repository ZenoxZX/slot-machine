using SlotMachine.DI.Core.Installers;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace SlotMachine.DI.Core.Scopes
{
    public sealed class ProjectLifetimeScope : LifetimeScope
    {
        [SerializeField] private MonoInstaller[] m_Installers;

        private static bool s_Initialized;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            if (s_Initialized)
                return;

            foreach (MonoInstaller installer in m_Installers)
                installer.Install(builder);

            s_Initialized = true;
        }
    }
}
