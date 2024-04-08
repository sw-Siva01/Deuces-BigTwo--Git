using System;
using System.Collections.Generic;

#nullable enable

public class TableMaster : ITableController, ITableInfo, ITableEventListener
{
    public event Action<List<IParticipantInfo>>? OnTableSetup;
    public event Action<ParticipantIdEnum, ISubmittableCard>? OnCardSubmitted;
    public event Action? OnTableCleared;
    public event Action? OnRoundEnded;

    public ISubmittableCard? CurrentSubmittableCard => _currentSubmittableCard;
    public bool IsFirstRound => _isFirstRound;
    public bool IsFirstTurn => _isFirstTurn;
    public int SubmittedCardCount => _submittedCardCount;
    public Dictionary<ParticipantIdEnum, IParticipantInfo> ParticipantInfoHasMap => _participantInfoHashMap;

    private bool _isInitialized;
    private ISubmittableCard? _currentSubmittableCard;
    private bool _isFirstRound;
    private bool _isFirstTurn;
    private int _submittedCardCount;
    private Dictionary<ParticipantIdEnum, IParticipantInfo> _participantInfoHashMap = new();
    private int _consecutionSubmittableNoneCount;

    public void Setup(List<IParticipantInfo> participants)
    {
        if (_isInitialized)
        {
            throw new InvalidOperationException("Invalid call! TableMaster already initialized");
        }

        foreach (var participant in participants)
        {
            _participantInfoHashMap.Add(participant.Id, participant);
        }

        _isFirstRound = true;
        _isFirstTurn = true;
        _submittedCardCount = 0;
        _isInitialized = true;
        _consecutionSubmittableNoneCount = 0;

        OnTableSetup?.Invoke(participants);
    }

    public void SubmitCard(
        ParticipantIdEnum participantId,
        ISubmittableCard submittedCard)
    {
        if (!_isInitialized)
        {
            throw new InvalidOperationException("Invalid call! TableMaster not yet initialized");
        }

        if (_isFirstRound)
        {
            _isFirstRound = false;
        }

        if (_isFirstTurn)
        {
            _isFirstTurn = false;
        }

        if (submittedCard.PokerHand != PokerHandEnum.None)
        {
            _currentSubmittableCard = submittedCard;
            _submittedCardCount += submittedCard.Cards.Count;
            _consecutionSubmittableNoneCount = 0;
        }
        else
        {
            _consecutionSubmittableNoneCount++;
        }

        if (_consecutionSubmittableNoneCount >= 3)
        {
            _isFirstTurn = true;
            _currentSubmittableCard = null;
            _consecutionSubmittableNoneCount = 0;
            OnRoundEnded?.Invoke();
        }

        OnCardSubmitted?.Invoke(participantId, submittedCard);
    }

    public void Clear()
    {
        if (!_isInitialized)
        {
            throw new InvalidOperationException("Invalid call! TableMaster not yet initialized");
        }

        _participantInfoHashMap.Clear();
        _isInitialized = false;

        OnTableCleared?.Invoke();
    }
}
