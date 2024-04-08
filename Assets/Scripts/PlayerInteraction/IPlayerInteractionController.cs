using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

#nullable enable

public interface IPlayerInteractionController 
{
    bool IsOnPlay { get; }

    void Setup(List<Card> playerCard);
    void SelectCard(Card card);
    void DeselectCard(Card card);

    UniTask<ISubmittableCard> GetPlayerActionAsync(CancellationToken cancellationToken);
}
