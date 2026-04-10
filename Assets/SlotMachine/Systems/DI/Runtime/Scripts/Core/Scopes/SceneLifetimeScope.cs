using SlotMachine.DI.Core.Installers;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace SlotMachine.DI.Core.Scopes
{
    public sealed class SceneLifetimeScope : LifetimeScope
    {
        [SerializeField] private MonoInstaller[] m_MonoInstallers;

        protected override void Awake()
        {
            base.Awake();
            AutoInjectSceneObjects();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            foreach (IInstaller installer in m_MonoInstallers)
                installer.Install(builder);
        }

        private void AutoInjectSceneObjects()
        {
            MonoBehaviour[] allMonoBehaviours = FindObjectsOfType<MonoBehaviour>(true);

            for (int i = 0; i < allMonoBehaviours.Length; i++)
            {
                MonoBehaviour mb = allMonoBehaviours[i];
                Container.Inject(mb);
            }
        }
    }
}
