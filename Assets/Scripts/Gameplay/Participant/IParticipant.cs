using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

#nullable enable

public interface IParticipant : IParticipantInfo
{
    void SetInitialCardInHandIndex(List<int> initialCardInHandIndex);
    UniTask StartTurnAsync(
        Action<ParticipantIdEnum, ISubmittableCard> onDone,
        CancellationToken cancellationToken);
}
