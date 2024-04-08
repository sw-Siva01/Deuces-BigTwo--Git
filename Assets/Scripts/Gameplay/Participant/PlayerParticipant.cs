using System;
using System.Threading;
using Cysharp.Threading.Tasks;

#nullable enable

public class PlayerParticipant : BaseParticipant
{
    public override ParticipantIdEnum Id => ParticipantIdEnum.Player;
    public override ParticipantIdEnum NextId => _nextId;

    private ParticipantIdEnum _nextId;
    private readonly IPlayerInteractionController _playerInteractionController;

    public PlayerParticipant(
        string name,
        ParticipantIdEnum nextParticipantId,
        ICardCollection cardCollection,
        IPlayerInteractionController playerInteractionController) : base(name, cardCollection)
    {
        _nextId = nextParticipantId;
        _playerInteractionController = playerInteractionController;
    }

    public override async UniTask StartTurnAsync(
        Action<ParticipantIdEnum, ISubmittableCard> onDone,
        CancellationToken cancellationToken)
    {
        var submittableCard = await _playerInteractionController.GetPlayerActionAsync(cancellationToken);

        if (submittableCard.PokerHand != PokerHandEnum.None)
        {
            foreach (var card in submittableCard.Cards)
            {
                _handCard.Remove(card);
            }
        }

        onDone.Invoke(Id, submittableCard);
    }
}
