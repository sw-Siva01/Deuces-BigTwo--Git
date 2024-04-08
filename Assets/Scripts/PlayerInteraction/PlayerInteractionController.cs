using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

#nullable enable

public class PlayerInteractionController : IPlayerInteractionController, IDisposable
{
    public bool IsOnPlay => _isOnPlay;

    private bool _isOnPlay;
    public bool _isWaitingForInput;
    private List<Card> _selectedCard = new();
    private ISubmittableCard? _submittableCard;

    private readonly CardInteractableCollection _cardInteractableCollection;
    private readonly ButtonInteractableCollection _buttonInteractableCollection;
    private readonly ISubmittableIdentifier _submittableIndentifier;
    private readonly ISubmittableComparator _submittableComparator;
    private readonly ITableInfo _tableInfo;

    public PlayerInteractionController(
        CardInteractableCollection cardInteractableCollection,
        ButtonInteractableCollection buttonInteractableCollection,
        ISubmittableIdentifier submittableIdentifier,
        ISubmittableComparator submittableComparator,
        ITableInfo tableInfo)
    {
        _cardInteractableCollection = cardInteractableCollection;
        _buttonInteractableCollection = buttonInteractableCollection;
        _submittableIndentifier = submittableIdentifier;
        _submittableComparator = submittableComparator;
        _tableInfo = tableInfo;
    }
    public void Setup(List<Card> playerCards)
    {
        for (int i = 0; i < playerCards.Count; i++)
        {
            _cardInteractableCollection.CardInteractables[i]
                .SetupReference(playerCards[i]);
        }
        
        _buttonInteractableCollection.SubmitButton?.onClick.AddListener(OnSubmitCard);
        _buttonInteractableCollection.PassButton?.onClick.AddListener(OnPass);
        _buttonInteractableCollection.OnTimerComplete += OnPass;

        _isOnPlay = true;
    }

    private void OnSubmitCard()
    {
        if (!_isWaitingForInput)
        {
            return;
        }

        Debug.LogFormat("selected card count {0}", _selectedCard.Count);

        var isValidSubmittable = _submittableIndentifier.TryGetSubmittable(
            _selectedCard,
            out var submittableCard);

        if (!isValidSubmittable)
        {
            Debug.Log("Invalid submittable card");
            return;
        }

        if (_tableInfo.IsFirstTurn)
        {
            _submittableCard = submittableCard;
            _isWaitingForInput = false;
        }
        else
        {
            isValidSubmittable = _submittableComparator.IsValidSubmittable(
                _tableInfo.CurrentSubmittableCard,
                submittableCard);

            if (isValidSubmittable)
            {
                _submittableCard = submittableCard;
                _isWaitingForInput = false;
            }
            else
            {
                Debug.Log("Invalid submittable card");
                return;
            }
        }
    }
    public void OnPass()
    {
        if (!_isWaitingForInput)
        {
            return;
        }

        _submittableCard = new SubmittableCard(PokerHandEnum.None, null!);

        _isWaitingForInput = false;
        Debug.Log("888888888888888888888888888");
    }

    public void SelectCard(Card card)
    {
        _selectedCard.Add(card);
    }

    public void DeselectCard(Card card)
    {
        _selectedCard.Remove(card);
    }

    public async UniTask<ISubmittableCard> GetPlayerActionAsync(CancellationToken cancellationToken)
    {
        _isWaitingForInput = true;

        _buttonInteractableCollection.gameObject.SetActive(true);

        await UniTask.WaitUntil(
            () => _isWaitingForInput == false,
            cancellationToken: cancellationToken);

        _buttonInteractableCollection.gameObject.SetActive(false);

        if (_submittableCard.PokerHand != PokerHandEnum.None)
        {
            for (int i = 0; i < _submittableCard.Cards.Count; i++)
            {
                _selectedCard.Remove(_submittableCard.Cards[i]);
            }
        }

        return _submittableCard;
    }

    public void Dispose()
    {
        _buttonInteractableCollection.SubmitButton?.onClick.RemoveListener(OnSubmitCard);
        _buttonInteractableCollection.PassButton?.onClick.RemoveListener(OnPass);
        _buttonInteractableCollection.OnTimerComplete -= OnPass;
    }
}
