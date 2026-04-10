using System.Collections.Generic;
using SlotMachine.DI.Core.Installers;
using SlotMachine.MessagePipe.Installers;
using VContainer;
using VContainer.Unity;

namespace DI.Installers
{
    public class GameSceneInstaller : MonoInstaller
    {
        
        public override void Install(IContainerBuilder builder)
        {
            foreach (IInstaller i in FetchInstallers())
                i.Install(builder);
        }
        
        private IEnumerable<IInstaller> FetchInstallers()
        {
            yield return new GamePipeInstaller();
        }
    }
}
