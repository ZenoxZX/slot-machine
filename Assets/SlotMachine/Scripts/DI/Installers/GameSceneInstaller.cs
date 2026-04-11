using System.Collections.Generic;
using SlotMachine.DI.Core.Installers;
using SlotMachine.MessagePipe.Installers;
using SlotMachine.Slot.Installers;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DI.Installers
{
    public class GameSceneInstaller : MonoInstaller
    {
        [SerializeField] private SlotInstaller m_SlotInstaller;
        
        public override void Install(IContainerBuilder builder)
        {
            foreach (IInstaller i in FetchInstallers())
                i.Install(builder);
        }
        
        private IEnumerable<IInstaller> FetchInstallers()
        {
            yield return new GamePipeInstaller();
            yield return m_SlotInstaller;
        }
    }
}
