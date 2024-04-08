using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

#nullable enable

public abstract class BaseParticipant : IParticipant
{
    public abstract ParticipantIdEnum Id { get; }
    public abstract ParticipantIdEnum NextId { get; }
    public string Name => _name;
    public int CardCount => _handCard.Count;

    protected string _name;
    protected List<Card> _handCard = new();

    protected readonly ICardCollection _cardCollection;

    public BaseParticipant(
        string name,
        ICardCollection cardCollection)
    {
        _name = name;
        _cardCollection = cardCollection;
    }

    public void SetInitialCardInHandIndex(List<int> initialCardInHandIndex)
    {
        foreach (var cardIndex in initialCardInHandIndex)
        {
            _handCard.Add(_cardCollection.Cards[cardIndex]);
        }
    }

    public abstract UniTask StartTurnAsync(
        Action<ParticipantIdEnum, ISubmittableCard> onDone,
        CancellationToken cancellationToken);
}
