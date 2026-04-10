using System;
using SlotMachine.DI.Core.Scopes;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable once InconsistentNaming
namespace SlotMachine.DI.Core
{
    /// <summary>
    /// Bootstrapper for Dependency Injection system.
    /// </summary>
    internal static class DIBootStrapper
    {
        private static bool s_Initialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (s_Initialized)
                return;

            ProjectLifetimeScope scopeResource = Resources.Load<ProjectLifetimeScope>("DI/ProjectLifetimeScope");

            if (scopeResource == null)
                throw new NullReferenceException("Ensure that the ProjectLifetimeScope resource exists at 'Resources/DI/ProjectLifetimeScope'.");

            ProjectLifetimeScope scopeInstance = Object.Instantiate(scopeResource);
            GameObject scopeGo = scopeInstance.gameObject;
            scopeGo.name = $"[{nameof(ProjectLifetimeScope)}]";
            scopeGo.hideFlags = HideFlags.NotEditable;

            Object.DontDestroyOnLoad(scopeGo);
            s_Initialized = true;
        }
    }
}
