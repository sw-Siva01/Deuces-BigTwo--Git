/*using Alija.Big2.Client.Gameplay;*/
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

#nullable enable

public interface IComputeSubmittable
{
    UniTask<ISubmittableCard> ComputeAsync(
        List<Card> cards,
        bool isFirstTurn,
        ISubmittableCard tableSubmittable,
        CancellationToken cancellationToken);
}
