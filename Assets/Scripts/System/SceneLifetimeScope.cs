using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

#nullable enable

public class SceneLifetimeScope : LifetimeScope
{
    [SerializeField]
    private List<Object> _installerObjects = new();

    protected override void Configure(IContainerBuilder builder)
    {
        var isInstallerAvailable = TryGetInstallers(
            _installerObjects,
            out var installers);

        if (!isInstallerAvailable || installers == null)
        {
            return;
        }

        foreach (var installer in installers)
        {
            installer.Install(builder);
        }
    }

    private bool TryGetInstallers(
        List<Object> installerObjects,
        out List<IInstaller>? installers)
    {
        if (installerObjects.Count <= 0)
        {
            installers = null;
            return false;
        }

        installers = new List<IInstaller>();
        foreach (var installerObject in installerObjects)
        {
            if (installerObject is IInstaller installer)
            {
                installers.Add(installer);
            }
            else
            {
                Debug.LogErrorFormat(
                    "SLC | {0} is not an IInstaller",
                    installerObject);
            }
        }

        return installers.Count > 0;
    }
}