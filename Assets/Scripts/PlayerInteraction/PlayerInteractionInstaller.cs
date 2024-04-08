using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

#nullable enable

public class PlayerInteractionInstaller : MonoBehaviour, IInstaller
{
    [SerializeField]
    private CardInteractableCollection _cardInteractableCollection;

    [SerializeField]
    private ButtonInteractableCollection _buttonInteractableCollection;

    public void Install(IContainerBuilder builder)
    {
        if (_cardInteractableCollection == null)
        {
            throw new InvalidOperationException("_cardInteractableCollection is null");
        }

        if (_buttonInteractableCollection == null)
        {
            throw new InvalidOperationException("_buttonInteractableCollection is null");
        }

        builder.RegisterComponent<CardInteractableCollection>(_cardInteractableCollection);

        builder.RegisterComponent<ButtonInteractableCollection>(_buttonInteractableCollection);

        builder.Register<PlayerInteractionController>(Lifetime.Singleton)
            .As<IPlayerInteractionController>();
    }
}