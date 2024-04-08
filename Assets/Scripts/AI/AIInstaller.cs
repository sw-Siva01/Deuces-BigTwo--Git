using UnityEngine;
using VContainer;
using VContainer.Unity;

#nullable enable

public class AIInstaller : MonoBehaviour, IInstaller
{
    public void Install(IContainerBuilder builder)
    {
        builder.Register<RandomComputeSubmittable>(Lifetime.Singleton)
            .As<IComputeSubmittable>();
    }
}