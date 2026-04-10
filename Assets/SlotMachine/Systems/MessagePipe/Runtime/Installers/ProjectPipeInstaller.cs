using SlotMachine.MessagePipe.Pipes;
using VContainer;
using VContainer.Unity;

namespace SlotMachine.MessagePipe.Installers
{
    public class ProjectPipeInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<ProjectPipe>(Lifetime.Singleton);
        }
    }
}