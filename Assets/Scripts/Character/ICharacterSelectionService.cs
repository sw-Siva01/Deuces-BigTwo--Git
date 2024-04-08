using Cysharp.Threading.Tasks;
using System.Threading;

#nullable enable

public interface ICharacterSelectionService
{
    CharacterView GetSelectedCharacter();
    CharacterView GetComputerCharacter();

    UniTask ShowSelectCharacterAsync(CancellationToken cancellationToken);
}
