using System;
using System.Threading;
/*using Alija.Big2.Client.AI;*/
using Cysharp.Threading.Tasks;

#nullable enable

public class ComputerParticipant : BaseParticipant
{
    public override ParticipantIdEnum Id => _id;
    public override ParticipantIdEnum NextId => _nextId;

    private ParticipantIdEnum _id;
    private ParticipantIdEnum _nextId;

    private readonly ITableInfo _tableInfo;
    private readonly IComputeSubmittable _computeSubmittable;

    public ComputerParticipant(
        ParticipantIdEnum participantId,
        string name,
        ParticipantIdEnum nextParticipantId,
        ICardCollection cardCollection,
        ITableInfo tableInfo,
        IComputeSubmittable computeSubmittable) : base(name, cardCollection)
    {
        _id = participantId;
        _nextId = nextParticipantId;
        _tableInfo = tableInfo;
        _computeSubmittable = computeSubmittable;
    }

    public override async UniTask StartTurnAsync(
        Action<ParticipantIdEnum, ISubmittableCard> onDone,
        CancellationToken cancellationToken)
    {
        var submittableCard = await _computeSubmittable.ComputeAsync(
            _handCard,
            _tableInfo.IsFirstTurn,
            _tableInfo.CurrentSubmittableCard ?? null!,
            cancellationToken: default);

        if (submittableCard.PokerHand != PokerHandEnum.None)
        {
            foreach (var card in submittableCard.Cards)
            {
                _handCard.Remove(card);
            }
        }

        onDone.Invoke(_id, submittableCard);
    }
}
