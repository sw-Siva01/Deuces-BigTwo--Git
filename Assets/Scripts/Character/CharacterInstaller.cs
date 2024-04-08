using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

#nullable enable

public class CharacterInstaller : MonoBehaviour, IInstaller
{
    [SerializeField]
    private CharacterSelectionPanel _characterSelectionPanel;

    public void Install(IContainerBuilder builder)
    {
        if (_characterSelectionPanel == null)
        {
            throw new InvalidOperationException("_characterSelectionPanel is null");
        }

        builder.RegisterComponent<CharacterSelectionPanel>(_characterSelectionPanel);

        builder.Register<CharacterSelectionService>(Lifetime.Singleton)
            .As<ICharacterSelectionService>();
    }
}
