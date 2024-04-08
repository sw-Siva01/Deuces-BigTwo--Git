using System;
using System.Threading;
using Cysharp.Threading.Tasks;

#nullable enable

public class CharacterSelectionService : ICharacterSelectionService
{
    private CharacterView? _characterView = null;
    private bool _isSelected;

    private readonly CharacterSelectionPanel _characterSelectionPanel;

    public CharacterSelectionService(CharacterSelectionPanel characterSelectionPanel)
    {
        _characterSelectionPanel = characterSelectionPanel;
    }

    public CharacterView GetSelectedCharacter()
    {
        if (_characterView == null)
        {
            throw new InvalidOperationException("_characterView is null");
        }

        return _characterView;
    }

    // TODO will adjust this later
    public CharacterView GetComputerCharacter()
    {
        return _characterSelectionPanel.CharacterSelectibles[0].CharacterView;
    }

    public async UniTask ShowSelectCharacterAsync(CancellationToken cancellationToken)
    {
        foreach (var characterSelectible in _characterSelectionPanel.CharacterSelectibles)
        {
            characterSelectible.Setup(SelectCharacter);
        }

        _characterSelectionPanel.gameObject.SetActive(true);

        await UniTask.WaitUntil(
            () => _isSelected == true,
            cancellationToken: cancellationToken);

        _characterSelectionPanel.gameObject.SetActive(false);
    }

    private void SelectCharacter(CharacterView characterView)
    {
        _characterView = characterView;
        _isSelected = true;
    }
}
